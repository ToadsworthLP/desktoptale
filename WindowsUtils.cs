using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    // Adapted from https://github.com/j3soon/OverlayWindow/tree/master
    public class WindowsUtils
    {
        #region DllImports
        [DllImport("kernel32.dll")]
        static extern void SetLastError(uint dwErrCode);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMarInset);
        
        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out NativeRect lpNativeRect);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeRect
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
        
        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
        #endregion
        
        const int GWL_EXSTYLE = -20;
        const int LWA_ALPHA = 0x00000002;
        const int WS_EX_LAYERED = 0x00080000;
        const int WS_EX_TOPMOST = 0x00000008;
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int WS_EX_NOACTIVATE = 0x08000000;
        static readonly IntPtr HWND_TOPMOST = (IntPtr)(-1);
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int S_OK = 0x00000000;
        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
        
        public static void MakeWindowOverlay(GameWindow window)
        {
            // Set to layered, transparent window.
            SetLastError(0);
            int ret = SetWindowLong(window.Handle, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST);
            if (ret == 0 && Marshal.GetLastWin32Error() != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            
            // Set to top-most window.
            if (!SetWindowPos(window.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            // Required in order to make layered window visible.
            if (!SetLayeredWindowAttributes(window.Handle, 0, 255, LWA_ALPHA))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            int[] margins = { -1 };
            if ((ret = DwmExtendFrameIntoClientArea(window.Handle, ref margins)) != S_OK)
                throw new Win32Exception(ret);

            window.IsBorderless = true;
        }

        public static void MakeTopmostWindow(GameWindow window)
        {
            if (!SetWindowPos(window.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static IList<WindowInfo> GetOpenWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            IList<WindowInfo> windows = new List<WindowInfo>();

            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                string title = builder.ToString();
                if (title == "Windows Input Experience") return true;
                if (title == ProgramInfo.NAME) return true;

                string processName = "???";
                uint processId;
                if (GetWindowThreadProcessId(hWnd, out processId) != 0)
                {
                    processName = Process.GetProcessById((int)processId).ProcessName;
                }

                windows.Add(new WindowInfo() {Title = title, ProcessName = processName, hWnd = hWnd});
                return true;
            }, 0);

            return windows;
        }

        public class WindowInfo
        {
            public string Title;
            public string ProcessName;
            public IntPtr hWnd;
        }

        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            if(!GetWindowRect(hWnd, out var nativeRect))
            {
                return Rectangle.Empty;
            }
            
            return new Rectangle(
                nativeRect.Left,
                nativeRect.Top,
                nativeRect.Right - nativeRect.Left,
                nativeRect.Bottom - nativeRect.Top
            );
        }

        public static void AttachConsole()
        {
            AttachConsole(ATTACH_PARENT_PROCESS);
        }
    }
}
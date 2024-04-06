using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class WindowsUtils
    {
        public static IntPtr MainWindowHwnd { get; private set; }
        
        #region DllImports
        [DllImport("kernel32.dll")]
        private static extern void SetLastError(uint dwErrCode);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMarInset);
        
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
        private static extern bool GetWindowRect(IntPtr hWnd, out NativeRect lpNativeRect);
        
        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        
        [DllImport("user32.dll", SetLastError=true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
        
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(NativePoint pt, int flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint { 
            public int x;
            public int y;
            
            public NativePoint(int x, int y) {
                this.x = x; 
                this.y = y;
            }

            public NativePoint(Point point)
            {
                this.x = point.X;
                this.y = point.Y;
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayDevice
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;

            public void Initialize()
            {
                cb = 0;
                DeviceName = new string((char)32, 32);
                DeviceString = new string((char)32, 128);
                DeviceID = new string((char)32, 128);
                DeviceKey = new string((char)32, 128);
                cb = Marshal.SizeOf(this);
            }
        }
        
        [Flags]
        private enum DisplayDeviceStateFlags
        {
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            PrimaryDevice = 0x4,
            MirroringDriver = 0x8,
            VGACompatible = 0x10,
            Removable = 0x20,
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000,
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct MonitorInfoEX
        {
            public int Size;
            public NativeRect Monitor;
            public NativeRect WorkArea;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
        }

        private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEX lpmi);

        [DllImport("User32.dll")]
        private static extern bool EnumDisplayDevices(byte[] lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, int dwFlags);
        #endregion
        
        private const int GWL_EXSTYLE = -20;
        private const int LWA_ALPHA = 0x00000002;
        private const int LWA_COLORKEY = 0x00000001;
        private const int ULW_COLORKEY = 0x00000001;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private static readonly IntPtr HWND_TOPMOST = (IntPtr)(-1);
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int S_OK = 0x00000000;
        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        public class SaveDialogResult
        {
            public DialogState Result;
            public Stream OutputStream;
            
            public enum DialogState { Succeeded, Cancelled, Failed }
        }
        
        public static void PrepareWindow(GameWindow window)
        {
            window.IsBorderless = true;
            
            SetLastError(0);
            SetWindowLong(window.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOPMOST);

            if (!SetLayeredWindowAttributes(window.Handle, 0, 255, LWA_ALPHA))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            
            int[] margins = { -1 };
            SetLastError(0);
            DwmExtendFrameIntoClientArea(window.Handle, ref margins);

            MainWindowHwnd = window.Handle;
        }

        public static void MakeClickable(GameWindow window)
        {
            SetWindowLong(window.Handle, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TOPMOST);
        }
        
        public static void MakeClickthrough(GameWindow window)
        {
            SetWindowLong(window.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOPMOST);
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

        public static WindowInfo GetWindowByName(string name)
        {
            IList<WindowInfo> allWindows = GetOpenWindows();
            
            // First, try matching by the exact window title
            foreach (WindowInfo window in allWindows)
            {
                if (window.Title == name) return window;
            }
            
            // Then try matching by process name
            foreach (WindowInfo window in allWindows)
            {
                if (window.ProcessName == name) return window;
            }

            return null;
        }

        public static Rectangle? GetWindowRect(IntPtr hWnd)
        {
            if(!GetWindowRect(hWnd, out var nativeRect))
            {
                return null;
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
        
        public static void RegisterForFileExtension(string extension)
        {
            try
            {
                string applicationPath = Assembly.GetExecutingAssembly().Location;
                RegistryKey fileReg =
                    Microsoft.Win32.Registry.CurrentUser.CreateSubKey($"Software\\Classes\\.{extension}");
                fileReg.CreateSubKey("shell\\open\\command").SetValue("", $"\"{applicationPath}\" \"%1\"");
                fileReg.Close();

                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);

                ShowMessageBox($"Successfully associated Desktoptale preset files (.{extension}) with this installation of Desktoptale!", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                ShowMessageBox($"Failed to associate Desktoptale preset files (.{extension}) with this installation of Desktoptale!", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        public static DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }

        public static void ShowDuplicateOptionsForm(Action<int> successAction, Action cancelAction)
        {
            DuplicateOptionsForm.Show(successAction, cancelAction);
        }

        public static SaveDialogResult OpenSaveDialog(string filter, string defaultName = "")
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = defaultName;
            dialog.Filter = filter;
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;

            DialogResult result = dialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                return new SaveDialogResult()
                {
                    Result = SaveDialogResult.DialogState.Succeeded,
                    OutputStream = dialog.OpenFile()
                };
            }

            if (result == DialogResult.Cancel)
            {
                return new SaveDialogResult() { Result = SaveDialogResult.DialogState.Cancelled };
            }

            return new SaveDialogResult() { Result = SaveDialogResult.DialogState.Failed };
        }

        public static bool IsPointOnScreen(Point point)
        {
            return MonitorFromPoint(new NativePoint(point), 0) != IntPtr.Zero;
        }

        public static bool IsRectOnFullyOnScreen(Rectangle rectangle)
        {
            Point topLeft = rectangle.Location;
            Point topRight = rectangle.Location + new Point(rectangle.Width, 0);
            Point bottomLeft = rectangle.Location + new Point(0, rectangle.Height);
            Point bottomRight = rectangle.Location + new Point(rectangle.Width, rectangle.Height);

            return IsPointOnScreen(topLeft) && IsPointOnScreen(topRight) && IsPointOnScreen(bottomLeft) && IsPointOnScreen(bottomRight);
        }

        public static void RegisterDisplaySettingsChangedCallback(Action callback)
        {
            SystemEvents.DisplaySettingsChanged += new EventHandler((sender, args) => callback());
        }

        public static IList<MonitorManager.MonitorInfo> GetConnectedMonitors()
        {
            List<MonitorManager.MonitorInfo> allMonitors = new List<MonitorManager.MonitorInfo>();

            bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData)
            {
                MonitorInfoEX monitor = new MonitorInfoEX() { Size = Marshal.SizeOf(typeof(MonitorInfoEX)) };

                if (GetMonitorInfo(hMonitor, ref monitor))
                {
                    DisplayDevice device = new DisplayDevice();
                    device.Initialize();

                    if (EnumDisplayDevices(ToLptString(monitor.DeviceName), 0, ref device, 0))
                    {
                        MonitorManager.MonitorInfo monitorInfo = new MonitorManager.MonitorInfo()
                        {
                            Name = device.DeviceString, 
                            DeviceId = monitor.DeviceName, 
                            Bounds = new Rectangle(monitor.Monitor.Left, monitor.Monitor.Top, monitor.Monitor.Right - monitor.Monitor.Left, monitor.Monitor.Bottom - monitor.Monitor.Top), 
                            WorkingArea = new Rectangle(monitor.WorkArea.Left, monitor.WorkArea.Top, monitor.WorkArea.Right - monitor.WorkArea.Left, monitor.WorkArea.Bottom - monitor.WorkArea.Top),
                        };
                        allMonitors.Add(monitorInfo);
                    }
                }

                return true;
            }

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
            return allMonitors;
        }

        private static byte[] ToLptString(string str)
        {
            var lptArray = new byte[str.Length + 1];

            var index = 0;
            foreach (char c in str.ToCharArray())
                lptArray[index++] = Convert.ToByte(c);

            lptArray[index] = Convert.ToByte('\0');

            return lptArray;
        }
    }
}
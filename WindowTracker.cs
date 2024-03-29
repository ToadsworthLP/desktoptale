using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class WindowTracker
    {
        private IDictionary<IntPtr, TrackedWindow> trackedWindows;
        private IDictionary<IntPtr, int> usages;

        private MonitorManager monitorManager;
        private bool changed = false;
        
        public WindowTracker(MonitorManager monitorManager)
        {
            this.monitorManager = monitorManager;
            
            trackedWindows = new Dictionary<IntPtr, TrackedWindow>();
            usages = new Dictionary<IntPtr, int>();
        }
        
        public void Update()
        {
            foreach (var window in trackedWindows.Values)
            {
                UpdateWindow(window);

                if (changed)
                {
                    break;
                }
            }
            
            if (changed)
            {
                changed = false;
            }
        }

        public TrackedWindow Subscribe(WindowInfo window)
        {
            if (!trackedWindows.ContainsKey(window.hWnd))
            {
                trackedWindows.Add(window.hWnd, new TrackedWindow(window));
                usages.Add(window.hWnd, 0);
            }

            usages[window.hWnd]++;
            return trackedWindows[window.hWnd];
        }

        public void Unsubscribe(WindowInfo window)
        {
            var updatesUses = --usages[window.hWnd];

            if (updatesUses <= 0)
            {
                trackedWindows.Remove(window.hWnd);
                usages.Remove(window.hWnd);
            }
        }

        private void UpdateWindow(TrackedWindow window)
        {
            Rectangle? rect = WindowsUtils.GetWindowRect(window.Window.hWnd);
            if (rect.HasValue)
            {
                Rectangle val = rect.Value;
                val.Location = monitorManager.ToMonoGameCoordinates(val.Location.ToVector2()).ToPoint();
                window.Bounds = val;
            }
            else
            {
                window.Bounds = Rectangle.Empty;
                window.NotifyWindowDestroyed();
                changed = true;
            }
        }
    }
}
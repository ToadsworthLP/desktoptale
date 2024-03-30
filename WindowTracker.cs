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
        
        private IEnumerator<TrackedWindow> windowUpdateEnumerator;
        
        public WindowTracker(MonitorManager monitorManager)
        {
            this.monitorManager = monitorManager;
            
            trackedWindows = new Dictionary<IntPtr, TrackedWindow>();
            usages = new Dictionary<IntPtr, int>();
        }
        
        public void Update()
        {
            if (trackedWindows.Count == 0)
            {
                windowUpdateEnumerator?.Dispose();
                return;
            }

            if (changed)
            {
                windowUpdateEnumerator?.Dispose();
                windowUpdateEnumerator = null;
                changed = false;
            }
            
            if (windowUpdateEnumerator == null)
            {
                windowUpdateEnumerator = trackedWindows.Values.GetEnumerator();
            }

            bool last = !windowUpdateEnumerator.MoveNext();
            if (last)
            {
                windowUpdateEnumerator.Reset();
                windowUpdateEnumerator.MoveNext();
            }
            
            TrackedWindow current = windowUpdateEnumerator.Current;
            UpdateWindow(current);
        }

        public TrackedWindow Subscribe(WindowInfo window)
        {
            if (!trackedWindows.ContainsKey(window.hWnd))
            {
                trackedWindows.Add(window.hWnd, new TrackedWindow(window));
                usages.Add(window.hWnd, 0);
                changed = true;
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
                changed = true;
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
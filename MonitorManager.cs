using System.Linq;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class MonitorManager
    {
        public MonitorInfo[] ConnectedMonitors { get; private set; }

        public MonitorManager()
        {
            WindowsUtils.RegisterDisplaySettingsChangedCallback(UpdateConnectedMonitors);
            UpdateConnectedMonitors();
        }

        public Vector2 GetClosestVisiblePoint(Vector2 point)
        {
            float minDistanceSquared = float.MaxValue;
            Vector2 closestPoint = point;
            
            foreach (var monitor in ConnectedMonitors)
            {
                Vector2 p = new Vector2(
                    MathF.Clamp(point.X, monitor.Bounds.Left, monitor.Bounds.Right),
                    MathF.Clamp(point.Y, monitor.Bounds.Top, monitor.Bounds.Bottom)
                );

                float distance = (point - p).LengthSquared();
                if (distance < minDistanceSquared)
                {
                    closestPoint = p;
                    minDistanceSquared = distance;
                }
            }

            return closestPoint;
        }
        
        private void UpdateConnectedMonitors()
        {
            ConnectedMonitors = WindowsUtils.GetConnectedMonitors().ToArray();
        }
        
        public class MonitorInfo
        {
            public Rectangle Bounds;
            public Rectangle WorkingArea;
            public string Name;
            public string DeviceId;
        }
    }
}
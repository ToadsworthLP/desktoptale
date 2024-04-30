using System.Linq;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class MonitorManager
    {
        public MonitorInfo[] ConnectedMonitors { get; private set; }
        public Rectangle BoundingRectangle { get; private set; }
        public Rectangle VirtualScreenBoundingRectangle { get; private set; }

        public MonitorManager()
        {
            WindowsUtils.RegisterDisplaySettingsChangedCallback(OnDisplaySettingsChanged);
            OnDisplaySettingsChanged();
        }
        
        public Vector2 ToMonoGameCoordinates(Vector2 input)
        {
            return input + new Vector2(
                BoundingRectangle.Location.X < 0 ? -BoundingRectangle.Location.X : 0,
                BoundingRectangle.Location.Y < 0 ? -BoundingRectangle.Location.Y : 0
            );
        }

        public Vector2 ToVirtualScreenCoordinates(Vector2 input)
        {
            return input + new Vector2(
                BoundingRectangle.Location.X < 0 ? BoundingRectangle.Location.X : 0,
                BoundingRectangle.Location.Y < 0 ? BoundingRectangle.Location.Y : 0
            );
        }

        public Vector2 GetClosestVisiblePoint(Vector2 point)
        {
            point = ToVirtualScreenCoordinates(point);
            
            float minDistanceSquared = float.MaxValue;
            Vector2 closestPoint = point;
            
            foreach (var monitor in ConnectedMonitors)
            {
                Vector2 p = new Vector2(
                    MathUtilities.Clamp(point.X, monitor.Bounds.Left, monitor.Bounds.Right),
                    MathUtilities.Clamp(point.Y, monitor.Bounds.Top, monitor.Bounds.Bottom)
                );

                float distance = (point - p).LengthSquared();
                if (distance < minDistanceSquared)
                {
                    closestPoint = p;
                    minDistanceSquared = distance;
                }
            }

            return ToMonoGameCoordinates(closestPoint);
        }

        private Rectangle GetBoundingRectangle()
        {
            Vector2 min = new Vector2(float.MaxValue);
            Vector2 max = new Vector2(float.MinValue);
            foreach (var monitor in ConnectedMonitors)
            {
                if (monitor.Bounds.X < min.X) min.X = monitor.Bounds.X;
                if (monitor.Bounds.X + monitor.Bounds.Width > max.X) max.X = monitor.Bounds.X + monitor.Bounds.Width;
                
                if (monitor.Bounds.Y < min.Y) min.Y = monitor.Bounds.Y;
                if (monitor.Bounds.Y + monitor.Bounds.Height > max.Y) max.Y = monitor.Bounds.Y + monitor.Bounds.Height;
            }

            var result = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            
            // Shrink it by one pixel on every side to convince Windows that it's not a fullscreen window
            // This makes sure that fullscreen content behind it (e.g. videos) continue to play instead of being "optimized" away
            result.Inflate(-1, -1);
            
            return result;
        }
        
        private void OnDisplaySettingsChanged()
        {
            ConnectedMonitors = WindowsUtils.GetConnectedMonitors().ToArray();
            BoundingRectangle = GetBoundingRectangle();
            VirtualScreenBoundingRectangle = new Rectangle(
                BoundingRectangle.Location.X + (BoundingRectangle.Location.X < 0 ? -BoundingRectangle.Location.X : 0),
                BoundingRectangle.Location.Y + (BoundingRectangle.Location.Y < 0 ? -BoundingRectangle.Location.Y : 0),
                BoundingRectangle.Width,
                BoundingRectangle.Height
            );
            
            MessageBus.Send(new DisplaySettingsChangedMessage());
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
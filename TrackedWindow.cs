using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class TrackedWindow
    {
        public delegate void TrackedWindowDestroyedEventHandler();

        public event TrackedWindowDestroyedEventHandler WindowDestroyed;
        
        public Rectangle Bounds { get; set; }
        public WindowInfo Window { get; }
        
        public TrackedWindow(WindowInfo window)
        {
            Window = window;
        }

        public void NotifyWindowDestroyed()
        {
            WindowDestroyed?.Invoke();
        }
    }
}
using Microsoft.Xna.Framework;

namespace Desktoptale.Distractions
{
    public interface IDistractionPattern
    {
        void Spawn(IDistractionManager manager, Rectangle screenRectangle);
    }
}
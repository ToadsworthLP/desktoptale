using Microsoft.Xna.Framework;

namespace Desktoptale.Distractions
{
    public interface IDistractionPattern
    {
        float Spawn(IDistractionManager manager, Rectangle screenRectangle);
    }
}
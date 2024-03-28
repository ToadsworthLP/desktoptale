using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public interface ICollider
    {
        Rectangle HitBox { get; }
        float Depth { get; }
    }
}
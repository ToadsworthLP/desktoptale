using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public interface IPhysicsObject
    {
        Rectangle HitBox { get; }
        float Depth { get; }

        void OnLeftClicked();
        void OnRightClicked();
    }
}
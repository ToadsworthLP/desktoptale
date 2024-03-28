using Microsoft.Xna.Framework;

namespace Desktoptale.Messages
{
    public class UpdateBoundaryMessage
    {
        public ICharacter Target { get; set; }
        public Rectangle? Boundary { get; set; }
    }
}
using Desktoptale.Characters;

namespace Desktoptale.Messages
{
    public class CharacterChangeRequestedMessage
    {
        public ICharacter Target { get; set; }
        public CharacterType Character { get; set; }
    }
}
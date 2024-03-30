using Desktoptale.Characters;

namespace Desktoptale.Messages
{
    public class AddCharacterRequestedMessage
    {
        public ICharacter Target { get; set; }
        public CharacterType Character { get; set; }
    }
}
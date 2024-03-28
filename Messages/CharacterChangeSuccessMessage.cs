using Desktoptale.Characters;

namespace Desktoptale.Messages
{
    public class CharacterChangeSuccessMessage
    {
        public ICharacter Target { get; set; }
        public CharacterType Character { get; set; }
    }
}
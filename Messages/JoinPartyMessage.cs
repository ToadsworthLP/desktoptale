namespace Desktoptale.Messages
{
    public class JoinPartyMessage
    {
        public Party Party { get; set; }
        public ICharacter Character { get; set; }
    }
}
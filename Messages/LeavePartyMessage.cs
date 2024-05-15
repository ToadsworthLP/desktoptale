namespace Desktoptale.Messages
{
    public class LeavePartyMessage
    {
        public Party Party { get; set; }
        public ICharacter Character { get; set; }
    }
}
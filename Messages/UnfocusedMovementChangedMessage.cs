namespace Desktoptale.Messages
{
    public class UnfocusedMovementChangedMessage
    {
        public ICharacter Target { get; set; }
        public bool Enabled { get; set; }
    }
}
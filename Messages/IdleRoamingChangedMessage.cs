namespace Desktoptale.Messages
{
    public class IdleRoamingChangedMessage
    {
        public ICharacter Target { get; set; }
        public bool Enabled { get; set; }
    }
}
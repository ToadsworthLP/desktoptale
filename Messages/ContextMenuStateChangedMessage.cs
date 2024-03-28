namespace Desktoptale.Messages
{
    public class ContextMenuStateChangedMessage
    {
        public ICharacter Target { get; set; }
        public bool Open { get; set; }
    }
}
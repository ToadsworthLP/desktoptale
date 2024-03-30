namespace Desktoptale.Messages
{
    public class ChangeContainingWindowMessage
    {
        public ICharacter Target { get; set; }
        public WindowInfo Window { get; set; }
    }
}
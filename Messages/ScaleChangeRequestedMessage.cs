namespace Desktoptale.Messages
{
    public class ScaleChangeRequestedMessage
    {
        public ICharacter Target { get; set; }
        public float ScaleFactor { get; set; }
    }
}
namespace Desktoptale
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Desktoptale game = new Desktoptale();
            game.Run();
            game.Dispose();
        }
    }
}
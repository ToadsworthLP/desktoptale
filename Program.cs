using System;
using CommandLine;

namespace Desktoptale
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                WindowsUtils.AttachConsole();
                
                Parser parser = new Parser(config =>
                {
                    config.HelpWriter = Console.Out;
                });
                parser.ParseArguments<Settings>(args)
                    .WithParsed<Settings>(Run)
                    .WithNotParsed(e =>
                    {
                        WindowsUtils.FreeConsole();
                    });
            }
            else
            {
                var settings = new Settings();
                Run(settings);
            }
        }

        private static void Run(Settings settings)
        {
            settings.Validate();
            
            Desktoptale game = new Desktoptale(settings);
            game.Run();
            game.Dispose();

            if (settings.PrintRegistryKeys)
            {
                WindowsUtils.FreeConsole();
            }
        }
    }
}
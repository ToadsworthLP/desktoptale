using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using CommandLine;
using Desktoptale.Messages;
using Desktoptale.Messaging;

namespace Desktoptale
{
    public static class Program
    {
        public const string AppId = "Desktoptale-14a4cfe9-1a59-45a4-b139-870346c425cb";
        public const string IpcChannelUri = "DesktoptaleIPC";
        
        [STAThread]
        public static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                WindowsUtils.AttachConsole();
                Console.WriteLine();
                
                Parser parser = new Parser(config =>
                {
                    config.HelpWriter = Console.Out;
                });
                parser.ParseArguments<Settings>(args)
                    .WithParsed<Settings>((settings) => Run(settings, args))
                    .WithNotParsed(e =>
                    {
                        WindowsUtils.FreeConsole();
                    });
            }
            else
            {
                var settings = new Settings();
                Run(settings, args);
            }
        }

        private static void Run(Settings settings, string[] args)
        {
            using (Mutex mutex = new Mutex(false, AppId))
            {
                if (!mutex.WaitOne(0))
                {
                    RunOtherInstance(args);
                    return;
                }

                RunFirstInstance(settings);
            }
        }

        private static void RunFirstInstance(Settings settings)
        {
            settings.Validate();
            
            IpcChannel channel = new IpcChannel(AppId);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcObject), IpcChannelUri, WellKnownObjectMode.Singleton);
            
            Desktoptale game = new Desktoptale(settings);
            game.Run();
            game.Dispose();

            if (settings.PrintRegistryKeys)
            {
                WindowsUtils.FreeConsole();
            }
        }

        private static void RunOtherInstance(string[] argsToPass)
        {
            IpcChannel channel = new IpcChannel();
            ChannelServices.RegisterChannel(channel, false);
            IpcObject ipcObject = (IpcObject)Activator.GetObject(typeof(IpcObject), $"ipc://{AppId}/{IpcChannelUri}");
            ipcObject.SendOtherInstanceStartedMessage(argsToPass);
        }

        private class IpcObject : MarshalByRefObject
        {
            public void SendOtherInstanceStartedMessage(string[] args)
            {
                MessageBus.Send(new OtherInstanceStartedMessage() { Args = args });
            }
        }
    }
}
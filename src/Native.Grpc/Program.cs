using System;
using Grpc.Core;
using MicrosoftChannels = System.Threading.Channels;
using Simple;

namespace Native.Grpc
{

    class Program
    {
        const int Port = 50051;

        public static void Main(string[] args)
        {
            var channel = MicrosoftChannels.Channel.CreateUnbounded<long>(new MicrosoftChannels.UnboundedChannelOptions()
            {
                SingleWriter = false,
                SingleReader = true
            });

            var server = new Server
            {
                Services = { Better.BindService(new BetServer()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}

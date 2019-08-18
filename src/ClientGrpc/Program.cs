using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Transport.Description;

namespace ClientGrpc
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var cts = new CancellationTokenSource();
            var client = new MessageServer.MessageServerClient(channel);
            var reply = await client.ClientConnectAsync(new Empty());

            using (var call = client.CallStream(new Empty(), Metadata.Empty, null, cts.Token))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    Console.WriteLine(responseStream.Current.MessageTypeCase);
                }
            }

            Console.WriteLine(reply.Status);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

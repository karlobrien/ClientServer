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
            Console.WriteLine(reply.status);

            // var client = new Better.BetterClient(channel);
            // String user = "you";

            // var reply = await client.PlaceBetAsync(new Bet {Horse = "Ista", Amt = 10});
            // Console.WriteLine("Greeting: " + reply.Status);
            // var cts = new CancellationTokenSource();
            // var st = client.RaceStatus(new Empty(), Metadata.Empty, null, cts.Token);

            // while (await st.ResponseStream.MoveNext())
            // {
            //     //cts.Cancel(); // cancel when first response.
            //     Console.WriteLine(st.ResponseStream.Current);
            // }
            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

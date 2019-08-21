using System;
using System.Threading.Tasks;
using Grpc.Core;
using Transport.Description;

namespace Server.Grpc
{
    public class ServerImpl : MessageServer.MessageServerBase
    {
        public override async Task<ServerResponse> ClientConnect(Empty request, ServerCallContext context)
        {
            Console.WriteLine($"Request received!");
            await Task.Delay(TimeSpan.FromSeconds(2));
            return new ServerResponse();
        }

        public override async Task CallStream(Empty request, IServerStreamWriter<StreamUpdate> responseStream, ServerCallContext context)
        {
            for(var i = 0; i < 100; i++)
            {
                StreamUpdate st = new StreamUpdate();
                if (i % 2 == 0)
                    st.Pulse = new HeartBeat { Tick = new Timestamp()};
                else
                    st.Price = new Price {Symbol = "VOD LN", RefPrice = 10};
                if (!context.CancellationToken.IsCancellationRequested)
                    await responseStream.WriteAsync(st);
            }
        }
    }
}
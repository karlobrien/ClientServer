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
            //return new ServerResponse {status = MessageStatus.Ack};
        }
    }
}
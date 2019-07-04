using System;
using System.Threading.Tasks;
using Grpc.Core;
using Simple;

namespace Native.Grpc
{
    public class SimpleImpl : Better.BetterBase
    {
        public override Task<BetResponse> PlaceBet(Bet bet, ServerCallContext context)
        {
            Console.WriteLine("Request Received");
            return Task.FromResult(new BetResponse {
                Id = 1, ClientId = 1, Status = BetResponse.Types.Status.Ack
            });
        }

        public override async Task RaceStatus(Empty request, IServerStreamWriter<RaceStream> responseStream, ServerCallContext context)
        {
            RaceStream rs = new RaceStream();
            RacePosition rp1 = new RacePosition();
            await responseStream.WriteAsync(rs);
        }
    }
}
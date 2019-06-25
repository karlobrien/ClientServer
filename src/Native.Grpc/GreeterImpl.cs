using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace Native.Grpc
{
    public class GreeterImpl : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }
}

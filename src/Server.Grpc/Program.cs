using Grpc.Core;
using System;
using Transport.Description;
using GrpcServer = Grpc.Core.Server;

namespace Server.Grpc
{
    class Program
    {
        const int Port = 50051;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Message Server!");

            var server = new GrpcServer
            {
                Services = { MessageServer.BindService(new ServerImpl()) },
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

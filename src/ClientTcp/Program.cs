using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientTcp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string baseUrl = null;
            var uri = baseUrl == null ? new Uri("net.tcp://127.0.0.1:9001") : new Uri(baseUrl);
            Console.WriteLine("Connecting to {0}", uri);
            var connectionBuilder = new HubConnectionBuilder();

            if (uri.Scheme == "net.tcp")
            {
                connectionBuilder.WithEndPoint(uri);
            }
            else
            {
                connectionBuilder.WithUrl(uri);
            }

            var connection = connectionBuilder.Build();

            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                connection.DisposeAsync().GetAwaiter().GetResult();
            };

            await connection.StartAsync();
            var reader = await connection.StreamAsChannelAsync<int>("ChannelCounter", 10, 2000);

            while (await reader.WaitToReadAsync())
            {
                while (reader.TryRead(out var item))
                {
                    Console.WriteLine($"received: {item}");
                }
            }

        }
    }
}

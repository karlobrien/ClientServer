using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientHttp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string baseUrl = "http://localhost:5000/main";

            var connection = new HubConnectionBuilder()
                .WithUrl(baseUrl)
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };

            await connection.StartAsync();
            connection.On<string>("Send", Console.WriteLine);
            await connection.SendCoreAsync("Echo", new object[]{"TEST"});

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

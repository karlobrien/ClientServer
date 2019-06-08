using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tcp.Signalr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();

            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseSetting(WebHostDefaults.PreventHostingStartupKey, "true")
                .ConfigureLogging(factory =>
                {
                    factory.AddConsole();
                })
                .UseKestrel(options =>
                {
                    // Default port
                    options.ListenLocalhost(5000);

                    // Hub bound to TCP end point
                    options.Listen(IPAddress.Any, 9001, builder =>
                    {
                        builder.UseHub<Streaming>();
                    });
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

                host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

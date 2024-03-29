﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Connections;

namespace Tcp.Native
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    // This shows how a custom framework could plug in an experience without using Kestrel APIs directly
                    //services.AddFramework(new IPEndPoint(IPAddress.Loopback, 8009));
                })
                .UseKestrel(options =>
                {
                    // TCP 8007
                    options.ListenLocalhost(8007, builder =>
                    {
                        builder.UseConnectionHandler<MyEchoConnectionHandler>();
                    });

                    // HTTP 5000
                    //options.ListenLocalhost(5000);

                    // HTTPS 5001
                    /* options.ListenLocalhost(5001, builder =>
                    {
                        builder.UseHttps();
                    });*/
                })
                .UseStartup<Startup>();
    }
}

using System;
using System.Buffers;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Tcp.Native
{
    public static class MyFrameworkExtensions
    {
        public static IServiceCollection AddFramework(this IServiceCollection services, IPEndPoint endPoint)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<KestrelServerOptions>, MyFrameworkOptionsSetup>());

            services.Configure<MyFrameworkOptions>(o =>
            {
                o.EndPoint = endPoint;
            });

            services.TryAddSingleton<IFrameworkMessageParser, FrameworkMessageParser>();
            return services;
        }
    }

    public class MyFrameworkOptionsSetup : IConfigureOptions<KestrelServerOptions>
    {
        private readonly MyFrameworkOptions _options;

        public MyFrameworkOptionsSetup(IOptions<MyFrameworkOptions> options)
        {
            _options = options.Value;
        }

        public void Configure(KestrelServerOptions options)
        {
            options.Listen(_options.EndPoint, builder =>
            {
                builder.UseConnectionHandler<MyFrameworkConnectionHandler>();
            });
        }

        // This is the connection handler the framework uses to handle new incoming connections
        private class MyFrameworkConnectionHandler : ConnectionHandler
        {
            private readonly IFrameworkMessageParser _parser;

            public MyFrameworkConnectionHandler(IFrameworkMessageParser parser)
            {
                _parser = parser;
            }

            public override async Task OnConnectedAsync(ConnectionContext connection)
            {
                var input = connection.Transport.Input;
                var output = connection.Transport.Output;


                // Code to parse framework messages
                while (true)
                {
                    var result = await input.ReadAsync();
                    var buffer = result.Buffer;

/*
                    if (_parser.TryParseMessage(ref buffer, out var message))
                    {
                        Console.WriteLine("Inside Parser");
                        await ProcessMessageAsync(message);
                    }
*/
                    input.AdvanceTo(buffer.Start, buffer.End);
                }
            }

            private Task ProcessMessageAsync(Message message)
            {
                return Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }

    // The framework exposes options for how to bind
    public class MyFrameworkOptions
    {
        public IPEndPoint EndPoint { get; set; }
    }

    // The framework exposes a message parser used to parse incoming protocol messages from the network
    public interface IFrameworkMessageParser
    {
        bool TryParseMessage(ref ReadOnlySequence<byte> buffer, out Message message);
    }

    public class FrameworkMessageParser : IFrameworkMessageParser
    {
        public bool TryParseMessage(ref ReadOnlySequence<byte> buffer, out Message message)
        {
            // TODO: Implement logic here
            message = new Message();
            message.Name = "Karl";
            message.Age = "18";
            return true;
        }
    }

    public class Message
    {
        // TODO: Add properties relevant to your message type here
        public string Name {get;set;}
        public string Age {get;set;}
    }
}

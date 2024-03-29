using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Tcp.Native
{
    public class MyEchoConnectionHandler : ConnectionHandler
    {
        private readonly ILogger<MyEchoConnectionHandler> _logger;

        public MyEchoConnectionHandler(ILogger<MyEchoConnectionHandler> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            _logger.LogInformation(connection.ConnectionId + " connected");

            while (true)
            {
                var result = await connection.Transport.Input.ReadAsync();
                var buffer = result.Buffer;

                foreach (var segment in buffer)
                {
                    _logger.LogInformation($"Reading {segment.Length}");
                    await connection.Transport.Output.WriteAsync(segment);
                }

                if (result.IsCompleted)
                {
                    break;
                }

                connection.Transport.Input.AdvanceTo(buffer.End);
            }

            _logger.LogInformation(connection.ConnectionId + " disconnected");
        }
    }
}

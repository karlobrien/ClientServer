using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using System.Reactive.Linq;

namespace Http.Signalr
{
    public class MainHub : Hub
    {
        public Task Echo(string message)
        {
            return Clients.Caller.SendAsync("Send", $"{Context.ConnectionId}: {message}");
        }
        public ChannelReader<int> ObservableCounter(int count, double delay)
        {
            var observable = Observable.Interval(TimeSpan.FromMilliseconds(delay))
                             .Select((_, index) => index)
                             .Take(count);

            return observable.AsChannelReader(Context.ConnectionAborted);
        }

        public ChannelReader<int> ChannelCounter(int count, int delay)
        {
            var channel = Channel.CreateUnbounded<int>();

            Task.Run(async () =>
            {
                for (var i = 0; i < count; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    await Task.Delay(delay);
                }

                channel.Writer.TryComplete();
            });

            return channel.Reader;
        }
    }

}

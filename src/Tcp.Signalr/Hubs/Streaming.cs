using System;
using System.Reactive.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tcp.Signalr;

public class Streaming : Hub
    {
        public ChannelReader<int> ObservableCounter(int count, int delay)
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
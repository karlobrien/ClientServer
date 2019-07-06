using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Native.Grpc
{
    public class BoundedMessageChannel
    {
        private const int MaxMessagesInChannel = 250;
        private readonly Channel<long> _channel;

        public BoundedMessageChannel()
        {
            var options = new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleReader = true,
                SingleWriter = true
            };

            _channel = Channel.CreateBounded<long>(options);

            Task.Factory.StartNew(async () =>
            {
                // Wait while channel is not empty and still not completed
                while (await _channel.Reader.WaitToReadAsync())
                {
                    var job = await _channel.Reader.ReadAsync();
                    Console.WriteLine(job);
                    //Call betting
                }
            }, TaskCreationOptions.LongRunning);
        }

        public async Task WriteMessagesAsync(long messages, CancellationToken ct = default)
        {
            while(await _channel.Writer.WaitToWriteAsync(ct) && !ct.IsCancellationRequested)
            {
                _channel.Writer.TryWrite(messages);
            }
        }
        public void CompleteWriter(Exception ex = null) =>_channel.Writer.Complete(ex);

        public bool TryCompleteWriter(Exception ex = null) => _channel.Writer.TryComplete(ex);
    }
}
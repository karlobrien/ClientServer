using System;
using System.Threading;
using System.Threading.Tasks;
using MicrosoftChannels = System.Threading.Channels;

namespace Native.Grpc
{
    public interface IIncomingMessageQueue
    {
        ValueTask<bool> Enqueue(long msg);
    }

    public class IncomingMessageQueue : IIncomingMessageQueue
    {
        private readonly IIncomingMessageProcessor _msgProcessor;
        private readonly MicrosoftChannels.ChannelWriter<long> _writer;
        public IncomingMessageQueue(IIncomingMessageProcessor msgProcessor, MicrosoftChannels.ChannelWriter<long> writer)
        {
            _msgProcessor = msgProcessor;
            _writer = writer;
        }

        public void Start()
        {
            //start thread that reads and processes the queue
        }
        public ValueTask<bool> Enqueue(long msg)
        {
            async Task<bool> AsyncSlowPath(long thing)
            {
                while (await _writer.WaitToWriteAsync())
                {
                    if (_writer.TryWrite(thing)) return true;
                }
                return false; // Channel was completed during the wait
            }

            return _writer.TryWrite(msg) ? new ValueTask<bool>(true) : new ValueTask<bool>(AsyncSlowPath(msg));
        }
    }
}
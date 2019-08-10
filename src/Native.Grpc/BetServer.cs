using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Grpc.Core;
using Simple;
using Ch = System.Threading.Channels;

namespace Native.Grpc
{
    public class BetServer : Better.BetterBase
    {
        private readonly SubscriberManager _subscriberManager;
        public BetServer()
        {
            _subscriberManager = new SubscriberManager();
             var obs = Observable.Interval(TimeSpan.FromSeconds(1));
        }

        public override Task<BetResponse> PlaceBet(Bet bet, ServerCallContext context)
        {
            Console.WriteLine("Request Received");
            return Task.FromResult(new BetResponse {
                Id = 1, ClientId = 1, Status = BetResponse.Types.Status.Ack
            });
        }

        public override async Task RaceStatus(Empty request, IServerStreamWriter<RaceStream> responseStream, ServerCallContext context)
        {
            Subscriber newSub = new Subscriber("karl", 250);
            _subscriberManager.AddSubscriber(newSub);


        }
    }

    public class SubscriberManager
    {
        private ConcurrentDictionary<string, Subscriber> _subscribers;

        public SubscriberManager()
        {
            _subscribers = new ConcurrentDictionary<string, Subscriber>();
        }

        public async Task BroadcastMessageAsync(RaceStream message)
        {
            await BroadcastMessages(message);
        }


        public void AddSubscriber(Subscriber subscriber)
        {
            bool added = _subscribers.TryAdd(subscriber.Name, subscriber);
            if (!added)
            {
                //_logger.LogInformation($"could not add subscriber: {subscriber.Name}");
            }
        }

        public void RemoveSubscriber(Subscriber subscriber)
        {
            try
            {
                _subscribers.TryRemove(subscriber.Name, out Subscriber item);
                //_logger.LogInformation($"Force Remove: {item.Name} - no longer works");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Could not remove {subscriber.Name}");
            }
        }

        private async Task BroadcastMessages(RaceStream message)
        {
            foreach (var subscriber in _subscribers.Values)
            {
                var item = await SendMessageToSubscriber(subscriber, message);
                if (item != null)
                {
                    RemoveSubscriber(item);
                };
            }
        }

        private async Task<Subscriber> SendMessageToSubscriber(Subscriber subscriber, RaceStream message)
        {
            try
            {
                //_logger.LogInformation($"Broadcasting: {message.Name} - {message.Message}");
                await subscriber.Enqueue(message);
                return null;
            }
            catch(Exception ex)
            {
                //_logger.LogError(ex, "Could not send");
                return subscriber;
            }
        }
    }

    public class Subscriber
    {
        public string Name {get;}
        private Ch.ChannelWriter<RaceStream> _writer;
        private Ch.ChannelReader<RaceStream> _reader;
        public Subscriber(string name, int capacity)
        {
            Name = name;
            var options = new BoundedChannelOptions(capacity)
            {
                SingleReader = true,
                SingleWriter = true,
                //FullMode =
            };

            var channel = Ch.Channel.CreateBounded<RaceStream>(options);
            _writer = channel.Writer;
            _reader = channel.Reader;
        }

        public ValueTask<bool> Enqueue(RaceStream rs)
        {
            async Task<bool> AsyncSlowPath(RaceStream thing)
            {
                while (await _writer.WaitToWriteAsync())
                {
                    if (_writer.TryWrite(thing)) return true;
                }
                return false; // Channel was completed during the wait
            }

            return _writer.TryWrite(rs) ? new ValueTask<bool>(true) : new ValueTask<bool>(AsyncSlowPath(rs));
        }
    }
}
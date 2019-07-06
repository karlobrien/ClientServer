using System;
using Simple;
using static Simple.Bet.Types;

namespace Native.Grpc
{
    public interface IIncomingMessageProcessor
    {
        void OnEvent(Bet clientBet);
    }

    public class IncomingMessageProcessor : IIncomingMessageProcessor
    {
        private readonly IOutgoingQueue _outgoingQueue;
        public IncomingMessageProcessor(IOutgoingQueue outgoingQueue)
        {

        }

        public void OnEvent(Bet clientBet)
        {
            //decide the type of message and give it to the dispatcher
            //also put it on the outgoing
            switch (clientBet.Type)
            {
                case BetType.Win:
                {
                    break;
                }
                case BetType.Eachway:
                {
                    break;
                }
                default:
                    break;
            }
        }
    }
}
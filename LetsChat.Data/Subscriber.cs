using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace LetsChat.Data
{
    public class Subscriber : SubscriptionClient
    {
        private const string SubscriptionKey = "ServiceBusSubscription";

        public Subscriber(IConfiguration config)
            : base(config[MessageRepository.SBConnectionString], config[MessageRepository.TopicKey],
                config[SubscriptionKey], ReceiveMode.PeekLock, RetryPolicy.Default)
        {

        }
    }
}

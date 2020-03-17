using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace LetsChat.Data
{
    public class Publisher : TopicClient
    {
        public Publisher(IConfiguration config)
            : base(config[MessageRepository.SBConnectionString], config[MessageRepository.TopicKey], RetryPolicy.Default)
        { }
    }
}

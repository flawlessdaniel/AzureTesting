using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace AzureMessaging.Managers
{
    internal class ServiceBusManager : IServiceBusManager
    {
        private readonly IQueueClient _queueClient;
        private readonly ITopicClient _topicClient;

        public ServiceBusManager(
            IQueueClient queueClient, 
            ITopicClient topicClient)
        {
            _queueClient = queueClient;
            _topicClient = topicClient;
        }

        public async Task EnqueueMessage<TMessage>(TMessage Message) where TMessage : struct
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Message));
            await _queueClient.SendAsync(new Message(data));
        }

        public async Task ProduceMessage<TMessage>(TMessage Message) where TMessage : struct
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Message));
            var topicMessage = new Message(data);
            if (topicMessage.UserProperties.TryAdd("Type",typeof(TMessage).Name))
                await _topicClient.SendAsync(topicMessage);
        }
    }
}

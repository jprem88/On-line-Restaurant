
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mongo.MessageBus
{
    public class MessageBus : IMessageBus
    {
        //private string connString = "Endpoint=sb://mango-project.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=cpm4H2NWApimWwkRIxvSgnYayIPLQdFUg+ASbEmPEEw=";
        private string connString = "Endpoint=sb://resoturent-project.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2vKiJmpOiHDT9OIJCw0RcS9WeiwyxHPj/+ASbLDOcrg=";
        public async Task PublishMessage(object message, string topic_queue_name)
        {

            await using var client = new ServiceBusClient(connString);
            ServiceBusSender serviceBusSender = client.CreateSender(topic_queue_name);
            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = new Guid().ToString()

            };
          await  serviceBusSender.SendMessageAsync(serviceBusMessage);
            await client.DisposeAsync();
        }
    }
}

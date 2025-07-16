using Azure.Messaging.ServiceBus;
using Mango.Services.RewardApi.Message;
using Mango.Services.RewardApi.Service;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string connectionString;
        private readonly string queueName;
        private readonly string userQueueName;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _emilProcessor;
        private readonly EmailService _emailService;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            connectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            queueName = _configuration.GetValue<string>("TopicQueueName:OrderCompletedQueue");


            var client = new ServiceBusClient(connectionString);
            _emilProcessor = client.CreateProcessor(queueName);
            _emailService = emailService;
        }

        public async Task Start()
        {
            _emilProcessor.ProcessMessageAsync += OnEmailRequestRecived;
            _emilProcessor.ProcessErrorAsync += ErrorHandler;
            _emilProcessor.StartProcessingAsync();
        }

        private  Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailRequestRecived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(Convert.ToString(body));
            try
            {
                // try to log email
               await _emailService.UpdateRewards(rewardMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch(Exception ex)
            {
                throw;
            }
        }


        public async Task Stop()
        {
            await _emilProcessor.StopProcessingAsync();
            await _emilProcessor.DisposeAsync();
        }
    }
}

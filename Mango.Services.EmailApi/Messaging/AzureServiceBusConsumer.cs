using Azure.Messaging.ServiceBus;
using Mango.Services.EmailApi.Models.Dto;
using Mango.Services.EmailApi.Service;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string connectionString;
        private readonly string queueName;
        private readonly string userQueueName;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _emilProcessor;
        private readonly ServiceBusProcessor _userProcessor;
        private readonly EmailService _emailService;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            connectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            queueName = _configuration.GetValue<string>("TopicQueueName:EmailShopingCart");
            userQueueName = _configuration.GetValue<string>("TopicQueueName:UserRegistration");


            var client = new ServiceBusClient(connectionString);
            _emilProcessor = client.CreateProcessor(queueName);
            _userProcessor = client.CreateProcessor(userQueueName);
            _emailService = emailService;
        }

        public async Task Start()
        {
            _emilProcessor.ProcessMessageAsync += OnEmailRequestRecived;
            _emilProcessor.ProcessErrorAsync += ErrorHandler;
            _emilProcessor.StartProcessingAsync();

            _userProcessor.ProcessMessageAsync += OnUserEmailRequestRecived;
            _userProcessor.ProcessErrorAsync += ErrorHandler;
            _userProcessor.StartProcessingAsync();
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
            CartDto cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(body));
            try
            {
                // try to log email
               await _emailService.EmailCartLog(cart);
                await args.CompleteMessageAsync(args.Message);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private async Task OnUserEmailRequestRecived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RegistrationRequestDto user = JsonConvert.DeserializeObject<RegistrationRequestDto>(Convert.ToString(body));
            try
            {
                // try to log email
                await _emailService.EmailUserLog(user);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
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

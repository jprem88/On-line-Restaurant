using Mango.Services.EmailApi.Messaging;
using System.Runtime.CompilerServices;

namespace Mango.Services.EmailApi.Extension
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer azureServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseServiceBusConsumer(this IApplicationBuilder applicationBuilder)
        {
            azureServiceBusConsumer = applicationBuilder.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLifeTime = applicationBuilder.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationLifeTime.ApplicationStarted.Register(OnStart);
            hostApplicationLifeTime.ApplicationStopping.Register(OnStop);
            return applicationBuilder;
        }

        private static void OnStop()
        {
            azureServiceBusConsumer.Stop();
        }

        private static void OnStart()
        {
            azureServiceBusConsumer.Start();
        }
    }
}

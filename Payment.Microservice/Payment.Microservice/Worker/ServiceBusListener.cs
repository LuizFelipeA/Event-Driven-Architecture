using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Payment.Microservice.Dtos;
using Payment.Microservice.Models;
using Payment.Microservice.Services.Interfaces;
using System.Text;

namespace Payment.Microservice.Worker
{
    /// <summary>
    /// This class will talk with the external provider and once the provider returns,
    /// it's going to post a message on the ServiceBus topic
    /// </summary>
    public class ServiceBusListener : IHostedService
    {
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;

        private readonly IExternalGatewayPaymentService _externalGatewayPaymentService;

        private readonly Microsoft.Azure.ServiceBus.I _messageBus;

        private ISubscriptionClient _subscriptionClient;

        public ServiceBusListener(
            ILogger logger,
            IConfiguration configuration,
            IExternalGatewayPaymentService externalGatewayPaymentService)
        {
            _logger = logger;
            _configuration = configuration;
            _externalGatewayPaymentService = externalGatewayPaymentService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            var paymentRequestMessageTopic = _configuration.GetValue<string>("OrderPaymentMessageTopic");

            var subscriptionName = "subscriptionName";

            _subscriptionClient = new SubscriptionClient(
                serviceBusConnectionString,
                paymentRequestMessageTopic,
                subscriptionName);

            var messageHandlerOptions =
                new MessageHandlerOptions(e => 
            {
                ProcessError(e.Exception);
                return Task.CompletedTask;
            })
            {
                MaxConcurrentCalls = 3,
                AutoComplete = true
            };

            _subscriptionClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected void ProcessError(Exception exception)
        {
            _logger.LogError(exception, "Error while processing Queue item in ServiceBusListener");
        }

        protected async Task ProcessMessageAsync(
            Message message,
            CancellationToken cancellationToken)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);

            var orderPaymentRequestMessage = JsonConvert.DeserializeObject<OrderPaymentMessageRequestModel>(messageBody);

            var paymentInfo = new PaymentInfoDto(
                cardNumber: orderPaymentRequestMessage.CardNumber,
                cardName: orderPaymentRequestMessage.CardName,
                cardExpiration: orderPaymentRequestMessage.CardExpiration,
                total: orderPaymentRequestMessage.Total);

            // Calls the external gateway service by HTTP request
            var paymentSuccess = await _externalGatewayPaymentService.PerformPayment(paymentInfo);

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            // Create a new message UPDATED to post the order again via service bus
            var orderPaymentUpdateMessage = new OrderPaymentUpdateMessage(
                orderId: orderPaymentRequestMessage.OrderId,
                paymentSuccess: paymentSuccess);

            try
            {
                //Calls PublishMessage Method
            }
            catch (Exception)
            {

                throw;
            }

            _logger.LogDebug($"{orderPaymentRequestMessage.OrderId}: ServiceListener received item.");
        }
    }
}

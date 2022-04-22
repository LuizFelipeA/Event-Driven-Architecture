using Newtonsoft.Json;
using Payment.Microservice.Dtos;
using Payment.Microservice.Services.Interfaces;
using System.Net.Http.Headers;

namespace Payment.Microservice.Services
{
    public class ExternalGatewayPaymentService : IExternalGatewayPaymentService
    {
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        public ExternalGatewayPaymentService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> PerformPayment(PaymentInfoDto paymentInfo)
        {
            var paymentDataAsString = JsonConvert.SerializeObject(paymentInfo);

            var requestContent = new StringContent(paymentDataAsString);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var baseUrl = _configuration.GetValue<string>("https://www.testLP.com/");

            var paymentProviderResponse = await _httpClient.PostAsync(baseUrl, requestContent);

            if (!paymentProviderResponse.IsSuccessStatusCode)
                throw new ApplicationException(
                    $"Something went wrong calling the API: {paymentProviderResponse.ReasonPhrase}");

            var paymentProviderResponseString = await paymentProviderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            var response = JsonConvert.DeserializeObject<bool>(paymentProviderResponseString);

            return response;
        }
    }
}

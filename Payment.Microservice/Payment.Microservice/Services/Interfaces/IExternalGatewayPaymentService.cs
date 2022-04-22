using Payment.Microservice.Dtos;

namespace Payment.Microservice.Services.Interfaces
{
    public interface IExternalGatewayPaymentService
    {
        Task<bool> PerformPayment(PaymentInfoDto paymentInfo);
    }
}

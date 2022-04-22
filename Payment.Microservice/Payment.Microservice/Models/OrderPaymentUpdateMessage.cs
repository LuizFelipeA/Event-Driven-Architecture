namespace Payment.Microservice.Models
{
    public class OrderPaymentUpdateMessage
    {
        public int OrderId { get;  }

        public bool PaymentSuccess { get; }

        public OrderPaymentUpdateMessage(
            int orderId,
            bool paymentSuccess)
        {
            OrderId = orderId;
            PaymentSuccess = paymentSuccess;
        }
    }
}

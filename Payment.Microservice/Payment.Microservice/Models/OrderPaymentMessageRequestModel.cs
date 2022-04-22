namespace Payment.Microservice.Models
{
    public class OrderPaymentMessageRequestModel
    {
        public int OrderId { get; set; }

        public string? CardNumber { get; set; }

        public string? CardName { get; set; }

        public string? CardExpiration { get; set; }

        public decimal Total { get; set; }
    }
}

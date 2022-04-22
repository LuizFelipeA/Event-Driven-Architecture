namespace Payment.Microservice.Dtos
{
    public class PaymentInfoDto
    {
        public string? CardNumber { get; }

        public string? CardName { get; }

        public string? CardExpiration { get;}

        public decimal Total { get; }

        public PaymentInfoDto(
            string? cardNumber,
            string? cardName,
            string? cardExpiration,
            decimal total)
        {
            CardNumber = cardNumber;
            CardName = cardName;
            CardExpiration = cardExpiration;
            Total = total;
        }
    }
}

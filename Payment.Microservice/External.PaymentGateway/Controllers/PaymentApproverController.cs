using External.PaymentGateway.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace External.PaymentGateway.Controllers
{
    [ApiController]
    [Route("api/paymentApprover")]

    public class PaymentApproverController : Controller
    {
        /// <summary>
        /// This endPoint simulate a payment request to an external payment provider
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns>True or False</returns>
        
        [HttpPost]
        public IActionResult TryPayment([FromBody] PaymentDto paymentRequest)
        {
            int num = new Random().Next(1000);

            if (num > 500)
                return Ok(true);

            return Ok(false);
        }
    }
}

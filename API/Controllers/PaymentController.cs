using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain;
using Domain.Validation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentservice;
        public PaymentController(IPaymentService paymentservice)
        {
            _paymentservice = paymentservice;
        }

        [HttpPost("ProcessPayment")]
        public async Task<ActionResult> ProcessPayment([FromBody] Payment payment)
        {
            if (payment.IsValid(out IEnumerable<string> errors))
            {
                var result = await _paymentservice.Process(payment).ConfigureAwait(false);

                return Ok(result);
            }else{
                return BadRequest(errors);
            }
        }
    }
}
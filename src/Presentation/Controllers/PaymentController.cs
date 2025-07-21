using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Presentation.Models.Payment;
using Presentation.Models.Settings;

namespace Presentation.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentServiceSettings _settings;
        private readonly IPaymentService _paymentService;

        public PaymentController(IOptions<PaymentServiceSettings> settings, IPaymentService paymentService)
        {
            _settings = settings.Value;
            _paymentService = paymentService;
        }

        public ActionResult Index()
        {
            return View(new PaymentInputModel());
        }

        [HttpPost]
        public async Task<ActionResult> StartPayment(PaymentInputModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var tokenRequest = new GetTokenRequest
            {
                Amount = model.Amount,
                ReturnUrl = Url.Action("Verify", "Payment")
            };

            var token = await _paymentService.GetTokenAsync(tokenRequest);

            if (string.IsNullOrEmpty(token))
            {
                return Content("Token generation failed");
            }

            return View("RedirectToGateway");
        }

        [HttpPost]
        public async Task<ActionResult> Verify()
        {
            string token = Request.Form["token"];
            string requestId = Request.Form["RequestId"];

            //get transaction from db by token and requestId

            //call IranKish verify api

            //update transaction

            return View("VerifyResult");
        }
    }
}

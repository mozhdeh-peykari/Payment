using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Payment;

namespace Presentation.Controllers;

public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
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
        string requestId = Request.Form["requestId"];
        string acceptorId = Request.Form["acceptorId"];
        string responseCode = Request.Form["responseCode"];
        string amount = Request.Form["amount"];

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token));
        }
        if (string.IsNullOrEmpty(requestId))
        {
            throw new ArgumentNullException(nameof(requestId));
        }

        var request = new VerifyRequest
        {
            Token = token,
            RequestId = requestId,
            AcceptorId = acceptorId,
            ResponseCode = responseCode,
            Amount = amount,
        };
        var result = await _paymentService.Verify(request);

        var dto = new VerifyResultModel
        {
            PaymentStatus = result.PaymentStatus.ToString(),
        };

        return View("VerifyResult", dto);
    }
}

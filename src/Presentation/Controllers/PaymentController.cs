using Application.Dtos;
using Application.Interfaces;
using Domain.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Presentation.Models.Payment;

namespace Presentation.Controllers;

public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly PaymentServiceSettings _settings;

    public PaymentController(IPaymentService paymentService,
        IOptions<PaymentServiceSettings> settings)
    {
        _paymentService = paymentService;
        _settings = settings.Value;
    }

    public ActionResult Index()
    {
        return View(new PaymentInputModel());
    }

    [HttpPost]
    public async Task<ActionResult> StartPayment(PaymentInputModel model)
    {
        //test
        //return View("RedirectToGateway", "981AFD2D2830D3478F357B9CABC8B82D0523");

        if (!ModelState.IsValid)
            return View("Index", model);

        var tokenRequest = new GetTokenRequest
        {
            Amount = model.Amount,
            ReturnUrl = _settings.CallbackUrl
        };

        var token = await _paymentService.GetTokenAsync(tokenRequest);

        if (string.IsNullOrEmpty(token))
        {
            return Content("Token generation failed");
        }

        return View("RedirectToGateway", new RedirectToGatewayModel
        {
            CallbackUrl = _settings.CallbackUrl,
            Token = token
        });
    }

    [HttpPost]
    public async Task<ActionResult> Verify(VerifyRequest model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var result = await _paymentService.Verify(model);

        var dto = new VerifyResultModel
        {
            PaymentStatus = result.PaymentState.ToString(),
        };

        return View("VerifyResult", dto);
    }
}

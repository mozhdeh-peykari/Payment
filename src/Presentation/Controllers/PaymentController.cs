using Application.Dtos;
using Application.Interfaces;
using Domain.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Presentation.Models;
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

        var response = await _paymentService.GetTokenAsync(tokenRequest);

        if (!response.IsSuccessful)
        {
            return View("Error", new ErrorViewModel
            {
                ErrorCode = response.ErrorCode,
                ErrorMessage = response.ErrorMessage
            });
        }

        return View("RedirectToGateway", new RedirectToGatewayModel
        {
            CallbackUrl = _settings.CallbackUrl,
            Token = response.Result
        });
    }

    [HttpPost]
    public async Task<ActionResult> Verify(VerifyRequest model)
    {
        var result = await _paymentService.Verify(model);

        if (!result.IsSuccessful)
        {
            return View("Error", new ErrorViewModel
            {
                ErrorCode = result.ErrorCode,
                ErrorMessage = result.ErrorMessage
            });
        }

        var dto = new VerifyResultModel
        {
            PaymentStatus = result.Result.PaymentState.ToString(),
        };

        return View("VerifyResult", dto);
    }
}

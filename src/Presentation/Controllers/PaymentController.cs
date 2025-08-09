using Application.Dtos;
using Application.Interfaces;
using Application.IPGServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Presentation.Models;
using Presentation.Models.Payment;
using System.Net;

namespace Presentation.Controllers;

public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IranKishIpgServiceSettings _settings;

    public PaymentController(IPaymentService paymentService,
        IOptions<IranKishIpgServiceSettings> settings)
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
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return View("Error", new ErrorViewModel
            {
                ErrorCode = response.ErrorCode,
                ErrorMessage = response.ErrorMessage
            });
        }

        return View("RedirectToGateway", new RedirectToGatewayModel
        {
            RedirctToGatewayUrl = $"{_settings.BaseUrl}{_settings.RedirectToGateway}",
            Token = response.Result
        });
    }

    [HttpPost]
    public async Task<ActionResult> Verify(VerifyRequest model)
    {
        var result = await _paymentService.Verify(model);

        if (!result.IsSuccessful)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
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

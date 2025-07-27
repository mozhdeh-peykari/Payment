using Application;
using Application.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Presentation.Models;

namespace Presentation.ActionFilters;

public class ErrorHandleFilter: IExceptionFilter
{
    private readonly ILogger<ErrorHandleFilter> _logger;
    private readonly IModelMetadataProvider _metadataProvider;

    public ErrorHandleFilter(ILogger<ErrorHandleFilter> logger,
        IModelMetadataProvider metadataProvider)
    {
        _logger = logger;
        _metadataProvider = metadataProvider;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError($"InternalServerError, Exception: {context.Exception.Message}");

        var errorModel = new ErrorViewModel
        {
            ErrorCode = (int)GlobalExceptions.UnexpectedError,
            ErrorMessage = GlobalExceptions.UnexpectedError.GetDescription()
        };

        context.Result = new ViewResult
        {
            ViewName = "Error",
            ViewData = new ViewDataDictionary<ErrorViewModel>(_metadataProvider, context.ModelState)
            {
                Model = errorModel
            }
        };

        // Mark the exception as handled
        context.ExceptionHandled = true;
    }
}

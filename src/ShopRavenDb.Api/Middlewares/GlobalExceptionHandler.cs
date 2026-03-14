using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ShopRavenDb.Api.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> _localizer;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = _localizer["Server error"],
                Detail = _localizer[exception.Message]
            };

            if (exception is ValidationException validationException)
            {
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = _localizer["Validation Error"];
                // Evita mostrar a string técnica do FluentValidation no Detail
                problemDetails.Detail = _localizer["One or more validation errors occurred."];
                
                // Traduz cada mensagem de erro do FluentValidation usando o localizador
                problemDetails.Extensions["errors"] = validationException.Errors.Select(e => new 
                { 
                    e.PropertyName, 
                    ErrorMessage = _localizer[e.ErrorMessage].Value 
                });
            }
            else if (exception is ArgumentException || exception.GetType().Name.Contains("ValidationException"))
            {
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = _localizer["Bad Request"];
                problemDetails.Detail = _localizer[exception.Message].Value;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

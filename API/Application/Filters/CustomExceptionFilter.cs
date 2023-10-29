using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MultiTenantOpenProject.API.Exceptions;
using MultiTenantOpenProject.API.Models;

namespace MultiTenantOpenProject.API.Application.Filters;

public class CustomExceptionFilter : IExceptionFilter
{
    private readonly ILogger<CustomExceptionFilter> _logger;

    public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<CustomExceptionFilter>));
    }

    public void OnException(ExceptionContext context)
    {
        // 304
        if (context.Exception is ResourceNotModifiedException)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
        }
        // 404
        else if (context.Exception is ResourceNotFoundException)
        {
            var problemDetails = new ApplicationProblemDetailsModel(
                StatusCodes.Status404NotFound,
                "The API could not find the resource requested.",
                nameof(ResourceNotFoundException),
                context.HttpContext.Request.Path,
                context.HttpContext.TraceIdentifier,
                new string[] { context.Exception.Message }
            );

            context.Result = new ObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = problemDetails.Status
            };
        }
        // 422
        else if (context.Exception is ResourceAlreadyExistsException)
        {
            var problemDetails = new ApplicationProblemDetailsModel(
                StatusCodes.Status422UnprocessableEntity,
                "The API could not process the requested entity.",
                nameof(ResourceAlreadyExistsException),
                context.HttpContext.Request.Path,
                context.HttpContext.TraceIdentifier,
                new string[] { context.Exception.Message }
            );

            context.Result = new ObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = problemDetails.Status
            };
        }
        // 500
        else if (context.Exception is IdentityException)
        {
            IdentityException? ex = context.Exception as IdentityException;

            var problemDetails = new ApplicationProblemDetailsModel(
                StatusCodes.Status500InternalServerError,
                "The API encountered an identity related error.",
                nameof(IdentityException),
                context.HttpContext.Request.Path,
                context.HttpContext.TraceIdentifier,
                ex?.IdentityErrors.Select(e => e.Description).ToArray()
            );

            context.Result = new ObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = problemDetails.Status
            };
        }
        // 500
        else if (context.Exception is LogicException)
        {
            var problemDetails = new ApplicationProblemDetailsModel(
                StatusCodes.Status500InternalServerError,
                "The API encountered an Logic related error.",
                nameof(LogicException),
                context.HttpContext.Request.Path,
                context.HttpContext.TraceIdentifier,
                new string[] { context.Exception.Message }
            );

            context.Result = new ObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = problemDetails.Status
            };
        }
        // 500
        else
        {
            var problemDetails = new ApplicationProblemDetailsModel(
                StatusCodes.Status500InternalServerError,
                "The API encountered an internal Error.",
                "Uncaught Exception",
                context.HttpContext.Request.Path,
                context.HttpContext.TraceIdentifier,
                new string[] { context.Exception.Message }
            );

            context.Result = new ObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = problemDetails.Status
            };
        }

        context.ExceptionHandled = true;
    }
}

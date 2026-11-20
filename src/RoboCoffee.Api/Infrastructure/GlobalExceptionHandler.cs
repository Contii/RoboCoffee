using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using RoboCoffee.Application.Catalog;
using RoboCoffee.Domain.Exceptions;

namespace RoboCoffee.Api.Infrastructure;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            ProductNotFoundException => StatusCodes.Status404NotFound,
            DomainException => StatusCodes.Status409Conflict,
            ArgumentOutOfRangeException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "An unexpected error occurred");
        }
        else
        {
            logger.LogInformation(
                "Request failed with {StatusCode}: {Message}",
                statusCode,
                exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;

        if (exception is ValidationException validationException)
        {
            var validationDetails = new ValidationProblemDetails(
                validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).Distinct().ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(
                validationDetails,
                cancellationToken);
            return true;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Invalid request",
                StatusCodes.Status404NotFound => "Resource not found",
                StatusCodes.Status409Conflict => "Conflict",
                _ => "An unexpected error occurred"
            },
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "The request could not be completed."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}

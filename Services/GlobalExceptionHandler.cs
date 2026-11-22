using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SjolystInnovation.AppKit.Storage;

namespace pyttogpanne_api.Services
{
    /// <summary>
    /// Maps uncaught exceptions to RFC 7807 ProblemDetails: client-correctable ones to 400 (with the
    /// message), a split query that kept seeing concurrent changes to 503, everything else to a 500
    /// that does not leak internals.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title) = exception switch
            {
                AppKitFileRefusedException => (StatusCodes.Status400BadRequest, exception.Message),
                DbQueryConcurrencyException => (StatusCodes.Status503ServiceUnavailable, "The data changed while it was being read. Try again."),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Unhandled exception");
            else
                _logger.LogWarning("Request rejected: {Message}", exception.Message);

            httpContext.Response.StatusCode = status;
            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails { Status = status, Title = title }
            });
        }
    }
}

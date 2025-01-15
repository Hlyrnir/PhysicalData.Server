using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;

namespace PhysicalData.Api.Endpoint
{
    // https://www.youtube.com/watch?v=eN4GX5WW87s
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionLogger> logException;

        public GlobalExceptionHandler(ILogger<ExceptionLogger> logException)
        {
            this.logException = logException;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exException, CancellationToken tknCancellation)
        {
            switch (exException)
            {
                default:
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            logException.LogError($"An error occurred while processing your request: {exException.Message}");

            Activity? httpActivity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

            ProblemDetail httpProblemDetail = new ProblemDetail()
            {
                Type = exException.GetType().Name,
                Title = "An unexpected error occurred.",
                Detail = exException.Message,
                Status = httpContext.Response.StatusCode,
                Instance = $"{httpContext.Request.Method} - {httpContext.Request.Path}",
                Extensions = new Dictionary<string, object?>
                {
                    { "requestId", httpContext.TraceIdentifier },
                    { "traceId", httpActivity?.Id }
                }
            };

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(httpProblemDetail, tknCancellation);

            return true;
        }
    }
}

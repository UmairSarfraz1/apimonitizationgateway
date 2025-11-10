using ApiMonetizationGateway.Core.IServices;

namespace ApiMonetizationGateway.API.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public RateLimitMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context, IRateLimitingService rateLimitingService)
        {
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKeyValues) ||
           string.IsNullOrEmpty(apiKeyValues.FirstOrDefault()))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "API key is required" });
                return;
            }

            var apiKey = apiKeyValues.First()!;
            var endpoint = context.Request.Path;

            // Checking rate limits
            var rateLimitResult = await rateLimitingService.CheckRateLimitAsync(apiKey, endpoint);

            if (!rateLimitResult.IsAllowed)
            {
                context.Response.StatusCode = rateLimitResult.ErrorCode == 429 ?
                     StatusCodes.Status429TooManyRequests
                    : StatusCodes.Status500InternalServerError;

                if (rateLimitResult.RetryAfterSeconds.HasValue)
                {
                    context.Response.Headers["Retry-After"] = rateLimitResult.RetryAfterSeconds.Value.ToString();
                }

                await context.Response.WriteAsJsonAsync(new { error = rateLimitResult.ErrorMessage });
                return;
            }

            await _requestDelegate(context);

            if (context.Response.StatusCode == 200 )
            {
                await rateLimitingService.RecordUsageAsync(apiKey, endpoint);
            }


        }


    }
}

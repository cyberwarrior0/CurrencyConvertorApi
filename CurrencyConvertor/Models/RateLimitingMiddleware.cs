using System.Collections.Concurrent;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ConcurrentDictionary<string, DateTime> _requestTimes = new ConcurrentDictionary<string, DateTime>();
    private readonly TimeSpan _timeSpan = TimeSpan.FromSeconds(1);

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress.ToString();
        var currentTime = DateTime.UtcNow;

        if (_requestTimes.TryGetValue(clientIp, out DateTime lastRequestTime))
        {
            if (currentTime - lastRequestTime < _timeSpan)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }
        }

        _requestTimes[clientIp] = currentTime;
        await _next(context);
    }
}

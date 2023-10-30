namespace MultiTenantOpenProject.API.Application.Middleware;

public class TraceMiddleware
{
    private readonly RequestDelegate _next;

    public TraceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.OnStarting(state =>
        {
            var httpContext = (HttpContext)state;
            httpContext.Response.Headers.Add("TraceId", context.TraceIdentifier);
            return Task.CompletedTask;
        }, context);

        await _next(context);
    }
}

namespace IDelivery.Api;

public class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
    }
}

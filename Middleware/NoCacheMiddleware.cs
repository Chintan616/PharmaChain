using Microsoft.AspNetCore.Authorization;

namespace PharmaChain.Middleware
{
    public class NoCacheMiddleware
    {
        private readonly RequestDelegate _next;

        public NoCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Set no-cache headers for all authenticated pages and logout responses
            if (context.User.Identity?.IsAuthenticated == true || 
                context.Request.Path.StartsWithSegments("/Account/Logout"))
            {
                context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                context.Response.Headers.Add("Pragma", "no-cache");
                context.Response.Headers.Add("Expires", "0");
            }

            await _next(context);
        }
    }
}

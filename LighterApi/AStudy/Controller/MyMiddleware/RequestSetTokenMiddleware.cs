using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LighterApi
{
    public class RequestSetTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestSetTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var hasToken = httpContext.Request.Query["hasToken"];

            if (!string.IsNullOrWhiteSpace(hasToken)) 
            {
                StringValues token = string.Empty;
                if (!httpContext.Request.Headers.TryGetValue("Authorization", out token))
                    await _next(httpContext);

                httpContext.Items["token"] = WebUtility.HtmlEncode(token.FirstOrDefault());
            }

            await _next(httpContext);
        }
    }
}

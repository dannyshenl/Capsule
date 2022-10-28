using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http;
using System.Threading.Tasks;

namespace XBMall.Server.Image
{
    class CapsuleMiddleware
    {
        private readonly CapsuleDispatch _capsuleDispatch;
        private readonly RequestDelegate _next;
        public CapsuleMiddleware(RequestDelegate next, CapsuleDispatch capsuleDispatch)
        {
            _capsuleDispatch = capsuleDispatch;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Method.Equals(HttpMethod.Options.ToString()))
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
                httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "content-type,x-requested-with,authorization,token");
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                return;
            }
            await _capsuleDispatch.Doing(httpContext);
        }
    }
}

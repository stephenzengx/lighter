﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LighterApi
{
    /// <summary>
    /// 自定义中间件
    /// </summary>
    public class RequestSetTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestSetTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext, IWebHostEnvironment env)
        {
            if (!httpContext.Request.Query.ContainsKey("hasToken"))
                await _next(httpContext);

            StringValues token = new StringValues();
            if (!httpContext.Request.Headers.TryGetValue("Authorization", out token))
                throw new ArgumentNullException();

            httpContext.Items["token"] = WebUtility.HtmlEncode(token.FirstOrDefault());                
            await _next(httpContext);
        }
    }
}

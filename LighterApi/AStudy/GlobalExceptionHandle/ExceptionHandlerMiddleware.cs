using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace LighterApi
{
    //https://andrewlock.net/creating-a-custom-error-handler-middleware-function/
    //https://code-maze.com/global-error-handling-aspnetcore/
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;
        public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IWebHostEnvironment env)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {               
                await HandleExceptionAsync(httpContext, ex, env.IsDevelopment());
                Console.WriteLine(env.EnvironmentName);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception ex, bool IsDevelopment)
        {
            //记日志
            if (!IsDevelopment)
            {
                var _logger = _loggerFactory.CreateLogger<ExceptionHandlerMiddleware>();
                _logger.LogError(ex.Message + "/" + ex.StackTrace + (ex.InnerException != null ? ",InnerException" + ex.InnerException.Message : ""));
            }                

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = IsDevelopment ? "An error occured: " + ex.Message : "An error occured"
            }.ToString());
        }        
    }
}

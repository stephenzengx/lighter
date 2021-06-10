using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LighterApi
{ 
    /// <summary>
    /// 扩展方法必须定义在静态类中
    /// </summary>
    public static class TokenValidateExtensions
    {
        public static IApplicationBuilder UseSetToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidateMiddleware>();
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace LighterApi
{
    /// <summary>
    /// StartupFilter扩展 Startup (有点类似于.net 的消息处理程序)
    /// </summary>
    public class RequestSetOptionsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<RequestSetTokenMiddleware>();
                next(builder);
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LighterApi
{
    //使用扩展方法注册服务组 跟配置结合在一起 to do
    public static class MyConfigServiceCollectionExtensions
    {
        public static IServiceCollection AddConfig(this IServiceCollection  services, IConfiguration config)
        {
            //services.Configure<PositionOptions>(
            //    config.GetSection(PositionOptions.Position));

            return services;
        }
    }
}

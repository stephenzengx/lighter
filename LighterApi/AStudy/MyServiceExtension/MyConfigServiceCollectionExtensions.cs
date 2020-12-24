using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LighterApi
{
    //使用扩展方法注册服务组 
    public static class MyConfigServiceCollectionExtensions
    {
        //合并服务集合
        public static IServiceCollection AddConfig(this IServiceCollection  services, IConfiguration config)
        {
            services.Configure<PositionOptions>(
                config.GetSection(PositionOptions.Position));

            return services;
        }
    }
}

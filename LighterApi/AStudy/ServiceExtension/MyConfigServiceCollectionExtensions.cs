using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LighterApi
{
    //使用扩展方法注册服务组 (合并服务集合) 
    public static class MyConfigServiceCollectionExtensions
    {
        //合并服务集合
        public static IServiceCollection AddConfig(this IServiceCollection  services, IConfiguration config)
        {
            services.Configure<PositionOptions>(
                config.GetSection(PositionOptions.Position));
            //
            services.Configure<TopItemSettings>(TopItemSettings.Month,
                                   config.GetSection("TopItem:Month"));
            services.Configure<TopItemSettings>(TopItemSettings.Year,
                                                config.GetSection("TopItem:Year"));

            return services;
        }
    }
}

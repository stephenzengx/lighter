
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi
{
    public static class ColorConsoleLoggerExtensions
    {
        //默认 LoggerConfiguration 添加
        public static ILoggerFactory AddColorConsoleLogger(
                                  this ILoggerFactory loggerFactory)
        {
            var config = new ColorConsoleLoggerConfiguration();
            return loggerFactory.AddColorConsoleLogger(config);
        }

        //Action委托设置LoggerConfiguration
        public static ILoggerFactory AddColorConsoleLogger(
                                this ILoggerFactory loggerFactory,
                                Action<ColorConsoleLoggerConfiguration> configure)
        {
            var config = new ColorConsoleLoggerConfiguration();

            configure(config);

            return loggerFactory.AddColorConsoleLogger(config);
        }

        //工厂添加 自定义LoggerProvider (LoggerConfiguration new object)
        public static ILoggerFactory AddColorConsoleLogger(
                                          this ILoggerFactory loggerFactory,
                                          ColorConsoleLoggerConfiguration config)
        {
            
            loggerFactory.AddProvider(new ColorConsoleLoggerProvider(config));

            return loggerFactory;
        }
    }
}

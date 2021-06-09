using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.IO;

namespace LighterApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        /*  使用泛型主机 (IHostBuilder) 时，只能将以下服务类型注入 Startup 构造函数：
                IWebHostEnvironment / IHostEnvironment / IConfiguration              
         */
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //配置web服务主机
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    //ASP.NET Core 项目模板默认使用 Kestrel。
                    //若要在调用 ConfigureWebHostDefaults 后提供其他配置，请使用 ConfigureKestrel：
                    //webBuilder.ConfigureKestrel(serverOptions =>
                    //{
                    //   //配置 Kestrel
                    //})
                    webHostBuilder.UseWebRoot("StaticFile");
                    webHostBuilder.UseStartup<Startup>();
                })
                //.UseContentRoot(Directory.GetCurrentDirectory())
                
                //NLog 配置
                .ConfigureLogging(logging =>
                  {
                      logging.ClearProviders();
                      logging.SetMinimumLevel(LogLevel.Trace);
                  })
                .UseNLog()
                

                #region 文件配置提供程序 
                //.ConfigureAppConfiguration((hostingContext, config) =>
                //{
                //    config.Sources.Clear();
                //    var env = hostingContext.HostingEnvironment;

                //    config.AddIniFile("MyIniConfig.ini", optional: true, reloadOnChange: true)
                //            .AddIniFile($"MyIniConfig.{env.EnvironmentName}.ini",
                //                            optional: true, reloadOnChange: true);

                //    config.AddJsonFile("MyIniConfig.ini", optional: true, reloadOnChange: true)
                //                .AddJsonFile($"MyIniConfig.{env.EnvironmentName}.ini",
                //                        optional: true, reloadOnChange: true);

                //    config.AddXmlFile("MyIniConfig.ini", optional: true, reloadOnChange: true)
                //                .AddXmlFile($"MyIniConfig.{env.EnvironmentName}.ini",
                //                        optional: true, reloadOnChange: true);

                //    config.AddEnvironmentVariables();//环境变量

                //    if (args != null)
                //        config.AddCommandLine(args); //命令行
                //})
                #endregion

                //作用域验证
                //.UseDefaultServiceProvider((context, options) => {
                //    options.ValidateScopes = true;
                //})

                //StartupFilter 注册中间件方式1
                //.ConfigureServices(services => 
                //    services.AddTransient<IStartupFilter, RequestSetOptionsStartupFilter>()
                //)
                ;
    };

}


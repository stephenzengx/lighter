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

        /*  ʹ�÷������� (IHostBuilder) ʱ��ֻ�ܽ����·�������ע�� Startup ���캯����
                IWebHostEnvironment / IHostEnvironment / IConfiguration              
         */
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //����web��������
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    //ASP.NET Core ��Ŀģ��Ĭ��ʹ�� Kestrel��
                    //��Ҫ�ڵ��� ConfigureWebHostDefaults ���ṩ�������ã���ʹ�� ConfigureKestrel��
                    //webBuilder.ConfigureKestrel(serverOptions =>
                    //{
                    //   //���� Kestrel
                    //})
                    webHostBuilder.UseWebRoot("StaticFile");
                    webHostBuilder.UseStartup<Startup>();
                })
                //.UseContentRoot(Directory.GetCurrentDirectory())
                
                //NLog ����
                .ConfigureLogging(logging =>
                  {
                      logging.ClearProviders();
                      logging.SetMinimumLevel(LogLevel.Trace);
                  })
                .UseNLog()
                

                #region �ļ������ṩ���� 
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

                //    config.AddEnvironmentVariables();//��������

                //    if (args != null)
                //        config.AddCommandLine(args); //������
                //})
                #endregion

                //��������֤
                //.UseDefaultServiceProvider((context, options) => {
                //    options.ValidateScopes = true;
                //})

                //StartupFilter ע���м����ʽ1
                //.ConfigureServices(services => 
                //    services.AddTransient<IStartupFilter, RequestSetOptionsStartupFilter>()
                //)
                ;
    };

}


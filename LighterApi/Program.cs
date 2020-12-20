using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LighterApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /*  ʹ�÷������� (IHostBuilder) ʱ��ֻ�ܽ����·�������ע�� Startup ���캯����
                IWebHostEnvironment / IHostEnvironment / IConfiguration              
         */
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                //StartupFilter ע���м����ʽ1
                //.ConfigureServices(services => 
                //    services.AddTransient<IStartupFilter, RequestSetOptionsStartupFilter>()
                //)
                ;

        #region �ڲ���������������÷���
        public static IHostBuilder CreateHostBuilder1(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddControllersWithViews();
                    })
                    .Configure(app =>
                    {
                        var loggerFactory = app.ApplicationServices
                            .GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger<Program>();
                        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                        var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

                        logger.LogInformation("Logged in Configure");

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                        }

                        var configValue = config["MyConfigKey"];
                    });
                });
        #endregion

    };

}


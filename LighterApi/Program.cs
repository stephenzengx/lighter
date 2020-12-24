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

                //����web��������
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //ASP.NET Core ��Ŀģ��Ĭ��ʹ�� Kestrel��
                    //��Ҫ�ڵ��� ConfigureWebHostDefaults ���ṩ�������ã���ʹ�� ConfigureKestrel��
                    //webBuilder.ConfigureKestrel(serverOptions =>
                    //{
                    //   //���� Kestrel
                    //})
                    //.UseStartup<Startup>();
                    webBuilder.UseStartup<Startup>();
                })

                //��������֤
                //.UseDefaultServiceProvider((context, options) => {
                //    options.ValidateScopes = true;
                //})

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
                    //config.AddXmlFile("appsettings.xml", optional: true, reloadOnChange: true);
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


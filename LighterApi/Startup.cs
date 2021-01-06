using Lighter.Application;
using Lighter.Application.Contracts;
using LighterApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace LighterApi
{
    public class Startup
    {
        private IConfiguration _configuration { get; }//�����ļ���
        private IWebHostEnvironment _env { get; }
        /*
         ʹ�÷������� (IHostBuilder) ʱ��ֻ�ܽ����·�������ע�� Startup ���캯����
                IWebHostEnvironment: �ṩ�й�����Ӧ�ó����Web�йܻ�������Ϣ��
                IHostEnvironment:�ṩ�й�������������Ӧ�ó����������������Ϣ��
                IConfiguration: ��ʾһ���/ֵӦ�ó����������ԡ�
         */
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;

            Console.WriteLine(_env.ApplicationName);//LighterApi((���򼯵�����)
            Console.WriteLine(_env.EnvironmentName);//Development(������������)
            Console.WriteLine(_env.ContentRootPath);//E:\Test\lighter\LighterApi(Ӧ�ó������ڵ��ļ���)
            Console.WriteLine(_env.WebRootPath);    //E:\Test\lighter\LighterApi\StaticFile ��̬��Դ(Program UseWebRoot ����)  
        }

        //ע����� 
        public void ConfigureServices(IServiceCollection services)
        {
            //AddDbContext /AddDefaultIdentity/AddEntityFrameworkStores /AddRazorPages

            /*1-Startup �½�
            StartupFilter��Startupע���м����ʽ������
            1-StartupFilterע����м����ͨ��Startupע����м����ִ�С�
            2-��������������ͬ��ʽע����м�����ȱ�ע����м����ִ�С�
             */
            // StartupFilter ע���м����ʽ2
            //services.AddSingleton<IStartupFilter, RequestSetOptionsStartupFilter>();

            //2- DI�½� 
            //2.1 ʹ����չ����ע������� 
            // MyConfigServiceCollectionExtensions ��
            //services.AddConfig(_configuration);

            //2.2 ������������
            //services.AddTransient<IOperationTransient, OperationService>();
            //services.AddScoped<IOperationScoped, OperationService>();
            //services.AddSingleton<IOperationSingleton, OperationService>();

            #region ����http����
            //�����÷�
            //services.AddHttpClient(); 

            //�����ͻ���
            //services.AddHttpClient("github", client=> {
            //    client.BaseAddress = new Uri("https://api.github.com/");
            //    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            //    client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            //});

            //���ͻ��ͻ���
            //services.AddTransient<ValidateHeaderHandler>();
            //services.AddHttpClient<RepoService>(c =>
            //{
            //    c.BaseAddress = new Uri("https://api.github.com/");
            //    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            //    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            //})           
            //.AddHttpMessageHandler<ValidateHeaderHandler>() //����HttpMessageHandler
            //.SetHandlerLifetime(TimeSpan.FromMinutes(5)); //���� HttpMessageHandler��������
            #endregion

            services.AddTransient<IQuestionService, QuestionService>()
                .AddTransient<IAnswerService, AnswerService>()
                .AddTransient<IOperation,OperationService>();

            services.AddHttpContextAccessor();//
            //services.AddDbContextPool<LighterDbContext>(options => {
            //    options.UseMySql(Configuration.GetConnectionString("LighterDbContext"));
            //}); �ص������������ޣ�Dbcontext�����˽�б���ʱ��ʹ�óؿ�����Щ�ط�Ҫ����ע��

            /*�ӳټ��� Microsoft.EntityFrameworkCore.Proxies ��
             .AddDbContext<BloggingContext>(
                b => b.UseLazyLoadingProxies()
                      .UseSqlServer(myConnectionString));
             */
                    
            //ע�� dbcontext
            services.AddDbContext<LighterDbContext>(options=> {                
                options.UseMySql(_configuration.GetConnectionString("LighterDbContext"));
            });

            //ע��mongo
            services.AddSingleton<IMongoClient>(sp=> {
                return new MongoClient(_configuration.GetConnectionString("LighterMongoServer"));
            });

            services.AddControllers()
                .AddNewtonsoftJson(x=>x.SerializerSettings.ReferenceLoopHandling=ReferenceLoopHandling.Ignore);
            //ע�� mvc��Ҫ����ط��� / newtonsoft ��չ��  / ��Project ProjectGroup�� ѭ����������
        }

        /// <summary>
        /// Configure ��������ָ��Ӧ����Ӧ HTTP ����ķ�ʽ
        /// </summary>
        /// <param name="app">IApplicationBuilder:�ṩ����������Ӧ������ܵ�����</param>
        public void Configure(IApplicationBuilder app)//, ILoggerFactory loggerFactory
        {
            #region �Զ���logprovider / logger
            ////logging���� ����Զ���provider (��ʹ����չ��ʽ)
            //loggerFactory.AddProvider(new ColorConsoleLoggerProvider(
            //                          new ColorConsoleLoggerConfiguration
            //                          {
            //                              LogLevel = LogLevel.Error,
            //                              Color = ConsoleColor.Red
            //                          }));

            ////default LogLevel.Warning; ConsoleColor.Yellow
            //loggerFactory.AddColorConsoleLogger();

            ////new object
            //loggerFactory.AddColorConsoleLogger(new ColorConsoleLoggerConfiguration
            //{
            //    LogLevel = LogLevel.Debug,
            //    Color = ConsoleColor.Gray
            //});

            ////ί�з�ʽ
            //loggerFactory.AddColorConsoleLogger(c =>
            //{
            //    c.LogLevel = LogLevel.Information;
            //    c.Color = ConsoleColor.Blue;
            //});
            #endregion

            #region ��� �м���ܵ�ִ��˳�� 
            //app.Use(async (context, next) =>
            //{        

            //    await context.Response.WriteAsync("mw1 hello world!\r\n");               
            //    await next.Invoke();
            //    await context.Response.WriteAsync("mw1 hello world!\r\n");
            //    /*InvalidOperationException: Headers are read-only, response has already started*/
            //    context.Response.Headers["test"] = "test value";
            //});

            ////����ܵ���·,���羲̬�ļ��м��
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("mw2 hello world!\r\n");
            //    StatusCode cannot be set because the response has already started.
            //    await next.Invoke(); //��Ӧ��ʼ�� next������м���Ͳ������޸�״̬����
            //});

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("middleware end!\r\n");
            //});
            #endregion

            #region �����м�� 
            //1 - Configure���� IApplicationBuilder app��������
            //    provides the mechanisms to configure an application's request pipeline
            //2 - Configure���� �����м��
            //      UseDeveloperExceptionPage: ������Ա�쳣ҳ(�м��)
            //      UseExceptionHandler: �쳣�������
            //      UseHsts��HTTP �ϸ��䰲ȫ��(HSTS)  < ���� Strict - Transport - Security  ��ͷ >
            //      UseHttpsRedirection: https �ض��� ==��UseStaticFiles: ��̬�ļ� == ��
            //      UseRouting��·���м�� == �� UseCors: ���� ==��
            //      Authentication: ��֤ / UseAuthorization: ��Ȩ,
            //      Custom Middlewares �Զ����м�� == �� Endpoint���ն��м��

            #endregion
            
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler
            }
            else
            {
                app.UseHsts();//��� Strict-Transport-Security ��ͷ
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();//For the wwwroot folder          
            app.UseStaticFiles(new StaticFileOptions //�������ľ�̬�ļ�·��
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @_configuration["StaticFilePath"].Substring(1))
                ),
                RequestPath = _configuration["StaticFilePath"]
            });

            app.UseGlobalExceptionHandler();

            #region Nlog���ñ���            
            //NLog.LogManager.Configuration.Variables["connectionString"] = _configuration["ConnectionStrings:LighterDbContext"];
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //������־�е������������
            #endregion

            //app.UseSetToken();//�Զ����м��

            app.UseRouting();//·���м��  Matches request to an endpoint.      
            app.UseEndpoints(endpoints =>
            {
                //Namespace: ControllerEndpointRouteBuilderExtensions
                endpoints.MapControllers();

                //endpoints.MapHealthChecks("/healthz").RequireAuthorization();

                //Namespace: EndpointRouteBuilderExtensions
                //endpoints.MapGet("/", async context => 
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }
    }
}

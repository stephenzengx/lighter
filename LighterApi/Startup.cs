using Lighter.Application;
using Lighter.Application.Contracts;
using LighterApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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
            services.AddConfig(_configuration);

            //2.2 ������������
            services.AddTransient<IOperationTransient, OperationService>();
            services.AddScoped<IOperationScoped, OperationService>();
            services.AddSingleton<IOperationSingleton, OperationService>();

            services.AddScoped<IQuestionService, QuestionService>()
                .AddScoped<IAnswerService, AnswerService>()
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
        public void Configure(IApplicationBuilder app)
        {
            #region �м��ԭ��demo
            //app.Use(async (context, next) =>
            //{
            //    var varia1 = _env.ApplicationName;//LighterApi((���򼯵�����)
            //    var varia2 = _env.EnvironmentName;//Development(������������)
            //    var varia3 = _env.ContentRootPath;//E:\Test\lighter\LighterApi(Ӧ�ó������ڵ��ļ���)
            //    var varia4 = _env.WebRootPath;               
            //    //_env.WebRootFileProvider

            //    await context.Response.WriteAsync("mw1 hello world!\r\n");               
            //    await next.Invoke();
            //    /*InvalidOperationException: Headers are read-only, response has already started*/
            //    context.Response.Headers["test"] = "test value";
            //});

            ////����ܵ���·,���羲̬�ļ��м��
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("mw2 hello world!\r\n");
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
            }
            else
            {
                app.UseExceptionHandler();
                app.UseHsts();//��� Strict-Transport-Security ��ͷ
            }

            //app.UseSetToken();//�Զ����м��

            app.UseRouting();//·���м��           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }
    }
}

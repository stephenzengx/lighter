using Lighter.Application;
using Lighter.Application.Contracts;
using LighterApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Newtonsoft.Json;

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

            /*
            StartupFilter��Startupע���м����ʽ������
            1-StartupFilterע����м����ͨ��Startupע����м����ִ�С�
            2-��������������ͬ��ʽע����м�����ȱ�ע����м����ִ�С�
             */
            // StartupFilter ע���м����ʽ2
            services.AddSingleton<IStartupFilter, RequestSetOptionsStartupFilter>();

            services.AddScoped<IQuestionService, QuestionService>()
                .AddScoped<IAnswerService, AnswerService>()
                .AddTransient<ITestService,TestService>();

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
            /*  1-Configure���� IApplicationBuilder app��������
                provides the mechanisms to configure an application's request pipeline
                2-Configure���� �����м��   
                UseDeveloperExceptionPage: ������Ա�쳣ҳ(�м��)
                UseExceptionHandler:    �쳣�������
                UseHsts��HTTP �ϸ��䰲ȫ��(HSTS) ����
                UseHttpsRedirection: https �ض���
                UseMvc()/UseMvcWithDefaultRoute(): MVC
                Razor Pages: Razorҳ��

                UseStaticFiles: ��̬�ļ�
                UseRouting��·���м��
                UseAuthorization: ��Ȩ
             */


            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

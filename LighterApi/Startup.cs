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
        public IConfiguration Configuration { get; }//�����ļ���

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //ע�����
        public void ConfigureServices(IServiceCollection services)
        {
            //ע�����
            services.AddScoped<IQuestionService, QuestionService>()
                .AddScoped<IAnswerService, AnswerService>();

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
                options.UseMySql(Configuration.GetConnectionString("LighterDbContext"));
            });
            //ע��mongo
            services.AddSingleton<IMongoClient>(sp=> {
                return new MongoClient(Configuration.GetConnectionString("LighterMongoServer"));
            });

            services.AddControllers()
                .AddNewtonsoftJson(x=>x.SerializerSettings.ReferenceLoopHandling=ReferenceLoopHandling.Ignore);
            //ע�� mvc��Ҫ����ط��� / newtonsoft ��չ��  / ��Project ProjectGroup�� ѭ����������
        }

        //���ùܵ��м��
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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

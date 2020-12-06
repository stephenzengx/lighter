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
        public IConfiguration Configuration { get; }//配置文件类

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //注入服务
        public void ConfigureServices(IServiceCollection services)
        {
            //注入服务
            services.AddScoped<IQuestionService, QuestionService>()
                .AddScoped<IAnswerService, AnswerService>();

            services.AddHttpContextAccessor();//
            //services.AddDbContextPool<LighterDbContext>(options => {
            //    options.UseMySql(Configuration.GetConnectionString("LighterDbContext"));
            //}); 池的性能提升有限，Dbcontext如果有私有变量时，使用池可能有些地方要额外注意

            /*延迟加载 Microsoft.EntityFrameworkCore.Proxies 包
             .AddDbContext<BloggingContext>(
                b => b.UseLazyLoadingProxies()
                      .UseSqlServer(myConnectionString));
             */
            //注入 dbcontext
            services.AddDbContext<LighterDbContext>(options=> {                
                options.UseMySql(Configuration.GetConnectionString("LighterDbContext"));
            });
            //注入mongo
            services.AddSingleton<IMongoClient>(sp=> {
                return new MongoClient(Configuration.GetConnectionString("LighterMongoServer"));
            });

            services.AddControllers()
                .AddNewtonsoftJson(x=>x.SerializerSettings.ReferenceLoopHandling=ReferenceLoopHandling.Ignore);
            //注入 mvc需要的相关服务 / newtonsoft 扩展包  / （Project ProjectGroup） 循环引用问题
        }

        //配置管道中间件
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();//路由中间件

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

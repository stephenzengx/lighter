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
        private IConfiguration _configuration { get; }//配置文件类
        private IWebHostEnvironment _env { get; }
        /*
         使用泛型主机 (IHostBuilder) 时，只能将以下服务类型注入 Startup 构造函数：
                IWebHostEnvironment: 提供有关运行应用程序的Web托管环境的信息。
                IHostEnvironment:提供有关其中正在运行应用程序的宿主环境的信息。
                IConfiguration: 表示一组键/值应用程序配置属性。
         */
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        //注入服务 
        public void ConfigureServices(IServiceCollection services)
        {
            //AddDbContext /AddDefaultIdentity/AddEntityFrameworkStores /AddRazorPages

            /*
            StartupFilter和Startup注册中间件方式的区别
            1-StartupFilter注册的中间件比通过Startup注册的中间件先执行。
            2-对于两个采用相同方式注册的中间件，先被注册的中间会先执行。
             */
            // StartupFilter 注册中间件方式2
            services.AddSingleton<IStartupFilter, RequestSetOptionsStartupFilter>();

            services.AddScoped<IQuestionService, QuestionService>()
                .AddScoped<IAnswerService, AnswerService>()
                .AddTransient<ITestService,TestService>();

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
                options.UseMySql(_configuration.GetConnectionString("LighterDbContext"));
            });

            //注入mongo
            services.AddSingleton<IMongoClient>(sp=> {
                return new MongoClient(_configuration.GetConnectionString("LighterMongoServer"));
            });

            services.AddControllers()
                .AddNewtonsoftJson(x=>x.SerializerSettings.ReferenceLoopHandling=ReferenceLoopHandling.Ignore);
            //注入 mvc需要的相关服务 / newtonsoft 扩展包  / （Project ProjectGroup） 循环引用问题
        }

        /// <summary>
        /// Configure 方法用于指定应用响应 HTTP 请求的方式
        /// </summary>
        /// <param name="app">IApplicationBuilder:提供了用于配置应用请求管道机制</param>
        public void Configure(IApplicationBuilder app)
        {
            /*  1-Configure方法 IApplicationBuilder app参数解释
                provides the mechanisms to configure an application's request pipeline
                2-Configure方法 常用中间件   
                UseDeveloperExceptionPage: 开发人员异常页(中间件)
                UseExceptionHandler:    异常处理程序
                UseHsts：HTTP 严格传输安全性(HSTS) ？？
                UseHttpsRedirection: https 重定向
                UseMvc()/UseMvcWithDefaultRoute(): MVC
                Razor Pages: Razor页面

                UseStaticFiles: 静态文件
                UseRouting：路由中间件
                UseAuthorization: 授权
             */


            if (_env.IsDevelopment())
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

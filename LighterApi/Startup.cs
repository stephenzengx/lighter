using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using MongoDB.Driver;
using Lighter.Application;
using Lighter.Application.Contracts;
using LighterApi.Data;

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

            //Console.WriteLine(_env.ApplicationName);//LighterApi((程序集的名称)
            //Console.WriteLine(_env.EnvironmentName);//Development(环境变量名称)
            //Console.WriteLine(_env.ContentRootPath);//E:\Test\lighter\LighterApi(应用程序集所在的文件夹)
            //Console.WriteLine(_env.WebRootPath);    //E:\Test\lighter\LighterApi\StaticFile 静态资源(Program UseWebRoot 设置)  
        }

        //注入服务 
        public void ConfigureServices(IServiceCollection services)
        {
            //AddDbContext /AddDefaultIdentity/AddEntityFrameworkStores /AddRazorPages

            /*1-Startup 章节
            StartupFilter和Startup注册中间件方式的区别
            1-StartupFilter注册的中间件比通过Startup注册的中间件先执行。
            2-对于两个采用相同方式注册的中间件，先被注册的中间会先执行。
             */
            // StartupFilter 注册中间件方式2
            //services.AddSingleton<IStartupFilter, RequestSetOptionsStartupFilter>();

            //2- DI章节 
            //2.1 使用扩展方法注册服务组 
            // MyConfigServiceCollectionExtensions 类
            //services.AddConfig(_configuration);

            //2.2 测试生命周期
            //services.AddTransient<IOperationTransient, OperationService>();
            //services.AddScoped<IOperationScoped, OperationService>();
            //services.AddSingleton<IOperationSingleton, OperationService>();

            services.AddTransient<IQuestionService, QuestionService>()
                .AddTransient<IAnswerService, AnswerService>()
                .AddTransient<IOperation,OperationService>();

            services.AddHttpContextAccessor(); //???

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
        public void Configure(IApplicationBuilder app)//, ILoggerFactory loggerFactory
        {
            #region 理解 中间件管道执行顺序 
            //app.Use(async (context, next) =>
            //{        

            //    await context.Response.WriteAsync("mw1 hello world!\r\n");               
            //    await next.Invoke();
            //    await context.Response.WriteAsync("mw1 hello world!\r\n");
            //    /*InvalidOperationException: Headers are read-only, response has already started*/
            //    context.Response.Headers["test"] = "test value";
            //});

            ////请求管道短路,例如静态文件中间件
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("mw2 hello world!\r\n");
            //    StatusCode cannot be set because the response has already started.
            //    await next.Invoke(); //响应开始后 next后面的中间件就不能再修改状态码了
            //});

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("middleware end!\r\n");
            //});
            #endregion

            #region 常用中间件 
            //1 - Configure方法 IApplicationBuilder app参数解释
            //    provides the mechanisms to configure an application's request pipeline
            //2 - Configure方法 常用中间件
            //      UseDeveloperExceptionPage: 开发人员异常页(中间件)
            //      UseExceptionHandler: 异常处理程序
            //      UseHsts：HTTP 严格传输安全性(HSTS)  < 加上 Strict - Transport - Security  标头 >
            //      UseHttpsRedirection: https 重定向 ==》UseStaticFiles: 静态文件 == 》
            //      UseRouting：路由中间件 == 》 UseCors: 跨域 ==》
            //      Authentication: 认证 / UseAuthorization: 授权,
            //      Custom Middlewares 自定义中间件 == 》 Endpoint：终端中间件

            #endregion
            
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler
            }
            else
            {
                app.UseHsts();//添加 Strict-Transport-Security 标头
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();//For the wwwroot folder          
            //app.UseStaticFiles(new StaticFileOptions //添加其余的静态文件路径
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(Directory.GetCurrentDirectory(), @_configuration["StaticFilePath"].Substring(1))
            //    ),
            //    RequestPath = _configuration["StaticFilePath"]
            //});

            app.UseGlobalExceptionHandler();

            #region Nlog设置变量            
            NLog.LogManager.Configuration.Variables["connectionString"] = _configuration["ConnectionStrings:LighterDbContext"];
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //避免日志中的中文输出乱码
            #endregion

            //app.UseTokenValidate();//自定义中间件

            app.UseRouting();//路由中间件  Matches request to an endpoint.

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

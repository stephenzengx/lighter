using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lighter.Application.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LighterApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class Study1Controller : ControllerBase
    {
        private IServiceProvider _serviceProvider;
        private ITestService  _testService;

        public Study1Controller(IServiceProvider serviceProvider,ITestService testService)
        {
            _serviceProvider = serviceProvider;
            //1- 通过依赖注入的方式 获取服务
            //_testService = testService;
            //2-(服务查找)通过serviceProvider 获取服务  （需要引用 Microsoft.Extensions.DependencyInjection)

            //GetService() 服务不存在,返回null
            //GetRequiredService() 服务不存在 抛异常
            _testService = _serviceProvider.GetService<TestService>();
            //_testService = _serviceProvider.GetRequiredService<TestService>();            
        }

        //https://localhost:6001/api/study1/test1?hasToken=Hello
        [Route("test1")]
        public IActionResult Test1()
        {
            var token = HttpContext.Items["token"].ToString();

            return Ok(token);
        }

        [Route("test2")]
        public IActionResult Test2()
        {

            return Ok("success");
        }

        [Route("test3")]
        public IActionResult Test3()
        {

            return Ok("success");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lighter.Application.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LighterApi.AStudy.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class Study1Controller : ControllerBase
    {
        private IServiceProvider _serviceProvider;
        private IAnswerService _answerService;

        public Study1Controller(IServiceProvider serviceProvider,IAnswerService answerService)
        {
            _serviceProvider = serviceProvider;
            //1- 通过依赖注入的方式 获取服务
            //_answerService = answerService;
            //2-通过serviceProvider 获取服务  （需要引用 Microsoft.Extensions.DependencyInjection)

            //GetService() 服务不存在,返回null
            //GetRequiredService() 服务不存在 抛异常
            _answerService = _serviceProvider.GetService<IAnswerService>();
            //_answerService = _serviceProvider.GetRequiredService<IAnswerService>();            
        }

        public IActionResult ProviderTest()
        {
           
            return Ok("success");
        }
    }
}

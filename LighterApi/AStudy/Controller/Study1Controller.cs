﻿using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LighterApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class Study1Controller : ControllerBase
    {
        //1-Startup
        private IServiceProvider _serviceProvider;
        private IOperation _operationService;

        private ILogger<Study1Controller> _logger;

        public Study1Controller(IServiceProvider serviceProvider, IOperation operationService, ILogger<Study1Controller> logger)
        {
            _serviceProvider = serviceProvider;
            //1- 通过依赖注入的方式 获取服务
            _operationService = operationService;
            //2- (服务查找)通过serviceProvider 获取服务  （需要引用 Microsoft.Extensions.DependencyInjection)
            // GetService() 服务不存在,返回null  /  GetRequiredService() 服务不存在 抛异常
            //_operationService = _serviceProvider.GetService<OperationService>();
            //_operationService = _serviceProvider.GetRequiredService<OperationService>();

            _logger = logger;
        }

        #region 验证服务生命周期
        //private readonly ILogger _logger;
        //private readonly IOperationSingleton _singletonOperation;
        //private readonly IOperationScoped _scopedOperation;
        //private readonly IOperationScoped _scopedOperationOne;
        //private readonly IOperationTransient _transientOperation;
        //private readonly IOperationTransient _transientOperationOne;

        //public Study1Controller(ILogger<Study1Controller> logger,
        //          IOperationTransient transientOperation,
        //          IOperationTransient transientOperationOne,
        //          IOperationScoped scopedOperation,
        //          IOperationScoped scopedOperationOne,
        //          IOperationSingleton singletonOperation)
        //{
        //    _logger = logger;
        //    _transientOperation = transientOperation;
        //    _transientOperationOne = transientOperationOne;

        //    _scopedOperation = scopedOperation;
        //    _scopedOperationOne = scopedOperationOne;

        //    _singletonOperation = singletonOperation;
        //}

        //[Route("serviceLifetimes")]
        //public IActionResult Test2()
        //{
        //    _logger.LogInformation("Singleton: " + _singletonOperation.OperationId);

        //    _logger.LogInformation("Scoped: " + _scopedOperation.OperationId);
        //    _logger.LogInformation("ScopedOne: " + _scopedOperationOne.OperationId);

        //    _logger.LogInformation("Transient: " + _transientOperation.OperationId);
        //    _logger.LogInformation("TransientOne: " + _transientOperationOne.OperationId);

        //    return Ok("success");
        //}
        #endregion

        //https://localhost:6001/api/study1/test1?hasToken=Hello
        [Route("IStartupFilter")]
        public IActionResult Test1()
        {
            var token = HttpContext.Items["token"].ToString();

            return Ok(token);
        }

        [Route("settoken")]
        [HttpGet]
        public IActionResult Test3()
        {
            var token = HttpContext.Items["token"].ToString();

            return Ok(token);
        }

        [Route("~/errortest/{city}")]//重写路由url
        [HttpGet]
        public IActionResult ErrorTest(string city)
        {
            if (!string.Equals(city?.TrimEnd(), "Redmond", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"We don't offer a weather forecast for {city}.", nameof(city));
            }

            return Ok("ErrorTest !");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace LighterApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogger _logger;
        //private readonly ILogger _myLogger;
        public LogController(ILogger<LogController> logger)//ILoggerFactory loggerFactory
        {
            //日志的创建方式 直接注入/注入日志工厂类 再创建
            _logger = logger;
            //_myLogger = loggerFactory.CreateLogger("mylog");
            //loggerFactory.CreateLogger<WeatherController>();
        }

        [HttpGet]
        [Route("logtest1")]
        public IActionResult Logtest1()
        {
            //_logger.LogTrace("LogTrace");
            //_logger.LogDebug("LogDebug");
            //_logger.LogInformation(new EventId(1001, "Log"), "Karl Log");
            //_logger.LogWarning("LogWarning");
            //_logger.LogError("LogError");
            //_logger.LogCritical("LogCritical");
            return Ok($"Test Log: {DateTime.Now}");
        }

        [HttpGet]
        [Route("logtest2")]
        public IActionResult Logtest2([FromQuery]string id)
        {
            return Ok();
            //var routeInfo = ControllerContext.ToCtxString(id); _logger.LogInformation(1000, routeInfo);
            //return ControllerContext.MyDisplayRouteInfo();
        }

        [Route("nlog")]
        [HttpGet]
        public IActionResult Test4()
        {
            //构造函数 直接注入  NLog._logger 报错
            //NLog.LogEventInfo errorEvent = new NLog.LogEventInfo(NLog.LogLevel.Error, null, "Pass my custom value");
            //_logger.Log(errorEvent);

            var ex = new Exception("Exception Test");
            ex.Data.Add("key1","value1");
            //params object[] args 这个参数要怎么用？？？
            _logger.LogError(ex, "exception message");

            return Ok();
        }
    }
}

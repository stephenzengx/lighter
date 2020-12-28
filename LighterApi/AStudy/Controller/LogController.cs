using Microsoft.AspNetCore.Mvc;
using Microsoft.Docs.Samples;
using Microsoft.Extensions.Logging;
using System;

namespace LighterApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;
        private readonly ILogger _myLogger;
        public LogController(ILogger<LogController> logger, ILoggerFactory loggerFactory)
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
            _logger.LogTrace("LogTrace");
            _logger.LogDebug("LogDebug");
            _logger.LogInformation(new EventId(1001, "Log"), "Karl Log");
            _logger.LogWarning("LogWarning");
            _logger.LogError("LogError");
            _logger.LogCritical("LogCritical");

            return Ok($"Test Log: {DateTime.Now}");
        }

        [HttpGet]
        [Route("logtest2")]
        public IActionResult Logtest2([FromQuery]string id)
        {
            var routeInfo = ControllerContext.ToCtxString(id); _logger.LogInformation(1000, routeInfo);
            return ControllerContext.MyDisplayRouteInfo();
        }
    }
}

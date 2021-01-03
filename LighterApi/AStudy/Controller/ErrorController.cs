using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private ILogger<ErrorController> _logger;
        private IWebHostEnvironment _env;

        public ErrorController(ILogger<ErrorController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _env = webHostEnvironment;
        }

        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment()//[FromServices] IWebHostEnvironment webHostEnvironment
        {
            if (_env.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [Route("/error")]
        public IActionResult ErrorProcess()
        {
            return Problem();
        }
    }
}

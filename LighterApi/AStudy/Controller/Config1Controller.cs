using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LighterApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class Config1Controller : ControllerBase
    {
        private IConfigurationRoot _configRoot;
        private IConfiguration _config;

        private PositionOptions  _positionOptions;

        //public Config1Controller(IConfiguration config)
        //{
        //    _config = config;
        //    _configRoot = (IConfigurationRoot)config;
        //}

        /// <summary>
        /// Microsoft.Extensions.Options.OptionsManager <- IOptions<T>
        /// </summary>
        /// <param name="positionOptions"></param>
        public Config1Controller(IOptions<PositionOptions> positionOptions)
        {
            _positionOptions = positionOptions.Value;
        }

        /// <summary>
        /// config load sequence
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sequence")]
        public IActionResult Sequence()
        {
            /*
            Microsoft.Extensions.Configuration.ChainedConfigurationProvider
            JsonConfigurationProvider for 'appsettings.json' (Optional)
            JsonConfigurationProvider for 'appsettings.Development.json' (Optional)
            EnvironmentVariablesConfigurationProvider
            CommandLineConfigurationProvider
             */

            string str = "";
            foreach (var provider in _configRoot.Providers.ToList())
            {
                str += provider.ToString() + "\n";
            }

            return Content(str);
        }

        [HttpGet]
        [Route("PositionBindGet")]
        public IActionResult PositionBindGet([FromQuery]string kind)
        {           
            if (kind.Equals("bind"))
            {
                PositionOptions _positionOptions = new PositionOptions();//要new一个对象
                _config.GetSection(PositionOptions.Position).Bind(_positionOptions);
                return Content(JsonConvert.SerializeObject(_positionOptions));
            }
            else
            {
                _positionOptions = _config.GetSection(PositionOptions.Position).Get<PositionOptions>();
                return Content(JsonConvert.SerializeObject(_positionOptions));
            }                           
        }
    }
}

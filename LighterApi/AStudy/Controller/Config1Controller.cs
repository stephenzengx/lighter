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

        private IOptionsMonitor<PositionOptions> _options;

        private readonly TopItemSettings _monthTopItem;
        private readonly TopItemSettings _yearTopItem;

        //public Config1Controller(IConfiguration config)
        //{
        //    _config = config;
        //    _configRoot = (IConfigurationRoot)config;
        //}

        /// <summary>
        /// Microsoft.Extensions.Options.OptionsManager <- IOptions<T>
        /// </summary>
        /// <param name="positionOptions"></param>
        //public Config1Controller(IOptions<PositionOptions> positionOptions)
        //{
        //    _positionOptions = positionOptions.Value;
        //}

        //public Config1Controller(IOptionsSnapshot<PositionOptions> positionOptions)
        //{
        //    _positionOptions = positionOptions.Value;
        //}

        //
        public Config1Controller(IOptionsSnapshot<TopItemSettings> namedOptionsAccessor)
        {
            _monthTopItem = namedOptionsAccessor.Get(TopItemSettings.Month);
            _yearTopItem = namedOptionsAccessor.Get(TopItemSettings.Year);
        }

        //public Config1Controller(IOptionsMonitor<PositionOptions> options)
        //{
        //    _options = options;
        //}

        /// <summary>
        /// config load sequence
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sequence")]
        public IActionResult Sequence()
        {
            /*  Microsoft.Extensions.Configuration.ChainedConfigurationProvider
                JsonConfigurationProvider for 'appsettings.json' (Optional)
                JsonConfigurationProvider for 'appsettings.Development.json' (Optional)
                EnvironmentVariablesConfigurationProvider
                CommandLineConfigurationProvider  */

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

        [HttpGet]
        [Route("snapshot")]
        public IActionResult Snapshot()
        {
            Console.WriteLine(_positionOptions.Name,_positionOptions.Title);
            return Ok();
        }

        [HttpGet]
        [Route("monitor")]
        public IActionResult Monitor()
        {
            _options.OnChange(Options=> Console.WriteLine(_options.CurrentValue.Title+" / " + _options.CurrentValue.Name));
            return Ok();
        }

        [HttpGet]
        [Route("IConfigureNamedOptions")]
        public IActionResult OnGet()
        {
            return Content($"Month:Name {_monthTopItem.Name} \n" +
                           $"Month:Model {_monthTopItem.Model} \n\n" +
                           $"Year:Name {_yearTopItem.Name} \n" +
                           $"Year:Model {_yearTopItem.Model} \n");
        }
    }
}

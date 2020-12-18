using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.AStudy.Controller
{
    public class TestService : ITest
    {
        private string _guid = string.Empty;

        public TestService()
        {
            _guid = Guid.NewGuid().ToString();
        }

        public string GetGuid()
        {
            return _guid;
        }
    }
}

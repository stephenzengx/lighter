using System;

namespace LighterApi
{
    public class TestService : ITestService
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

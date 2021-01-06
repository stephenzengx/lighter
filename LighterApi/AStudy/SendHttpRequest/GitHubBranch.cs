using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi
{
    public class GitHubBranch
    {
        public string name { get; set; }

        public Commit commit { get; set; }

        public bool @protected { get; set; }
    }

    public class Commit
    {
        public string sha { get; set; }

        public string url { get; set; }
    }
}

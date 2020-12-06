using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lighter.Domain.Project
{
    public class Member : Entity
    {
        public string Progress { get; set; }
        public string ProjectId { get; set; }
        public bool IsAssitant { get; set; } //是否是助教
        public string GroupId { get; set; }
        public ProjectGroup Group { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data
{
    public class SubjectProject :Entity
    {
        public string SubjectId { get; set; }
        public Subject Subject  { get; set; }   
        public string ProjectId { get; set; }
        public Project Project { get; set; }

    }
}

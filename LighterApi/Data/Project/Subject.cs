using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data
{
    public class Subject : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<SubjectProject> SubjectProjects { get; set; }
    }
}

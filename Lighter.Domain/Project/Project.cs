using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lighter.Domain.Project
{
    public class Project : Entity
    {
        public string Title { get; set; }
        public string Supervisor { get; set; } //负责人
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ProjectGroup> Groups { get; set; }
        public List<SubjectProject> SubjectProjects { get; set; }
    }
}

using System.Collections.Generic;

namespace LighterApi.Data
{
    public class ProjectGroup :Entity
    {
        public string Name { get; set; }//组名
        public string ProjectId { get; set; }//项目id
        public Project Project { get; set; }
        public List<Member> Mebers { get; set; }//成员
    }
}

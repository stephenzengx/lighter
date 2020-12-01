using LighterApi.Share;

namespace LighterApi.Data
{
    public class Task :Entity
    {
        public string Title { get; set; }
        //public string SectionId { get; set; }
        public string Description { get; set; }
        public string ProjectId { get; set; }
        public Project Project { get; set; }
        public string MemberId { get; set; }
        public Member Member { get; set; }
        public EnumTaskStatus Status { get; set; }
        public string Tags { get; set; }
    }
}

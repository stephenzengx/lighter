using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lighter.Domain.Question
{
    public class Question: Entity
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        //public EnumQuestionStatus Status { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Content { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public int ViewCount { get; set; }

        public int VoteCount { get; set; }
        public List<string> VoteUps { get; set; } = new List<string>(); //向上投票记录id
        public List<string> VoteDowns { get; set; } = new List<string>();  //向下投票记录id

        public List<string> Answers { get; set; } = new List<string>();
    }
}

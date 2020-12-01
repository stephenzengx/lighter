using LighterApi.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data
{
    public class Vote : Entity
    {
        public string SourceType { get; set; }//Question | Answer
        public string SourceId { get; set; } //QuestionId | AnswerId
        public EnumVoteDirection Direction { get; set; }
    }
}

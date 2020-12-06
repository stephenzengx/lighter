using Lighter.Domain.Share;

namespace Lighter.Domain.Question
{
    public class Vote : Entity
    {
        public string SourceType { get; set; }//Question | Answer
        public string SourceId { get; set; } //QuestionId | AnswerId
        public EnumVoteDirection Direction { get; set; }
    }
}

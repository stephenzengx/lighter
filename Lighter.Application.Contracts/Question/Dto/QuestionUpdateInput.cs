using System.Collections.Generic;

namespace Lighter.Application.Contracts.Dto
{
    public class QuestionUpdateInput
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }

        public string Summary { get; set; }
    }
}

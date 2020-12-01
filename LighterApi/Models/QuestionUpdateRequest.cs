using LighterApi.Data;
using System.Collections.Generic;
namespace LighterApi.Models
{
    public class QuestionUpdateRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }
        public string Summary { get; set; }

    }
}

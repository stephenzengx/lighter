﻿using Lighter.Domain.Question;
using System.Collections.Generic;

namespace Lighter.Application.Contracts.Dto
{
    public class QuestionAnswerInput //:Question
    {
        public Question Question { get; set; }
        public IEnumerable<Answer> AnswerList { get; set; }
    }
}

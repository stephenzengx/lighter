﻿using System;
using System.Collections.Generic;
namespace LighterApi.Data
{
    public class Answer :Entity
    {
        public string QuestionId { get; set; }
        public string Content { get; set; }

        public int VoteCount { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<string> VoteUpRecIds { get; set; } = new List<string>(); //向上投票记录id
        public List<string> VoteDownRecIds { get; set; } = new List<string>(); //向下投票记录id
    }
}

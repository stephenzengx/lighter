using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lighter.Domain.Question
{
    public class Comment
    {
        public string Content { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateAt { get; set; }  
        //后续可能会需要用户name
    }
}

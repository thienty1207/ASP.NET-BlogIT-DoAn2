using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogIT.Models
{
    public class TopPostViewModel
    {
        public BlogPost Post { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogIT.Models
{
    public class ReportsViewModel
    {
        public IEnumerable<ReportBlog> BlogReports { get; set; }
        public IEnumerable<ReportComment> CommentReports { get; set; }
        public IEnumerable<ReportCommentReply> CommentReplyReports { get; set; }
        public IEnumerable<ReportUser> UserReports { get; set; }
    }
}
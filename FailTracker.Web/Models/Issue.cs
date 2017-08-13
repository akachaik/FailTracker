using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FailTracker.Web.Models
{
    public class Issue
    {
        public Issue()
        {
            
        }
        public Issue(ApplicationUser user, string subject, string body)
        {
            CreatedBy = user;
            Subject = subject;
            Body = body;
            CreatedAt = DateTime.Now;
        }

        public int IssueId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }

        public  ApplicationUser CreatedBy { get; set; }
    }
}

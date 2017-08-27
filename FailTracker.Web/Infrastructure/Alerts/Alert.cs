using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FailTracker.Web.Infrastructure.Alerts
{
    public class Alert
    {
        public Alert(string alertClass, string message)
        {
            AlertClass = alertClass;
            Message = message;
        }

        public string AlertClass { get; set; }
        public string Message { get; set; }

    }
}

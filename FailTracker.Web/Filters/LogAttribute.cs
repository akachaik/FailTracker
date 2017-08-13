using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using FailTracker.Web.Domain;
using FailTracker.Web.Models;
using Microsoft.AspNet.Identity;

namespace FailTracker.Web.Filters
{
    public class LogAttribute : ActionFilterAttribute
    {
        private IDictionary<string, object> _parameters;
        public ApplicationDbContext Context { get; set; }
        public string Description { get; set; }

        public LogAttribute(string description)
        {
            Description = description;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _parameters = filterContext.ActionParameters;
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var userId = filterContext.HttpContext.User.Identity.GetUserId();
            var user = Context.Users.Find(userId);

            var description = Description;

            foreach (var kvp in _parameters)
            {
                description = description.Replace("{" + kvp.Key + "}",
                    kvp.Value.ToString());
            }

            Context.Logs.Add(new LogAction(user,
                filterContext.ActionDescriptor.ActionName,
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                description));

            Context.SaveChanges();
        }
    }
}

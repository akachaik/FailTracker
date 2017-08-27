using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FailTracker.Web.Models;

namespace FailTracker.Web.Filters
{
    public class IssueTypeSelectListPopulatorAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;

            if (viewResult != null && viewResult.Model is IHaveIssueTypeSelectList)
            {
                ((IHaveIssueTypeSelectList)viewResult.Model).AvailableIssueTypes = GetAvilableIssueTypes();
            }
        }

        private SelectListItem[] GetAvilableIssueTypes()
        {
            return Enum.GetValues(typeof(IssueType))
                .Cast<IssueType>()
                .Select(t => new SelectListItem
                {
                    Text = t.ToString(),
                    Value = t.ToString()
                })
                .ToArray();

        }
    }

    public interface IHaveIssueTypeSelectList
    {
        SelectListItem[] AvailableIssueTypes { get; set; }
    }
}
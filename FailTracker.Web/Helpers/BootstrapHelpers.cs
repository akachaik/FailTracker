using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Mvc.Html;
using System.Web.Mvc.Html;

namespace FailTracker.Web.Helpers
{
    public static class BootstrapHelpers
    {
        public static IHtmlString BootstrapLabelFor<TModel, TProp>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProp>> property)
        {
            return helper.LabelFor(property, new
            {
                @class = "col-md-2 control-label"
            });
        }

        public static IHtmlString BootstrapLabel(this HtmlHelper helper, string propertyName)
        {
            return helper.Label(propertyName, new
            {
                @class = "col-md-2 control-label"
            });
        }
    }
}

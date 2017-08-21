using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using StructureMap.Configuration.DSL;
using Microsoft.AspNet.Identity;
using FailTracker.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Microsoft.Owin.Security;

namespace FailTracker.Web.Infrastructure
{
    public class MvcRegistry : Registry
    {
        public MvcRegistry()
        {
            For<BundleCollection>().Use(BundleTable.Bundles);
            For<RouteCollection>().Use(RouteTable.Routes);
            For<IIdentity>().Use(() => HttpContext.Current.User.Identity);
            For<HttpSessionStateBase>()
                .Use(() => new HttpSessionStateWrapper(HttpContext.Current.Session));
            For<HttpContextBase>()
                .Use(() => new HttpContextWrapper(HttpContext.Current));
            For<HttpServerUtilityBase>()
                .Use(() => new HttpServerUtilityWrapper(HttpContext.Current.Server));

            For<IUserStore<ApplicationUser>>()
                .Use<UserStore<ApplicationUser>>();
            For<DbContext>()
                .Use(() => new ApplicationDbContext());
            For<IAuthenticationManager>()
                .Use(() => HttpContext.Current.GetOwinContext().Authentication);
        }
    }
}

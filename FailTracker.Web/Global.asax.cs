using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FailTracker.Web.Infrastructure;
using StructureMap;
using StructureMap.TypeRules;

namespace FailTracker.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {

        public IContainer Container
        {
            get => (IContainer) HttpContext.Current.Items["_Container"];
            set => HttpContext.Current.Items["_Container"] = value;
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DependencyResolver.SetResolver(new StructureMapDependencyResolver(() => Container ?? ObjectFactory.Container));

            ObjectFactory.Configure(cfg =>
            {

                cfg.AddRegistry(new StandardRegistry());
                cfg.AddRegistry(new ControllerRegistry());
                cfg.AddRegistry(new ActionFilterRegistry(() => Container ?? ObjectFactory.Container));
            });
        }

        public void Application_BeginRequest()
        {
            Container = ObjectFactory.Container.GetNestedContainer();
        }

        public void Application_EndRequest()
        {
            Container.Dispose();
            Container = null;
        }
    }
}

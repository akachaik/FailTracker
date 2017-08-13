using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace FailTracker.Web.Infrastructure
{
    public class ActionFilterRegistry : Registry
    {
        public ActionFilterRegistry(Func<IContainer> containerFactory)
        {
            Scan(scan =>
            {
                For<IFilterProvider>()
                    .Use(new StructureMapFilterProvider(containerFactory));

                SetAllProperties(x =>
                    x.Matching(p =>
                        p.DeclaringType.CanBeCastTo(typeof(ActionFilterAttribute)) &&
                        p.DeclaringType.Namespace.StartsWith("FailTracker") &&
                        !p.PropertyType.IsPrimitive &&
                        p.PropertyType != typeof(string)));

            });
        }
    }
}

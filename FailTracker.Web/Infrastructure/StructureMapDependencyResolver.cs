using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace FailTracker.Web.Infrastructure
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                return null;
            }

            return serviceType.IsAbstract || serviceType.IsInterface
                ? ObjectFactory.TryGetInstance(serviceType)
                : ObjectFactory.GetInstance(serviceType);

        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ObjectFactory.GetAllInstances(serviceType).Cast<object>();
        }
    }
}

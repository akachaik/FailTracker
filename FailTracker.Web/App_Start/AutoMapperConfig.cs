using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FailTracker.Web.Controllers;
using FailTracker.Web.Infrastructure.Mapping;
using FailTracker.Web.Infrastructure.Tasks;
using FailTracker.Web.Models;

namespace FailTracker.Web
{
    public class AutoMapperConfig : IRunAtInit
    {
        public void Execute()
        {
            var types = Assembly.GetExecutingAssembly().GetExportedTypes();

            LoadStarndardMappings(types);
            LoadCustomMappings(types);
        }

        private void LoadCustomMappings(IEnumerable<Type> types)
        {
            var maps = (from t in types
                from i in t.GetInterfaces()
                where typeof(IHaveCustomMappings).IsAssignableFrom(t) &&
                      !t.IsAbstract &&
                      !t.IsInterface
                select (IHaveCustomMappings) Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
            {
                map.CreateMappings(Mapper.Configuration);
            }

        }

        private void LoadStarndardMappings(IEnumerable<Type> types)
        {
            var maps = (from t in types
                from i in t.GetInterfaces()
                where i.IsGenericType &&
                      i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                      !t.IsAbstract &&
                      !t.IsInterface
                select new
                {
                    Source = i.GetGenericArguments()[0],
                    Destination = t
                }).ToArray();

            foreach (var map in maps)
            {
                Mapper.CreateMap(map.Source, map.Destination);
            }

        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap.Configuration.DSL;

namespace FailTracker.Web.Infrastructure
{
    public class StandardRegistry : Registry
    {
        public StandardRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });
        }
    }
}

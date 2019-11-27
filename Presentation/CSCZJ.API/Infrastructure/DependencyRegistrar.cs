using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSCZJ.Core.Infrastructure;
using CSCZJ.Core.Infrastructure.DependencyManagement;

namespace CSCZJ.Web.Api.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 2; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {

            //TODO:
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;

namespace MultiCalc
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected async void Application_Start()
        {
/*            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            //builder.RegisterWebApiFilterProvider(config);
            // You can register hubs all at once using assembly scanning...
            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
*/



        }
    }
}

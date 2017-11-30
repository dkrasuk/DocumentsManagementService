using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.App_Start;
using WebApi.Providers;

namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();

            Bootstraper.Register(container);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);

            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory(container));

            var locator = new UnityServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => locator);

            config.EnableCors();
            //new CorsPolicyProvider()

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

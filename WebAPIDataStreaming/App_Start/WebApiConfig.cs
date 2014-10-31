using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPIDataStreaming.Controllers;
using WebAPIDataStreaming.Common;

namespace WebAPIDataStreaming
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
               name: "ActionApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new RequestModelValidator());
            
        }
    }
}

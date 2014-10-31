using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using WebAPISecureSocketLayering.Common;

namespace WebAPISecureSocketLayering
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
               routeTemplate: "{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            config.Filters.Add(new HttpsValidator());
            config.MessageHandlers.Add(new CertificateValidationHandler());
        }
    }
}

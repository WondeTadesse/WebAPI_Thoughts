//|---------------------------------------------------------------|
//|                         WEB API CLIENT                        |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                         WEB API CLIENT                        |
//|---------------------------------------------------------------|

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using Newtonsoft.Json;
using System.Web.Http.SelfHost;

namespace WebAPIClient
{
    /// <summary>
    /// OWIN server start up class
    /// </summary>
    public class ServerStartUp
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpListener listener = (HttpListener)appBuilder.Properties["System.Net.HttpListener"];

            // For Https set as an Anonymous 
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "OWINActionApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            appBuilder.UseWebApi(config);
        }
    }
}

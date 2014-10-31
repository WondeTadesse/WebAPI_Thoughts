using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebAPIDataStreaming
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static int  MaxRequestLength;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (Application.Get("Error") != null)
                Application.Set("Error", null);

            MaxRequestLength = GetMaxFileSize();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception != null)
            {
                if (Application.Get("Error") == null)
                    Application.Add("Error", exception);
                else
                    Application.Set("Error", exception);
                Response.Redirect("~/Error", false);
            }
        }

        private static int GetMaxFileSize()
        {
            try
            {
                Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
                HttpRuntimeSection httpRuntimeSection = configuration.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
                return httpRuntimeSection.MaxRequestLength;
            }
            catch
            {
                return -1;
            }
        }

    }
}

//|---------------------------------------------------------------|
//|                WEB API SECURE SOCKET LAYERING                 |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                WEB API SECURE SOCKET LAYERING                 |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

namespace WebAPISecureSocketLayering.Common
{
    /// <summary>
    /// Https URI validator class
    /// </summary>
    public class HttpsValidator : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Validate request URI
        /// </summary>
        /// <param name="actionContext">HttpActionContext value</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                var controllerFilters = actionContext.ControllerContext.ControllerDescriptor.GetFilters();
                var actionFilters = actionContext.ActionDescriptor.GetFilters();

                string responseMessage = string.Empty;

                if ((controllerFilters != null && controllerFilters.Select(t => t.GetType() == typeof(HttpsValidator)).Count() > 0) ||
                    (actionFilters != null && actionFilters.Select(t => t.GetType() == typeof(HttpsValidator)).Count() > 0))
                    responseMessage = "Requested URI requires HTTPS";

                if (!string.IsNullOrWhiteSpace(responseMessage))
                {
                    if (actionContext.Request != null && actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, responseMessage, new MediaTypeHeaderValue("text/json"));
                }
            }
        }
    }
}
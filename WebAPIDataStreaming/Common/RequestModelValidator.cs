//|---------------------------------------------------------------|
//|                     WEB API DATA STREAMING                    |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                     WEB API DATA STREAMING                    |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http.Headers;

using POCOLibrary;

namespace WebAPIDataStreaming.Common
{
    /// <summary>
    /// Request Model Validator class
    /// </summary>
    public class RequestModelValidator : ActionFilterAttribute
    {
        /// <summary>
        /// Occured before the action method is invoked
        /// </summary>
        /// <param name="actionContext">HttpActionContext value</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                // Validate ModelState
                if (!actionContext.ModelState.IsValid)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                     HttpStatusCode.BadRequest, new FileResponseMessage() { IsExists = false, Content = "Request is not valid !." },
                     new MediaTypeHeaderValue("text/json"));
                }
            }
        }
    }
}
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebAPISecureSocketLayering.Common;

using WebAPICommonLibrary;
using POCOLibrary;

namespace WebAPISecureSocketLayering.Controllers
{
    [HttpsValidator] // Enforce HTTPS request to the controller
    public class PhysicianController : PhysicianBaseController
    {
        [ActionName("GetPhysicians")]
        public new  HttpResponseMessage GetPhysicians()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, base.GetPhysicians(), new MediaTypeHeaderValue("application/json"));
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
        }

        [HttpGet]
        public new  HttpResponseMessage GetPhysician(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, base.GetPhysician(id), new MediaTypeHeaderValue("application/json"));
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
        }

        [HttpPost]
        public override HttpResponseMessage ActivePhysicians()
        {
            return base.ActivePhysicians();
        }

        [HttpGet]
        public override HttpResponseMessage RemovePhysician(int id)
        {
            return base.RemovePhysician(id);
        }

        [Route("AddPhysician")]
        [HttpPost]
        public HttpResponseMessage AddPhysician(HttpRequestMessage physicianRequest)
        {
            PhysicianBase physician = physicianRequest.Content.ReadAsAsync<PhysicianBase>().Result;
            return base.AddPhysician(physician);
        }
    }
}

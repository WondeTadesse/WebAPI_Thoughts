//|---------------------------------------------------------------|
//|                  WEB API DOCUMENTATION DEMO                   |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                  WEB API DOCUMENTATION DEMO                   |
//|---------------------------------------------------------------|

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Net.Http.Headers;

using POCOLibrary;
using WebAPICommonLibrary;
using WebAPIDocLib = WebAPIDocumentationExtenderLibrary;

namespace WebAPISecureSocketLayering.Controllers
{
    public class PhysicianController : PhysicianBaseController
    {
        [WebAPIDocLib.ResponseType(typeof(Physicians))]
        [HttpGet]
        public new HttpResponseMessage GetPhysicians()
        {
            return Request.CreateResponse(HttpStatusCode.OK, base.GetPhysicians(), new MediaTypeHeaderValue("application/json"));
        }

        [WebAPIDocLib.ResponseType(typeof(PhysicianBase))]
        [WebAPIDocLib.RequestType(typeof(InternalPhysicianBase), "id")]
        [HttpGet]
        public HttpResponseMessage GetPhysician(InternalPhysicianBase id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, base.GetPhysician(id.ID), new MediaTypeHeaderValue("application/json"));
        }

        [WebAPIDocLib.ResponseType(typeof(ExternalPhysician))]
        [HttpGet]
        public override HttpResponseMessage ActivePhysicians()
        {
            return base.ActivePhysicians();
        }

        [WebAPIDocLib.ResponseType(typeof(PhysicianBase))]
        [HttpPost]
        public override HttpResponseMessage RemovePhysician(int id)
        {
            return base.RemovePhysician(id);
        }

        /// <summary>
        /// Add internal physician
        /// </summary>
        /// <param name="physicianRequest">PhysicianRequest value</param>
        /// <returns></returns>
        [WebAPIDocLib.RequestType(typeof(InternalPhysicianBase), "physicianRequest")]
        [WebAPIDocLib.ResponseType(typeof(bool), typeof(Message))]
        [HttpPost]
        public HttpResponseMessage AddInternalPhysician(HttpRequestMessage physicianRequest)
        {
            InternalPhysician internalPhysician = physicianRequest.Content.ReadAsAsync<InternalPhysician>().Result;
            return base.AddPhysician(internalPhysician);
        }

    }
}

//|---------------------------------------------------------------|
//|                  WEB API DOCUMENTATION DEMO                   |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2014 - Present         |
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
    /// <summary>
    /// Physician controller
    /// </summary>
    public class PhysicianController : PhysicianBaseController
    {
        /// <summary>
        /// Get Physicians
        /// </summary>
        /// <returns>List of physicians</returns>
        [WebAPIDocLib.ResponseType(typeof(Physicians))]
        [HttpGet]
        public new HttpResponseMessage GetPhysicians()
        {
            return Request.CreateResponse(HttpStatusCode.OK, base.GetPhysicians(), new MediaTypeHeaderValue("application/json"));
        }

        /// <summary>
        /// Get Physician by ID
        /// </summary>
        /// <param name="physician">Physician object</param>
        /// <returns>physician object</returns>
        [WebAPIDocLib.ResponseType(typeof(PhysicianBase))]
        [HttpGet]
        public HttpResponseMessage GetPhysician([FromUri]int id)
        {
            // Try to get physician by ID or FirstName ....
            return Request.CreateResponse(HttpStatusCode.OK, base.GetPhysician(id), new MediaTypeHeaderValue("application/json"));
        }

        /// <summary>
        /// Get Active physicians
        /// </summary>
        /// <returns>List of physicians</returns>
        [WebAPIDocLib.ResponseType(typeof(ExternalPhysician))]
        [HttpGet]
        public new HttpResponseMessage ActivePhysicians()
        {
            return Request.CreateResponse(HttpStatusCode.OK, base.ActivePhysicians(), new MediaTypeHeaderValue("application/json")); 
        }

        /// <summary>
        /// Remove physician
        /// </summary>
        /// <param name="id">physician id value</param>
        /// <returns>Message object</returns>
        [WebAPIDocLib.ResponseType(typeof(PhysicianBase))]
        [HttpPost]
        public new HttpResponseMessage RemovePhysician([FromUri]int id)
        {
            return base.RemovePhysician(id);
        }

        /// <summary>
        /// Add internal physician
        /// </summary>
        /// <param name="physicianRequest">PhysicianRequest value</param>
        /// <returns>true/false or message object</returns>
        [WebAPIDocLib.RequestType(typeof(InternalPhysicianBase), "physicianRequest")]
        [WebAPIDocLib.ResponseType(typeof(bool), typeof(Message))]
        [HttpPost]
        public HttpResponseMessage AddInternalPhysician(HttpRequestMessage physicianRequest)
        {
            InternalPhysician internalPhysician = physicianRequest.Content.ReadAsAsync<InternalPhysician>().Result;
            return base.AddPhysician(internalPhysician);
        }

        /// <summary>
        /// Add external physician
        /// </summary>
        /// <param name="physician">Physician value</param>
        /// <returns>true/false or message object</returns>
        [WebAPIDocLib.RequestType(typeof(ExternalPhysicianBase), "physician")]
        [WebAPIDocLib.ResponseType(typeof(bool), typeof(Message))]
        [HttpPost]
        public HttpResponseMessage AddExternalPhysician([FromBody]ExternalPhysicianBase physician)
        {
            return base.AddPhysician(physician);
        }

    }
}

//|---------------------------------------------------------------|
//|                         WEB API CLIENT                        |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2014 - Present         |
//|---------------------------------------------------------------|
//|                         WEB API CLIENT                        |
//|---------------------------------------------------------------|

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using POCOLibrary;

namespace WebAPICommonLibrary
{
    /// <summary>
    /// Physician api controller class
    /// </summary>
    public class PhysicianSelfHostController : PhysicianBaseController
    {
        /// <summary>
        /// Calculate Salary raise
        /// </summary>
        /// <param name="request">HttpRequestMessage value</param>
        /// <returns>HttpResponseMessage object</returns>
        [AcceptVerbs("Post","Put")]
        public HttpResponseMessage CalculateSalaryRaise(HttpRequestMessage request)
        {
            try
            {
                InternalPhysician physicianBase = request.Content.ReadAsAsync<InternalPhysician>().Result;
                physicianBase.Salary = physicianBase.CalculateSalaryRaise();
                return Request.CreateResponse(HttpStatusCode.OK, physicianBase, new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new Message().Content = string.Concat("Error happened : - Message ", exception.Message),
                    new MediaTypeHeaderValue("text/json"));
            }
        }       
    }
}

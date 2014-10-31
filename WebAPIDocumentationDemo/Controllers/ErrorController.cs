using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAPIDocumentationDemo.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            ViewBag.Error = Server.GetLastError() != null ? Server.GetLastError().Message : string.Empty;
            return View();
        }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebAPIDocumentationDemo.Areas.HelpPage.Controllers;

namespace WebAPIDocumentationDemo.Controllers
{
    public class HomeController : Controller
    {        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }
    }
}

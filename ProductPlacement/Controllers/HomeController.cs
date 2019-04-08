using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductPlacement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Brand Detector";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Address:";

            return View();
        }
    }
}
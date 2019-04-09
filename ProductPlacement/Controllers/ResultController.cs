using ProductPlacement.Models;
using Newtonsoft.Json;
using System.Web.Mvc;


namespace ProductPlacement.Controllers
{
    public partial class ResultController : Controller
    {
        // GET: Result
        public ActionResult Result(int ar_brandindextoshow)
        {
            var l_EvaluationResults = Session["results"] as EvaluationResults;
            l_EvaluationResults.BrandIndexToShow = ar_brandindextoshow;

            return View("Result");
        }
    }
}

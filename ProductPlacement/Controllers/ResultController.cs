using ProductPlacement.Models;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.IO;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Helpers;

namespace ProductPlacement.Controllers
{
    public partial class ResultController : Controller
    {
        // GET: Result
        public ActionResult Result(int ar_brandindextoshow)
        {
            var l_EvaluationResults = Session["results"] as EvaluationResults;
            l_EvaluationResults.BrandIndexToShow = ar_brandindextoshow;

            return View(l_EvaluationResults);
        }

        // GET: Result
        public ActionResult Sample(string ar_resultfolder_name)
        {
            //===read the file result found in ar_workingfolder
            string ls_resultfile_path_and_name = ConfigurationManager.AppSettings["pathforuploadingvideos"] + "/" + ar_resultfolder_name + "/Results.txt";

            EvaluationResults l_evaluationResults = new EvaluationResults();
            List<DataResult> l_list_DataResult = new List<DataResult>();
            List<string> l_list_BrandNames = new List<string>();

            using (StreamReader r = new StreamReader(ls_resultfile_path_and_name))
            {
                string l_json = r.ReadToEnd();
                l_list_DataResult = (List<DataResult>) JsonConvert.DeserializeObject<IEnumerable<DataResult>>(l_json);
            }

            string l_domainnamefordownloadingresults = ConfigurationManager.AppSettings["domainnamefordownloadingresults"];
            string l_ResultPathURL = l_domainnamefordownloadingresults + "/" + ar_resultfolder_name;
            l_evaluationResults.ResultPathURL = l_ResultPathURL;

            l_evaluationResults.BrandIndexToShow = 0;
            l_evaluationResults.array_DataResult = l_list_DataResult.ToArray();

            //===getting distinct BrandNames
            foreach (DataResult l_dataresult in l_evaluationResults.array_DataResult)
            {
                l_list_BrandNames.Add(l_dataresult.BrandName);
            }
            l_evaluationResults.BrandNames = l_list_BrandNames.ToArray().Distinct().ToArray();


            HttpContext.Session["results"] = l_evaluationResults;

            return RedirectToAction("Result", "Result", new { ar_brandindextoshow = 0 });
        }

    }
}

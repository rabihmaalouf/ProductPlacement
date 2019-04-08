using ProductPlacement.Models;
using Newtonsoft.Json;
using System.Web.Mvc;


namespace ProductPlacement.Controllers
{
    public partial class ResultController : Controller
    {
        // GET: Result
        public ActionResult Result(string ar_EvaluationResults_serialized)
        {
            EvaluationResults l_EvaluationResults = (EvaluationResults)JsonConvert.DeserializeObject(ar_EvaluationResults_serialized, typeof(EvaluationResults));

            ViewData["param_evaluationResults"] = l_EvaluationResults;

            return View("Result");
        }
    }
}




/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 public class ResultController : Controller
    {
        private product_placementContext db = new product_placementContext();

        // GET: Result
        public ActionResult Index()
        {

            EvaluationResults l_evaluationResults = TempData["temp_param_evaluationResults"] as EvaluationResults;

            ViewData["param_evaluationResults"] = l_evaluationResults;
            return View();
        }

        public ActionResult FetchGalleryData(string ar_result_path, string[] ar_brand_names)
        {
            var imagesModel = new List<ImageGallery>();

            string l_result_folder_name = Path.Combine(ar_result_path, ar_brand_names[0]); //===showing by default the results of first brand
            string l_path_for_uploading_videos = ConfigurationManager.AppSettings["pathforuploadingvideos"];
            var imageFiles = Directory.GetFiles(Path.Combine(l_path_for_uploading_videos, l_result_folder_name));

            Guid obj = Guid.NewGuid();

            foreach (var item in imageFiles)
            {
                ImageGallery l_temp_ImageGallery = new ImageGallery();
                l_temp_ImageGallery.ID = Guid.NewGuid();
                l_temp_ImageGallery.Name = Path.GetFileName(item);
                l_temp_ImageGallery.ImagePath = l_result_folder_name;

                imagesModel.Add(l_temp_ImageGallery);

            }
            //return View(imagesModel);
            return Json(imagesModel, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Index()
        //{

        //    List<ImageGallery> imagesModel = new List<ImageGallery>();
        //    var imageFiles = Directory.GetFiles(Server.MapPath("~/Results/epson/"));
        //    Guid obj = Guid.NewGuid();

        //    foreach (var item in imageFiles)
        //    {
        //        ImageGallery l_temp_ImageGallery = new ImageGallery();
        //        l_temp_ImageGallery.ID = Guid.NewGuid();
        //        l_temp_ImageGallery.Name = Path.GetFileName(item);
        //        l_temp_ImageGallery.ImagePath = Path.GetFullPath(item);

        //        imagesModel.Add(l_temp_ImageGallery);

        //    }
        //    return View(imagesModel);
        //    //return View(imagesModel.ImageList.ToList());
        //    //IEnumerable<product_placement.Models.ImageGallery>
        //    //}

        //    //return View(db.ImageGalleries.ToList());
        //}

        // GET: Result/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageGallery imageGallery = db.ImageGalleries.Find(id);
            if (imageGallery == null)
            {
                return HttpNotFound();
            }
            return View(imageGallery);
        }

        // GET: Result/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Result/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ImagePath")] ImageGallery imageGallery)
        {
            if (ModelState.IsValid)
            {
                imageGallery.ID = Guid.NewGuid();
                db.ImageGalleries.Add(imageGallery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(imageGallery);
        }

        // GET: Result/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageGallery imageGallery = db.ImageGalleries.Find(id);
            if (imageGallery == null)
            {
                return HttpNotFound();
            }
            return View(imageGallery);
        }

        // POST: Result/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ImagePath")] ImageGallery imageGallery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(imageGallery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(imageGallery);
        }

        // GET: Result/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageGallery imageGallery = db.ImageGalleries.Find(id);
            if (imageGallery == null)
            {
                return HttpNotFound();
            }
            return View(imageGallery);
        }

        // POST: Result/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ImageGallery imageGallery = db.ImageGalleries.Find(id);
            db.ImageGalleries.Remove(imageGallery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    

 
 */

using Newtonsoft.Json;
using ProductPlacement.CloudVision;
using ProductPlacement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;

public class DemoMainController : Controller
{
    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Index(FileModel FileModel)
    {

        string l_fileName = Path.GetFileName(FileModel.PostedFile.FileName);
        string l_brand1 = FileModel.BrandName1;
        string l_brand2 = FileModel.BrandName2;
        string l_brand3 = FileModel.BrandName3;
        Double l_cost_of_1_second = FileModel.CostOf1Second;

        List<string> l_BrandNames = new List<string>();

        if (!String.IsNullOrEmpty(l_brand1)) {l_BrandNames.Add(l_brand1);}
        if (!String.IsNullOrEmpty(l_brand2)) {l_BrandNames.Add(l_brand2);}
        if (!String.IsNullOrEmpty(l_brand3)) {l_BrandNames.Add(l_brand3);}
        string[] l_arrays_BrandNames = l_BrandNames.ToArray();


        string l_path_for_uploading_videos = ConfigurationManager.AppSettings["pathforuploadingvideos"];
        if (!Directory.Exists(l_path_for_uploading_videos)) {Directory.CreateDirectory(l_path_for_uploading_videos);}

        //===create folder for each uploaded video
        int l_directoryCount = Directory.GetDirectories(l_path_for_uploading_videos).Length + 1;

        string l_currenttime = DateTime.Now.ToString("yyyyMMdd_hhmmss");

        string l_working_folder_name = "video_" + l_directoryCount.ToString() + "_" + l_currenttime;
        string l_path_for_uploading_current_video = Path.Combine(l_path_for_uploading_videos, l_working_folder_name);
        Directory.CreateDirectory(l_path_for_uploading_current_video);

        string l_uploaded_file_path_and_name = Path.Combine(l_path_for_uploading_current_video, l_fileName);
        FileModel.PostedFile.SaveAs(l_uploaded_file_path_and_name);
        ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", l_fileName);

        brand_detection.f_main(l_path_for_uploading_videos, l_working_folder_name, l_fileName, l_arrays_BrandNames, l_cost_of_1_second);

        return RedirectToAction("Result", "Result", new { ar_brandindextoshow = 0 });

    }
}

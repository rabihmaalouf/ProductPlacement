using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Vision.V1;
using OpenCvSharp;
using Newtonsoft.Json;
using System.Configuration;
using ProductPlacement.Models;
using System.Web.Hosting;

namespace ProductPlacement.CloudVision
{
    public class brand_detection
    {

        public static EvaluationResults f_main(String ar_path_for_uploading_videos, String ar_working_folder_name, String ar_uploaded_video_name, String[] ar_brand_names, double ar_cost_of_1_second)
        {
            //===initializing google API key
            string ls_google_app_credentials_path_and_filename = HostingEnvironment.MapPath("~/CloudVision/google_cloud_credential_for_logo_detection-nowting-bd7886019869.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", ls_google_app_credentials_path_and_filename);

            VideoCapture l_capture = VideoCapture.FromFile(Path.Combine(ar_path_for_uploading_videos, ar_working_folder_name, ar_uploaded_video_name));
            Double l_framerate = l_capture.Get(5);
            Double l_frameid;
            int li_counter = 0;
            Mat l_frame = new Mat();
            Mat l_frame_with_boundarie = new Mat();

            String l_result_folder_name = Path.Combine(ar_working_folder_name, "Results");
            String ls_result_folder_path = Path.Combine(ar_path_for_uploading_videos, l_result_folder_name);
            Directory.CreateDirectory(ls_result_folder_path);

            while (true)
            {
                l_capture.Read(l_frame);
                if (l_frame.Empty()) break;

                l_frameid = l_capture.Get(1);
                if (l_frameid % l_framerate != 0) continue; //===getting 1 frame per second

                //======================================================================================================
                //======================================================================================================
                li_counter++;
                //if (li_counter != 26) { continue; } //=== temp code to process only 1 frame
                if (li_counter != 26 && li_counter!= 27 && li_counter != 28) { continue; } //=== temp code to process only 3 frames
                //======================================================================================================
                //======================================================================================================

                //===find all texts in frame
                AnnotateImageResponse l_response = f_find_brand_by_DetectDocumentText_in(l_frame);

                //===set a rectangle over each corresponding brand-text and save the frame
                f_hilight_brand_and_save_frame(l_frame, l_response, ar_brand_names, ar_cost_of_1_second, ls_result_folder_path, "pic_" + li_counter.ToString());

            }

            EvaluationResults l_EvaluationResults = f_get_evaluationresults(l_result_folder_name, ar_brand_names);

            return l_EvaluationResults;
        }


        public static AnnotateImageResponse f_find_brand_by_DetectDocumentText_in(Mat ar_frame)
        {
            var client = ImageAnnotatorClient.Create();

            Byte[] l_image_in_byte = ar_frame.ToBytes(".png");
            Image image = Image.FromBytes(l_image_in_byte);

            AnnotateImageRequest request = new AnnotateImageRequest
            {
                Image = image,
                Features =
                {
                    new Feature { Type = Feature.Types.Type.DocumentTextDetection },
                }
            };
            AnnotateImageResponse response = client.Annotate(request);

            return response;

        }

        public static void f_hilight_brand_and_save_frame(Mat ar_frame, AnnotateImageResponse ar_AnnotateImageResponse, string[] ar_brand_names, double ar_cost_of_1_second, string ar_result_path, string ar_picture_name)
        {

            Point p = new Point();
            List<Point> l_list_points = new List<Point>();
            List<List<Point>> ListOfListOfPoint = new List<List<Point>>();

            EntityAnnotation annotation = new EntityAnnotation();
            Boolean l_at_least_1_occurence_found = false;

            foreach (string l_brand_name in ar_brand_names)
            {
                if (String.IsNullOrEmpty(l_brand_name.Trim())) { continue; }

                Mat l_frame = new Mat();
                ar_frame.CopyTo(l_frame);
                int li_count_of_brand_occurences_found = 0;

                l_at_least_1_occurence_found = false;
                int li_count_of_all_occurences_found = ar_AnnotateImageResponse.TextAnnotations.Count;
                for (int i = 1; i < li_count_of_all_occurences_found; i++) //=== skip i=0 as it contain all occurences
                {
                    annotation = ar_AnnotateImageResponse.TextAnnotations[i];

                    if (annotation.Description != null && annotation.Description.ToLower().Contains(l_brand_name.ToLower()))
                    {
                        l_at_least_1_occurence_found = true;
                        li_count_of_brand_occurences_found++;

                        l_list_points.Clear();
                        ListOfListOfPoint.Clear();

                        p.X = annotation.BoundingPoly.Vertices.ElementAt(0).X;
                        p.Y = annotation.BoundingPoly.Vertices.ElementAt(0).Y;
                        l_list_points.Add(new Point(p.X, p.Y));

                        p.X = annotation.BoundingPoly.Vertices.ElementAt(1).X;
                        p.Y = annotation.BoundingPoly.Vertices.ElementAt(1).Y;
                        l_list_points.Add(new Point(p.X, p.Y));

                        p.X = annotation.BoundingPoly.Vertices.ElementAt(2).X;
                        p.Y = annotation.BoundingPoly.Vertices.ElementAt(2).Y;
                        l_list_points.Add(new Point(p.X, p.Y));

                        p.X = annotation.BoundingPoly.Vertices.ElementAt(3).X;
                        p.Y = annotation.BoundingPoly.Vertices.ElementAt(3).Y;
                        l_list_points.Add(new Point(p.X, p.Y));

                        ListOfListOfPoint.Add(l_list_points);

                        Cv2.Polylines(l_frame, ListOfListOfPoint, true, Scalar.Red, 1);
                    }
                }

                string l_picutre_name;
                if (l_at_least_1_occurence_found)
                {
                    l_picutre_name  = ar_picture_name + "_pm.png"; //===pm as Positive match

                    //===log result in JSON file for only positive match
                    data_result l_current_data_result = new data_result();
                    l_current_data_result.brand_name = l_brand_name;
                    l_current_data_result.frame_ref = l_picutre_name;
                    l_current_data_result.total_occurences = li_count_of_brand_occurences_found;
                    l_current_data_result.cost = li_count_of_brand_occurences_found * ar_cost_of_1_second;

                    List<data_result> l_data_result = new List<data_result>();
                    l_data_result.Add(l_current_data_result);

                    using (StreamWriter l_file = File.AppendText(Path.Combine(ar_result_path, "Results.txt")))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(l_file, l_data_result);
                    }

                } 

                else
                { l_picutre_name = ar_picture_name + "_fm.png"; }//===fm as False match

                String ls_brand_result_path = Path.Combine(ar_result_path, l_brand_name);
                Directory.CreateDirectory(ls_brand_result_path);
                String ls_brand_result_path_and_name = Path.Combine(ls_brand_result_path, l_picutre_name);

                Cv2.ImWrite(ls_brand_result_path_and_name, l_frame);
            }

        }



        public static EvaluationResults f_get_evaluationresults(string ar_ResultPath, string[] ar_BrandNames)
        {
            EvaluationResults l_evaluationResults = new EvaluationResults();

            ImageGallery l_ImageGallery;
            List<ImageGallery> l_list_ImageGallery = new List<ImageGallery>();

            string l_domainnamefordownloadingresults = ConfigurationManager.AppSettings["domainnamefordownloadingresults"];
            string l_path_for_uploading_videos = ConfigurationManager.AppSettings["pathforuploadingvideos"];

            foreach (string ar_Brand in ar_BrandNames)
            {
                string l_result_folder_name = Path.Combine(ar_ResultPath, ar_Brand);

                var l_imageFiles = Directory.GetFiles(Path.Combine(l_path_for_uploading_videos, l_result_folder_name));

                List<string> l_list_ImageFileNames = new List<string>();
                foreach (var item in l_imageFiles)
                {
                    l_list_ImageFileNames.Add(Path.GetFileName(item));
                }
                l_ImageGallery = new ImageGallery();
                l_ImageGallery.BrandName = ar_Brand;
                l_ImageGallery.ImageFileNames = l_list_ImageFileNames.ToArray();

                l_list_ImageGallery.Add(l_ImageGallery);

            }

            l_evaluationResults.ResultPath = ar_ResultPath;
            l_evaluationResults.ResultPathURL = l_domainnamefordownloadingresults + "/" + ar_ResultPath.Replace("\\", "/");
            l_evaluationResults.BrandIndexToShow = 0; //===show by default the first brand
            l_evaluationResults.array_ImageGallery = l_list_ImageGallery.ToArray();

            return l_evaluationResults;
        }
    }


    public class data_result
    {
        public string brand_name { get; set; }
        public string frame_ref { get; set; }
        public int total_occurences { get; set; }
        public double cost { get; set; }

    }


}

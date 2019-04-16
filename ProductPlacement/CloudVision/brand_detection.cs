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
using System.Web;

namespace ProductPlacement.CloudVision
{
    public class brand_detection
    {

        public static void f_main(String ar_path_for_uploading_videos, String ar_working_folder_name, String ar_uploaded_video_name, String[] ar_brand_names, double ar_cost_of_1_second)
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

            String ls_result_folder_path = Path.Combine(ar_path_for_uploading_videos, ar_working_folder_name);
            Directory.CreateDirectory(ls_result_folder_path);

            List<DataResult> l_list_DataResult = new List<DataResult>();

            while (true)
            {
                l_capture.Read(l_frame);
                if (l_frame.Empty()) break;

                l_frameid = l_capture.Get(1);
                if (l_frameid % l_framerate != 0) continue; //===getting 1 frame per second

                //======================================================================================================
                //======================================================================================================
                li_counter++;
                if (li_counter != 1 && li_counter != 2 && li_counter != 3) { continue; } //=== temp code to process only the first 3 frames
                //======================================================================================================
                //======================================================================================================

                //===find all texts in frame
                AnnotateImageResponse l_response = f_find_brand_by_DetectDocumentText_in(l_frame);

                //===set a rectangle over each corresponding brand-text and save the frame
                foreach (string l_brand_name in ar_brand_names)
                {
                    if (String.IsNullOrEmpty(l_brand_name.Trim())) { continue; }

                    DataResult l_current_data_result = f_hilight_brand_and_save_frame(l_frame, l_response, l_brand_name, ar_cost_of_1_second, ls_result_folder_path, "pic_" + li_counter.ToString(), li_counter);
                    l_list_DataResult.Add(l_current_data_result);
                }

            }

            //===write result into file
            using (StreamWriter l_file = File.AppendText(Path.Combine(ls_result_folder_path, "Results.txt")))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(l_file, l_list_DataResult);
            }


            string l_domainnamefordownloadingresults = ConfigurationManager.AppSettings["domainnamefordownloadingresults"];

            EvaluationResults l_EvaluationResults = new EvaluationResults();
            l_EvaluationResults.ResultPathURL = l_domainnamefordownloadingresults + "/" + ar_working_folder_name;
            l_EvaluationResults.BrandNames = ar_brand_names;
            l_EvaluationResults.BrandIndexToShow = 0;
            l_EvaluationResults.array_DataResult = l_list_DataResult.ToArray();

            HttpContext.Current.Session["results"] = l_EvaluationResults;

            return;
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

        public static DataResult f_hilight_brand_and_save_frame(Mat ar_frame, AnnotateImageResponse ar_AnnotateImageResponse, string ar_brand_name, double ar_cost_of_1_second, string ar_result_path, string ar_picture_name, int ar_second_number)
        {

            Point p = new Point();
            List<Point> l_list_points = new List<Point>();
            List<List<Point>> ListOfListOfPoint = new List<List<Point>>();

            EntityAnnotation annotation = new EntityAnnotation();
            Boolean l_at_least_1_occurence_found = false;

            List<DataResult> l_data_result = new List<DataResult>();


            Mat l_frame = new Mat();
            ar_frame.CopyTo(l_frame);
            int li_count_of_brand_occurences_found = 0;

            l_at_least_1_occurence_found = false;
            int li_count_of_all_occurences_found = ar_AnnotateImageResponse.TextAnnotations.Count;
            for (int i = 1; i < li_count_of_all_occurences_found; i++) //=== skip i=0 as it contain all occurences
            {
                annotation = ar_AnnotateImageResponse.TextAnnotations[i];

                if (annotation.Description != null && annotation.Description.ToLower().Contains(ar_brand_name.ToLower()))
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
                l_picutre_name = ar_picture_name + "_pm.png"; //===pm as Positive match
            }

            else
            { l_picutre_name = ar_picture_name + "_fm.png"; }//===fm as False match

            //===save picture
            String ls_brand_result_path = Path.Combine(ar_result_path, ar_brand_name);
            Directory.CreateDirectory(ls_brand_result_path);
            String ls_brand_result_path_and_name = Path.Combine(ls_brand_result_path, l_picutre_name);
            Cv2.ImWrite(ls_brand_result_path_and_name, l_frame);

            //===log result into DataResult
            DataResult l_current_data_result = new DataResult();
            l_current_data_result.BrandName = ar_brand_name;
            l_current_data_result.FrameRef = l_picutre_name;
            l_current_data_result.SecondRef = ar_second_number;
            l_current_data_result.TotalOccurences = li_count_of_brand_occurences_found;
            l_current_data_result.Cost = li_count_of_brand_occurences_found * ar_cost_of_1_second;
            l_data_result.Add(l_current_data_result);

            //===return DataResult
            return l_current_data_result;
        }
    }
}

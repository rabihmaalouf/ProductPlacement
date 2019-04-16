using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ProductPlacement.Models
{
    public class DataResult
    {
        public string BrandName { get; set; }
        public string FrameRef { get; set; }
        public int SecondRef { get; set; }
        public int TotalOccurences { get; set; }
        public double Cost { get; set; }
    }

    public class EvaluationResults
    {
        public string ResultPathURL;
        public string[] BrandNames;
        public int BrandIndexToShow;
        public DataResult[] array_DataResult;
    }
}


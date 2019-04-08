using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ProductPlacement.Models
{
    public class ImageGallery
    {
        public string BrandName;
        public string[] ImageFileNames { get; set; }
    }

    public class EvaluationResults
    {
        public string ResultPath;
        public string ResultPathURL;
        public int BrandIndexToShow;
        public ImageGallery[] array_ImageGallery;
    }
}


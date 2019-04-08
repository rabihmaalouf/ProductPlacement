using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProductPlacement.Models
{
    public class FileModel
    {
        [DataType(DataType.Upload)]
        [DisplayName("Upload File")]
        [Required(ErrorMessage = "Please Choose a Video to Upload.")]
        public HttpPostedFileBase PostedFile { get; set; }

        [DisplayName("Brand Name 1")]
        [Required(ErrorMessage = "Please Mention a Brand Name")]
        public string BrandName1 { get; set; }

        [DisplayName("Brand Name 2")]
        public string BrandName2 { get; set; }

        [DisplayName("Brand Name 3")]
        public string BrandName3 { get; set; }

        [DisplayName("Cost of 1 Second")]
        [Range(0, 1000, ErrorMessage = "The Value Must be Between 0 and 1000")]
        public double CostOf1Second { get; set; }

    }

}

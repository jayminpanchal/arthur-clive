using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Arthur_Clive.Data
{
    /// <summary>Details of image to be uploaded to S3</summary>
    public class ImageData
    {
        /// <summary>Name of Object</summary>
        [Required]
        public string ObjectName { get; set; }
        /// <summary>Name of bucket</summary>
        [Required]
        public string BucketName { get; set; }
        /// <summary>Data of image uploaded</summary>
        [Required]
        [FileExtensions(Extensions = "jpg,jpeg")]
        public IFormFile Image { get; set; }
    }


}

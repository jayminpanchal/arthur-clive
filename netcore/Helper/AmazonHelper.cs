using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using Arthur_Clive.Logger;
using Microsoft.AspNetCore.Http;

namespace Arthur_Clive.Helper
{
    /// <summary>Helper method for Amazon S3</summary>
    public class AmazonHelper
    {
        /// <summary>Amazon s3 client</summary>
        public static IAmazonS3 s3Client;

        /// <summary>Get Amazon S3 client</summary>
        public static IAmazonS3 GetAmazonS3Client()
        {
            string accessKey = GlobalHelper.ReadXML().Elements("amasons3").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("accesskey").FirstOrDefault().Value;
            string secretKey = GlobalHelper.ReadXML().Elements("amasons3").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("secretkey").FirstOrDefault().Value;
            s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.APSouth1);
            return s3Client;
        }

        /// <summary>Get Amazon S3 presigned url for object</summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        public static string GetAmazonS3Object(string bucketName, string objectKey)
        {
            try
            {
                GetAmazonS3Client();
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.Now.AddMinutes(5)
                };
                string url = s3Client.GetPreSignedURL(request);
                return url;
            }
            catch (Exception ex)
            {
                Logger.LoggerDataAccess.CreateLog("AmazonHelper", "GetAmazonS3Object", ex.Message);
                return "";
            }
        }

        /// <summary>Get Amazon S3 url without credentials for object</summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        public static string GetS3Object(string bucketName, string objectName)
        {
            string s3PrefixUrl = "https://s3.ap-south-1.amazonaws.com/";
            string presignedUrl = s3PrefixUrl + bucketName + "/" + objectName;
            return presignedUrl;
        }

        /// <summary>Upload image to s3</summary>
        /// <param name="file">Details of image</param>
        /// <param name="bucketName">Details of image</param>
        /// <param name="objectName">Details of image</param>
        public async static Task<bool> UploadImageToS3(IFormFile file, string bucketName, string objectName)
        {
            try
            {
                IAmazonS3 client;
                using (client = GetAmazonS3Client())
                {
                    var request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead,
                        Key = string.Format(objectName),
                        InputStream = file.OpenReadStream()
                    };

                    await client.PutObjectAsync(request);
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AmazonHelper", "UploadImageToS3", ex.Message);
                return false;
            }
        }
    }
}

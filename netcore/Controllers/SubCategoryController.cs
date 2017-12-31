using System;
using Arthur_Clive.Data;
using Arthur_Clive.Logger;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using AH = Arthur_Clive.Helper.AmazonHelper;
using WH = Arthur_Clive.Helper.MinioHelper;
using MH = Arthur_Clive.Helper.MongoHelper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Arthur_Clive.Controllers
{
    /// <summary>Controller to get subcategorised products</summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SubCategoryController : Controller
    {
        /// <summary></summary>
        public IMongoDatabase _db = MH._client.GetDatabase("ProductDB");

        /// <summary>Get the product that matches the filters</summary>
        /// <param name="productFor">Whom is the product for</param>
        /// <param name="productType">Type of product</param>
        /// <remarks>This api is used to get product that falls under the filters productFor and productType</remarks>
        /// <response code="200">Returns products that match the filter</response>
        /// <response code="404">No products found</response> 
        /// <response code="400">Process ran into an exception</response>  
        [HttpGet("{productFor}/{productType}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> Get(string productFor, string productType)
        {
            try
            {
                IAsyncCursor<Product> cursor = await _db.GetCollection<Product>("Product").FindAsync(Builders<Product>.Filter.Eq("ProductFor", productFor) & Builders<Product>.Filter.Eq("ProductType", productType));
                var products = cursor.ToList();
                if (products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        string objectName = product.ProductSKU + ".jpg";
                        //product.MinioObject_URL = WH.GetMinioObject("arthurclive-products", objectName).Result;
                        //product.MinioObject_URL = AH.GetAmazonS3Object("arthurclive-products", objectName);
                        product.MinioObject_URL = AH.GetS3Object("arthurclive-products", objectName);
                    }
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Success",
                        Data = products
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "No products found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("SubCategoryController", "Get", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

    }
}
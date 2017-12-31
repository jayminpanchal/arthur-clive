using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Minio;
using System;
using Arthur_Clive.Data;
using Arthur_Clive.Logger;
using System.Threading.Tasks;
using MongoDB.Driver;
using AH = Arthur_Clive.Helper.AmazonHelper;
using WH = Arthur_Clive.Helper.MinioHelper;
using MH = Arthur_Clive.Helper.MongoHelper;
using Swashbuckle.AspNetCore.Examples;
using Arthur_Clive.Swagger;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Arthur_Clive.Controllers
{
    /// <summary>Controller to get, post and delete products</summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        /// <summary></summary>
        public IMongoDatabase _db = MH._client.GetDatabase("ProductDB");

        /// <summary>Get all the products </summary>
        /// <remarks>This api is used to get all the products</remarks>
        /// <response code="200">Returns products</response>
        /// <response code="404">No products found</response> 
        /// <response code="400">Process ran into an exception</response>   
        [HttpGet]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> Get()
        {
            try
            {
                var collection = _db.GetCollection<Product>("Product");
                var filter = FilterDefinition<Product>.Empty;
                IAsyncCursor<Product> cursor = await collection.FindAsync(filter);
                var products = cursor.ToList();
                if (products.Count > 0)
                {
                    foreach (var data in products)
                    {
                        string objectName = data.ProductSKU + ".jpg";
                        //data.ObjectURL = WH.GetMinioObject("products", objectName).Result;
                        //data.ObjectURL = AH.GetAmazonS3Object("arthurclive-products", objectName);
                        data.MinioObject_URL = AH.GetS3Object("arthurclive-products", objectName);
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
                LoggerDataAccess.CreateLog("ProductController", "Get", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Get products with filter</summary>
        /// <param name="productFor">For whom is the product</param>
        /// <param name="productType">type of product</param>
        /// <param name="productDesign">Design on product</param>
        /// <response code="200">Returns products that match the filters</response>
        /// <response code="404">No products found</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpGet("{productFor}/{productType}/{productDesign}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> GetProductByFilter(string productFor, string productType, string productDesign)
        {
            try
            {
                var collection = _db.GetCollection<Product>("Product");
                var filter = Builders<Product>.Filter.Eq("ProductFor", productFor) & Builders<Product>.Filter.Eq("ProductType", productType) & Builders<Product>.Filter.Eq("ProductDesign", productDesign);
                IAsyncCursor<Product> cursor = await collection.FindAsync(filter);
                var products = cursor.ToList();
                if (products.Count > 0)
                {
                    foreach (var data in products)
                    {
                        string objectName = data.ProductSKU + ".jpg";
                        //data.ObjectURL = WH.GetMinioObject("products", objectName).Result;
                        //data.ObjectURL = AH.GetAmazonS3Object("arthurclive-products", objectName);
                        data.MinioObject_URL = AH.GetS3Object("arthurclive-products", objectName);
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
                LoggerDataAccess.CreateLog("ProductController", "Get", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Insert a new products </summary>
        /// <remarks>This api is used to insert a new product</remarks>
        /// <param name="product">Details of product to be inserted</param>
        /// <response code="200">Product inserted successfully</response>
        /// <response code="400">Process ran into an exception</response> 
        [Authorize("Level1Access")]
        [HttpPost]
        [SwaggerRequestExample(typeof(Product), typeof(InsertProductDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> Post([FromBody]Product product)
        {
            try
            {
                product.ProductDiscountPrice = (product.ProductPrice - (product.ProductPrice * (product.ProductDiscount / 100)));
                product.ProductSKU = product.ProductFor + "-" + product.ProductType + "-" + product.ProductDesign + "-" + product.ProductColour + "-" + product.ProductSize;
                string objectName = product.ProductSKU + ".jpg";
                //product.MinioObject_URL = WH.GetMinioObject("arthurclive-products", objectName).Result;
                //product.MinioObject_URL = AH.GetAmazonS3Object("arthurclive-products", objectName);
                product.MinioObject_URL = AH.GetS3Object("arthurclive-products", objectName);
                await _db.GetCollection<Product>("Product").InsertOneAsync(product);
                return Ok(new ResponseData
                {
                    Code = "200",
                    Message = "Inserted",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("ProductController", "Post", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Delete a product</summary>
        /// <param name="productSKU">SKU of product to be deleted</param>
        /// <remarks>This api is used to delete a product</remarks>
        /// <response code="200">Product deleted successfully</response>
        /// <response code="404">No product found</response>   
        /// <response code="400">Process ran into an exception</response>   
        [Authorize("Level1Access")]
        [HttpDelete("{productSKU}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult Delete(string productSKU)
        {
            try
            {
                var product = MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("ProductSKU", productSKU), "ProductDB", "Product").Result;
                if (product != null)
                {
                    var response = _db.GetCollection<Product>("Product").DeleteOneAsync(product);
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Deleted",
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "Product Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("ProductController", "Delete", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Update product details</summary>
        /// <param name="data">Details of product</param>
        /// <param name="productSKU">SKU of product to be updated</param>
        /// <response code="200">Product updated successfully</response>
        /// <response code="404">No product found</response>   
        /// <response code="400">Process ran into an exception</response> 
        [Authorize("Level1Access")]
        [HttpPut("{productSKU}")]
        [SwaggerRequestExample(typeof(UpdateProduct), typeof(UpdateProductDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> Update([FromBody]UpdateProduct data, string productSKU)
        {
            try
            {
                var checkData = MH.CheckForDatas("ProductSKU", productSKU, null, null, "ProductDB", "Product");
                if (checkData != null)
                {
                    var objectId = BsonSerializer.Deserialize<Product>(checkData).Id;
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                    if (data.ProductFor != null)
                    {
                        productSKU = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("_id", objectId, null, null, "ProductDB", "Product")).ProductSKU;
                        var objectName = data.ProductFor + "-" + productSKU.Split('-')[1] + "-" + productSKU.Split('-')[2] + "-" + productSKU.Split('-')[3] + "-" + productSKU.Split('-')[4];
                        await MH.UpdateProductDetails(BsonSerializer.Deserialize<Product>(checkData).Id, data.ProductFor, "ProductFor", objectName);
                    }
                    if (data.ProductType != null)
                    {
                        productSKU = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("_id", objectId, null, null, "ProductDB", "Product")).ProductSKU;
                        var objectName = productSKU.Split('-')[0] + "-" + data.ProductType + "-" + productSKU.Split('-')[2] + "-" + productSKU.Split('-')[3] + "-" + productSKU.Split('-')[4];
                        await MH.UpdateProductDetails(BsonSerializer.Deserialize<Product>(checkData).Id, data.ProductType, "ProductType", objectName);
                    }
                    if (data.ProductDesign != null)
                    {
                        productSKU = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("_id", objectId, null, null, "ProductDB", "Product")).ProductSKU;
                        var objectName = productSKU.Split('-')[0] + "-" + productSKU.Split('-')[1] + "-" + data.ProductDesign + "-" + productSKU.Split('-')[3] + "-" + productSKU.Split('-')[4];
                        await MH.UpdateProductDetails(BsonSerializer.Deserialize<Product>(checkData).Id, data.ProductDesign, "ProductDesign", objectName);
                    }
                    if (data.ProductBrand != null)
                    {
                        var update = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductBrand", data.ProductBrand));
                    }
                    if (data.ProductPrice > 0)
                    {
                        var update = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductPrice", data.ProductPrice));
                        double discountPercentage;
                        if (data.ProductDiscount > 0)
                        {
                            discountPercentage = data.ProductDiscount;
                        }
                        else
                        {
                            discountPercentage = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("ProductSKU", productSKU, null, null, "ProductDB", "Product")).ProductDiscount;
                        }
                        var discountPrice = (data.ProductPrice - (data.ProductPrice * (discountPercentage / 100)));


                        var update1 = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductDiscountPrice", discountPrice));
                    }
                    if (data.ProductDiscount > 0)
                    {

                        var update = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductDiscount", data.ProductDiscount));
                        double price;
                        if (data.ProductPrice > 0)
                        {
                            price = data.ProductPrice;
                        }
                        else
                        {
                            price = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("ProductSKU", productSKU, null, null, "ProductDB", "Product")).ProductPrice;
                        }
                        var discountPrice = (price - (price * (data.ProductDiscount / 100)));
                        var update1 = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductDiscountPrice", discountPrice));
                    }
                    if (data.ProductStock > 0)
                    {
                        var update = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductStock", data.ProductStock));
                    }
                    if (data.ProductSize != null)
                    {
                        productSKU = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("_id", objectId, null, null, "ProductDB", "Product")).ProductSKU;
                        var objectName = productSKU.Split('-')[0] + "-" + productSKU.Split('-')[1] + "-" + productSKU.Split('-')[2] + "-" + productSKU.Split('-')[3] + "-" + data.ProductSize;
                        await MH.UpdateProductDetails(BsonSerializer.Deserialize<Product>(checkData).Id, data.ProductSize, "ProductSize", objectName);
                    }
                    if (data.ProductMaterial != null)
                    {
                        var update = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductMaterial", data.ProductMaterial));
                    }
                    if (data.ProductColour != null)
                    {
                        productSKU = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("_id", objectId, null, null, "ProductDB", "Product")).ProductSKU;
                        var objectName = productSKU.Split('-')[0] + "-" + productSKU.Split('-')[1] + "-" + productSKU.Split('-')[2] + "-" + data.ProductColour + "-" + productSKU.Split('-')[4];
                        await MH.UpdateProductDetails(BsonSerializer.Deserialize<Product>(checkData).Id, data.ProductColour, "ProductColour", objectName);
                    }
                    if (data.RefundApplicable != null)
                    {
                        var update = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("RefundApplicable", data.RefundApplicable));
                    }
                    if (data.ReplacementApplicable != null)
                    {
                        var update = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ReplacementApplicable", data.ReplacementApplicable));
                    }
                    if (data.ProductDescription != null)
                    {
                        var update = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductDescription", data.ProductDescription));
                    }
                    var MinioObject_URl = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/" + data.ProductFor + "-" + data.ProductType + "-" + data.ProductDesign + "-" + data.ProductColour + "-" + data.ProductSize + ".jpg";
                    var updateURL = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("MinioObject_URL", MinioObject_URl));
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Updated",
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "Product not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("ProductController", "Update", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Get all the reviews added for each products</summary>
        /// <response code="200">Returns reviews added for each product</response>  
        /// <response code="404">No product found</response>   
        /// <response code="400">Process ran into an exception</response>  
        [Authorize("Level1Access")]
        [HttpGet("getallreviews")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult GetAllReviews()
        {
            try
            {
                var productList = MH.GetListOfObjects(null, null, null, null, null, null, "ProductDB", "Product").Result;
                if (productList != null)
                {
                    List<ReviewsForEachProduct> reviewsList = new List<ReviewsForEachProduct>();
                    foreach (var product in productList)
                    {
                        var productData = BsonSerializer.Deserialize<Product>(product);
                        reviewsList.Add(new ReviewsForEachProduct { ProductSKU = productData.ProductSKU, ProductReviews = productData.ProductReviews });
                    }
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Success",
                        Data = reviewsList
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "No products found"
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("ProductController", "GetAllReviews", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Insert review for an product</summary>
        /// <param name="data">Details of review</param>
        /// <param name="productSKU">SKU of product for which the review is added</param>
        /// <response code="200">Review inserted successfully</response>
        /// <response code="401">Review data from body is empty</response>   
        /// <response code="402">No orders found with given id</response>     
        /// <response code="404">No product found</response>   
        /// <response code="400">Process ran into an exception</response>  
        [HttpPost("insertreview/{productSKU}")]
        [SwaggerRequestExample(typeof(Review), typeof(InsertReviewDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> InsertReview([FromBody]Review data, string productSKU)
        {
            try
            {
                var checkData = MH.CheckForDatas("ProductSKU", productSKU, null, null, "ProductDB", "Product");
                if (checkData != null)
                {
                    var objectId = BsonSerializer.Deserialize<Product>(checkData).Id;
                    if (data != null)
                    {
                        var reviews = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("_id", objectId, null, null, "ProductDB", "Product")).ProductReviews;
                        Review[] reviewArray = new Review[reviews.Length + 1];
                        data.Id = reviews.Length + 1;
                        data.Approved = false;
                        int i = 0;
                        if (reviews.Length != 0)
                        {
                            foreach (var review in reviews)
                            {
                                reviewArray[i] = review;
                                i++;
                            }
                        }
                        reviewArray[i] = data;
                        var updateReview = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("_id", objectId), "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductReviews", reviewArray));
                        var checkOrder = MH.CheckForDatas("OrderId", data.OrderId, null, null, "OrderDB", "OrderInfo");
                        if (checkOrder == null)
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "402",
                                Message = "No orders with id '" + data.OrderId + "' is found",
                                Data = null
                            });
                        }
                        var orderInfo = BsonSerializer.Deserialize<OrderInfo>(checkOrder);
                        List<ProductDetails> productDetails = new List<ProductDetails>();
                        foreach (var product in orderInfo.ProductDetails)
                        {
                            if (product.ProductSKU == productSKU)
                            {
                                product.Reviewed = true;
                            }
                            productDetails.Add(product);
                        }
                        var updateOrderInfo = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", data.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("ProductDetails", productDetails));

                        return Ok(new ResponseData
                        {
                            Code = "200",
                            Message = "Inserted",
                            Data = null
                        });
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "401",
                            Message = "Review data from body is empty",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "Product not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("ProductController", "InsertReview", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Update review for an product</summary>
        /// <param name="data">Details of review need to update</param>
        /// <param name="productSKU">SKU of product for which the review is added</param>
        /// <response code="200">Review inserted successfully</response>  
        /// <response code="401">No reviews found</response>    
        /// <response code="402">No reviews found with given id</response>  
        /// <response code="404">No product found</response>   
        /// <response code="400">Process ran into an exception</response> 
        [Authorize("Level1Access")]
        [HttpPut("updatereview/{productSKU}")]
        [SwaggerRequestExample(typeof(UpdateReview), typeof(UpdateReviewDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> UpdateReview([FromBody]UpdateReview data, string productSKU)
        {
            try
            {
                var checkData = MH.CheckForDatas("ProductSKU", productSKU, null, null, "ProductDB", "Product");
                if (checkData != null)
                {
                    var objectId = BsonSerializer.Deserialize<Product>(checkData).Id;
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                    var reviews = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("_id", objectId, null, null, "ProductDB", "Product")).ProductReviews;
                    List<Review> reviewList = new List<Review>();
                    Review[] reviewArray = new Review[reviews.Length];
                    int i = 0;
                    if (reviews.Length == 0)
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "401",
                            Message = "No reviews found"
                        });
                    }
                    else
                    {
                        foreach (var review in reviews)
                        {
                            if (review.Id == data.Id)
                            {
                                review.Approved = data.Approved;
                            }
                            reviewArray[i] = review;
                            i++;
                            reviewList.Add(review);
                        }
                    }
                    var result = reviewList.FindAll(x => x.Id == data.Id);
                    if (result.Count == 0)
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "No reviews with id '" + data.Id + "' is found"
                        });
                    }
                    var updateReview = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductReviews", reviewArray));
                    var updatedReviews = BsonSerializer.Deserialize<Product>(MH.CheckForDatas("_id", objectId, null, null, "ProductDB", "Product")).ProductReviews;
                    double numberOfReviews = 0;
                    double totalRating = 0;
                    foreach (var review in updatedReviews)
                    {
                        if (review.Approved == true)
                        {
                            numberOfReviews += 1;
                            totalRating += review.Rating;
                        }
                    }
                    double overallRating;
                    if (totalRating > 0)
                    {
                        overallRating = totalRating / numberOfReviews;
                    }
                    else
                    {
                        overallRating = 0;
                    }
                    var updateRating = await MH.UpdateSingleObject(filter, "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductRating", overallRating));
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Updated"
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "Product not found"
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("ProductController", "InsertReview", ex.Message);
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

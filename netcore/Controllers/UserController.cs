﻿using System;
using System.Threading.Tasks;
using Arthur_Clive.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using Arthur_Clive.Logger;
using AH = Arthur_Clive.Helper.AmazonHelper;
using WH = Arthur_Clive.Helper.MinioHelper;
using MH = Arthur_Clive.Helper.MongoHelper;
using System.Linq;
using Swashbuckle.AspNetCore.Examples;
using Arthur_Clive.Swagger;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson.Serialization;

namespace Arthur_Clive.Controllers
{
    /// <summary>Controller to refresh address, cart and wishlist, to get products in cart and wishlist and to get userinfo of user</summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        /// <summary></summary>
        public IMongoDatabase _db = MH._client.GetDatabase("UserInfo");
        /// <summary></summary>
        public UpdateDefinition<BsonDocument> updateDefinition;

        /// <summary>Get default address of user</summary>
        /// <param name="username">UserName of user</param>
        /// <remarks>This api is user to get default address of an user</remarks>
        /// <response code="200">Returns the default address of the user</response>
        /// <response code="400">Process ran into an exception</response>  
        [HttpGet("userinfo/{username}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> GetDefaultAddressOfUser(string username)
        {
            try
            {
                IAsyncCursor<Address> cursor = await _db.GetCollection<Address>("UserInfo").FindAsync(Builders<Address>.Filter.Eq("UserName", username) & Builders<Address>.Filter.Eq("DefaultAddress", true));
                var userInfo = cursor.ToList();
                return Ok(new ResponseData
                {
                    Code = "200",
                    Message = "Success",
                    Data = userInfo
                });
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("UserController", "GetDefaultAddressOfUser", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                }); ;
            }
        }

        /// <summary>Refresh address details of user</summary>
        /// <remarks>This api is uses to refresh userinfo of a user</remarks>
        /// <param name="data">List of address to be updated</param>
        /// <param name="username">UserName of user</param>
        /// <response code="200">Address List of user is refreshed</response>
        /// <response code="400">Process ran into an exception</response>    
        [HttpPost("userinfo/{username}")]
        [SwaggerRequestExample(typeof(AddressList), typeof(AddressDetail))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> RefreshUserInfo([FromBody]AddressList data, string username)
        {
            try
            {
                var result = _db.GetCollection<Address>("UserInfo").DeleteManyAsync(Builders<Address>.Filter.Eq("UserName", username)).Result;
                if (data.ListOfAddress.Count > 0)
                {
                    data.ListOfAddress.ToList().ForEach(c => c.UserName = username);
                    await _db.GetCollection<Address>("UserInfo").InsertManyAsync(data.ListOfAddress);
                }
                return Ok(new ResponseData
                {
                    Code = "200",
                    Message = "Inserted",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("UserController", "RefreshUserInfo", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Get products in cart of user</summary>
        /// <param name="username">UserName of user</param>
        /// <remarks>This api is user to get products in cart of an user</remarks>
        /// <response code="200">Returns the products in cart</response>
        /// <response code="400">Process ran into an exception</response> 
        [HttpGet("cart/{username}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> GetProductsInCart(string username)
        {
            try
            {
                IAsyncCursor<Cart> cursor = await _db.GetCollection<Cart>("Cart").FindAsync(Builders<Cart>.Filter.Eq("UserName", username));
                var products = cursor.ToList();
                foreach (var data in products)
                {
                    string objectName = data.ProductSKU + ".jpg";
                    //data.ObjectUrl = WH.GetMinioObject("products", objectName).Result;
                    //data.ObjectUrl = AH.GetAmazonS3Object("arthurclive-products", objectName);
                    data.MinioObject_URL = AH.GetS3Object("arthurclive-products", objectName);
                }
                return Ok(new ResponseData
                {
                    Code = "200",
                    Message = "Success",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("UserController", "GetProductsInCart", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Refresh products in cart</summary>
        /// <remarks>This api is used to refresh products in cart of the user</remarks>
        /// <param name="data">List of cart details to be updated</param>
        /// <param name="username">UserName of user</param>
        /// <response code="200">Cart of user is refreshed succeddfully</response>
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("cart/{username}")]
        [SwaggerRequestExample(typeof(CartList), typeof(CartDetail))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> RefreshCart([FromBody]CartList data, string username)
        {
            try
            {
                var result = _db.GetCollection<Cart>("Cart").DeleteManyAsync(Builders<Cart>.Filter.Eq("UserName", username)).Result;
                if (data.ListOfProducts.Count > 0)
                {
                    data.ListOfProducts.ToList().ForEach(c => c.UserName = username);
                    await _db.GetCollection<Cart>("Cart").InsertManyAsync(data.ListOfProducts);
                }
                return Ok(new ResponseData
                {
                    Code = "200",
                    Message = "Inserted",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("UserController", "RefreshCart", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Get products in wishlist of user</summary>
        /// <param name="username">UserName of user</param>
        /// <remarks>This api is user to get products in wishlist of an user</remarks>
        /// <response code="200">Returns the products in wishlist</response>
        /// <response code="400">Process ran into an exception</response> 
        [HttpGet("wishlist/{username}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> GetProductsInWishList(string username)
        {
            try
            {
                IAsyncCursor<WishList> cursor = await _db.GetCollection<WishList>("WishList").FindAsync(Builders<WishList>.Filter.Eq("UserName", username));
                var products = cursor.ToList();
                foreach (var data in products)
                {
                    string objectName = data.ProductSKU + ".jpg";
                    //data.ObjectUrl = WH.GetMinioObject("products", objectName).Result;
                    //data.ObjectUrl = AH.GetAmazonS3Object("arthurclive-products", objectName);
                    data.MinioObject_URL = AH.GetS3Object("arthurclive-products", objectName);
                }
                return Ok(new ResponseData
                {
                    Code = "200",
                    Message = "Success",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("UserController", "GetProductsInWishList", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Refresh products in wishlist</summary>
        /// <remarks>This api is used to refresh products in wishlist of the user</remarks>
        /// <param name="data">List of wishlist details to be updated</param>
        /// <param name="username">UserName of user</param>
        /// <response code="200">Wishlist of user is refreshed successfully</response>
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("wishlist/{username}")]
        [SwaggerRequestExample(typeof(WishlistList), typeof(WishlistDetail))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> RefreshWishList([FromBody]WishlistList data, string username)
        {
            try
            {
                var result = _db.GetCollection<WishList>("WishList").DeleteManyAsync(Builders<WishList>.Filter.Eq("UserName", username)).Result;
                if (data.ListOfProducts.Count > 0)
                {
                    data.ListOfProducts.ToList().ForEach(c => c.UserName = username);
                    await _db.GetCollection<WishList>("WishList").InsertManyAsync(data.ListOfProducts);
                }
                return Ok(new ResponseData
                {
                    Code = "200",
                    Message = "Inserted",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("UserController", "RefreshWishList", ex.Message);
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

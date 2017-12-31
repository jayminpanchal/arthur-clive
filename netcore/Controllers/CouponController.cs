using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arthur_Clive.Data;
using Arthur_Clive.Logger;
using Arthur_Clive.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Examples;
using MH = Arthur_Clive.Helper.MongoHelper;

namespace Arthur_Clive.Controllers
{
    /// <summary>Controller to handle request based on coupon</summary>
    [Route("api/[controller]")]
    public class CouponController : Controller
    {
        /// <summary></summary>
        public IMongoDatabase _db = MH._client.GetDatabase("CouponDB");

        /// <summary>Get all the coupons added to DB</summary>
        /// <response code="200">Returns all the coupons found on db</response>
        /// <response code="404">No coupons found</response>  
        /// <response code="400">Process ran into an exception</response>  
        [Authorize("Level1Access")]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult GetAllCoupon()
        {
            try
            {
                var getCoupons = MH.GetListOfObjects(null, null, null, null, null, null, "CouponDB", "Coupon").Result;
                if (getCoupons != null)
                {
                    List<Coupon> couponList = new List<Coupon>();
                    foreach (var coupon in getCoupons)
                    {
                        couponList.Add(BsonSerializer.Deserialize<Coupon>(coupon));
                    }
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Success",
                        Data = couponList
                    });
                }
                else
                {
                    return Ok(new ResponseData
                    {
                        Code = "404",
                        Message = "No coupons found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("CouponController", "GetAllCoupon", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Insert coupon</summary>
        /// <remarks>This api is used to insert a new coupon</remarks>
        /// <param name="data">Coupon data to be inserted</param>
        /// <response code="200">Coupon inserted successfully</response>
        /// <response code="401">Coupon already added</response>  
        /// <response code="400">Process ran into an exception</response>  
        [Authorize("Level1Access")]
        [HttpPost]
        [SwaggerRequestExample(typeof(Coupon), typeof(CouponData))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> InsertCoupon([FromBody]Coupon data)
        {
            try
            {
                if (MH.CheckForDatas("Code", data.Code, null, null, "CouponDB", "Coupon") == null)
                {
                    await _db.GetCollection<Coupon>("Coupon").InsertOneAsync(data);
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Coupon inserted successfully",
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "401",
                        Message = "Coupon already added",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("CouponController", "InsertCoupon", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Check if coupon is valid or not</summary>
        /// <remarks>This api is used to check if the coupon is valid</remarks>
        /// <param name="username">UserName of username</param>
        /// <param name="code">Coupon code</param>
        /// <response code="200">Coupon is valid</response>
        /// <response code="401">Coupon expired</response>  
        /// <response code="402">Coupon invalid</response> 
        /// <response code="403">Coupon invalid</response> 
        /// <response code="404">Coupon not found</response>  
        /// <response code="400">Process ran into an exception</response> 
        [HttpGet("check/{username}/{code}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult CheckCoupon(string username, string code)
        {
            try
            {
                var checkData = MH.CheckForDatas("Code", code, null, null, "CouponDB", "Coupon");
                if (checkData != null)
                {
                    var data = BsonSerializer.Deserialize<Coupon>(checkData);
                    if (data.ExpiryTime > DateTime.UtcNow)
                    {
                        if (data.ApplicableFor == "All" || data.ApplicableFor == username)
                        {
                            if (data.UsageCount != 0)
                            {
                                return Ok(new ResponseData
                                {
                                    Code = "200",
                                    Message = "Coupon is valid",
                                    Data = new CouponContent { Value = data.Value, Percentage = data.Percentage }
                                });
                            }
                            else
                            {
                                return BadRequest(new ResponseData
                                {
                                    Code = "403",
                                    Message = "Coupon usage limit exceeded",
                                    Data = null
                                });
                            }
                        }
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "402",
                                Message = "Coupon invalid",
                                Data = null
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "401",
                            Message = "Coupon expired",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "Coupon not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("CouponController", "CheckCoupon", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Update coupon</summary>
        /// <remarks>This api is used to use coupon and update coupon details</remarks>
        /// <param name="data">Coupon details</param>
        /// <param name="code">Coupon code</param>
        /// <response code="200">Coupon updated successfully</response>
        /// <response code="404">Coupon not found</response>  
        /// <response code="400">Process ran into an exception</response> 
        [HttpPut("{code}")]
        [SwaggerRequestExample(typeof(UseCoupon), typeof(UseCouponData))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult UseCoupon([FromBody]UseCoupon data, string code)
        {
            try
            {
                var checkData = MH.CheckForDatas("Code", code, null, null, "CouponDB", "Coupon");
                if (checkData != null)
                {
                    var result = BsonSerializer.Deserialize<Coupon>(checkData);
                    var filter = Builders<BsonDocument>.Filter.Eq("Code", code);
                    if (data.UsageCount > 0)
                    {
                        var coupon = BsonSerializer.Deserialize<Coupon>(MH.CheckForDatas("Code", code, null, null, "CouponDB", "Coupon"));
                        if (result.Percentage == true)
                        {
                            var update = MH.UpdateSingleObject(filter, "CouponDB", "Coupon", Builders<BsonDocument>.Update.Set("UsageCount", result.UsageCount - data.UsageCount));
                        }
                        else
                        {
                            if (data.Amount > coupon.Value)
                            {
                                return Ok(new ResponseData
                                {
                                    Code = "401",
                                    Message = "Amount is higher than the coupon value",
                                    Data = null
                                });
                            }
                            else
                            {
                                var balance = coupon.Value - data.Amount;
                                var update = MH.UpdateSingleObject(filter, "CouponDB", "Coupon", Builders<BsonDocument>.Update.Set("Value", balance));
                                if (balance == 0)
                                {
                                    var updateUsageCount = MH.UpdateSingleObject(filter, "CouponDB", "Coupon", Builders<BsonDocument>.Update.Set("UsageCount", 0));
                                }
                            }
                        }
                    }
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Coupon updated successfully",
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "Coupon not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("CouponController", "UseCoupon", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Update coupon</summary>
        /// <remarks>This api is used to update coupon details</remarks>
        /// <param name="data">Coupon details</param>
        /// <param name="code">Coupon code</param>
        /// <response code="200">Coupon updated successfully</response>
        /// <response code="404">Coupon not found</response>  
        /// <response code="400">Process ran into an exception</response> 
        [Authorize("Level1Access")]
        [HttpPut("update/{code}")]
        [SwaggerRequestExample(typeof(UpdateCoupon), typeof(CouponUpdateData))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult UpdateCoupon([FromBody]UpdateCoupon data, string code)
        {
            try
            {
                var checkData = MH.CheckForDatas("Code", code, null, null, "CouponDB", "Coupon");
                if (checkData != null)
                {
                    var result = BsonSerializer.Deserialize<Coupon>(checkData);
                    var filter = Builders<BsonDocument>.Filter.Eq("Code", code);
                    if (data.ApplicableFor != null)
                    {
                        var update = MH.UpdateSingleObject(filter, "CouponDB", "Coupon", Builders<BsonDocument>.Update.Set("ApplicableFor", data.ApplicableFor));
                    }
                    if (data.ExpiryTime != null)
                    {
                        var update = MH.UpdateSingleObject(filter, "CouponDB", "Coupon", Builders<BsonDocument>.Update.Set("ExpiryTime", data.ExpiryTime));
                    }
                    if(data.Value > 0)
                    {
                        var update = MH.UpdateSingleObject(filter, "CouponDB", "Coupon", Builders<BsonDocument>.Update.Set("Value", data.Value));
                    }
                    if(data.Percentage != null)
                    {
                        var updateResult = MH.UpdateSingleObject(filter, "CouponDB", "Coupon", Builders<BsonDocument>.Update.Set("Percentage", data.Percentage));
                    }
                    if(data.UsageCount > 0)
                    {
                        var update = MH.UpdateSingleObject(filter, "CouponDB", "Coupon", Builders<BsonDocument>.Update.Set("UsageCount", data.UsageCount));
                    }                    
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Coupon updated successfully",
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "Coupon not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("CouponController", "UpdateCoupon", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = ex.Message
                });
            }
        }

        /// <summary>Delete a coupon</summary>
        /// <param name="code">Code of coupon</param>
        /// <response code="200">Coupon deleted successfully</response>
        /// <response code="404">Coupon not found</response>  
        /// <response code="400">Process ran into an exception</response> 
        [Authorize("Level1Access")]
        [HttpDelete("{code}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult DeleteCoupon(string code)
        {
            try
            {
                var checkData = MH.CheckForDatas("Code", code, null, null, "CouponDB", "Coupon");
                if (checkData != null)
                {
                    var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("Code",code),"CouponDB","Coupon");
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Coupon deleted successfully",
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "Coupon not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("CouponController", "UpdateCoupon", ex.Message);
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

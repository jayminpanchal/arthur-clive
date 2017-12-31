using Microsoft.AspNetCore.Mvc;
using Arthur_Clive.Helper;
using System;
using Arthur_Clive.Data;
using MH = Arthur_Clive.Helper.MongoHelper;
using PU = Arthur_Clive.Helper.PayUHelper;
using Arthur_Clive.Logger;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Arthur_Clive.Controllers
{
    /// <summary>Controller to make payment using PayUMoney and get return responce</summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        /// <summary></summary>
        public IMongoDatabase _db = MH._client.GetDatabase("UserInfo");
        /// <summary></summary>
        public IMongoDatabase product_db = MH._client.GetDatabase("ProductDB");
        /// <summary></summary>
        public IMongoDatabase order_db = MH._client.GetDatabase("OrderDB");

        /// <summary>Success responce from the PayU payment gateway</summary>
        /// <param name="paymentResponse">Responce data from PayU</param>
        [HttpPost("success")]
        public async Task<ActionResult> PaymentSuccess(IFormCollection paymentResponse)
        {
            if (paymentResponse != null)
            {
                PaymentModel paymentModel = new PaymentModel
                {
                    Email = paymentResponse["email"],
                    OrderId = Convert.ToInt16(paymentResponse["udf1"]),
                    UserName = paymentResponse["udf2"],
                    ProductInfo = paymentResponse["productinfo"],
                    FirstName = paymentResponse["firstname"],
                    Amount = paymentResponse["amount"],
                };
                try
                {
                    if (PU.Generatehash512(PU.GetReverseHashString(paymentResponse["txnid"], paymentModel)) == paymentResponse["hash"])
                    {
                        var updatePaymentMethod = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentMethod", paymentResponse["mode"].ToString()));
                        var updatePaymentDetails = await MH.UpdatePaymentDetails(paymentModel.OrderId);
                        var removeCartItems = MH.RemoveCartItems(paymentModel.OrderId, paymentModel.UserName, paymentModel.Email);
                        var sendGift = GlobalHelper.SendGift(paymentModel.OrderId);
                        var sendProductDetails = EmailHelper.SendEmail_ProductDetails(paymentModel.Email, paymentModel.OrderId);
                        return Redirect(GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("redirectsuccess").First().Value);
                    }
                    else
                    {
                        var updatePaymentMethod = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentMethod", paymentResponse["mode"].ToString()));
                        PaymentMethod paymentDetails = new PaymentMethod();
                        List<StatusCode> statusCodeList = new List<StatusCode>();
                        var orderData = BsonSerializer.Deserialize<OrderInfo>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo").Result);
                        foreach (var detail in orderData.PaymentDetails.Status)
                        {
                            statusCodeList.Add(detail);
                        }
                        statusCodeList.Add(new StatusCode { StatusId = 3, Description = "Payment Failed", Date = DateTime.UtcNow });
                        paymentDetails.Status = statusCodeList;
                        var updatePaymentDetails = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentDetails", paymentDetails));
                        return Redirect(GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("redirectfailure").First().Value);
                    }

                }
                catch (Exception ex)
                {
                    LoggerDataAccess.CreateLog("PaymentController", "PaymentSuccess", ex.Message);
                    var updatePaymentMethod = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentMethod", paymentResponse["mode"].ToString()));
                    PaymentMethod paymentDetails = new PaymentMethod();
                    List<StatusCode> statusCodeList = new List<StatusCode>();
                    var orderData = BsonSerializer.Deserialize<OrderInfo>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo").Result);
                    foreach (var detail in orderData.PaymentDetails.Status)
                    {
                        statusCodeList.Add(detail);
                    }
                    statusCodeList.Add(new StatusCode { StatusId = 3, Description = "Payment Failed", Date = DateTime.UtcNow });
                    paymentDetails.Status = statusCodeList;
                    var updatePaymentDetails = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentDetails", paymentDetails));
                    return Redirect(GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("redirectfailure").First().Value);
                }
            }
            else
            {
                return Redirect(GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("redirectfailure").First().Value);
            }
        }

        /// <summary>Failure responce from the PayU payment gateway</summary>
        /// <param name="paymentResponse">Responce data from PayU</param>
        [HttpPost("failed")]
        public async Task<ActionResult> PaymentFailed(IFormCollection paymentResponse)
        {
            if (paymentResponse != null)
            {
                string responseHash = paymentResponse["hash"];
                PaymentModel paymentModel = new PaymentModel
                {
                    Email = paymentResponse["email"],
                    OrderId = Convert.ToInt16(paymentResponse["udf1"]),
                    UserName = paymentResponse["udf2"],
                    ProductInfo = paymentResponse["productinfo"],
                    FirstName = paymentResponse["firstname"],
                    Amount = paymentResponse["amount"],
                };
                var updatePaymentMethod = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentMethod", paymentResponse["mode"].ToString()));
                PaymentMethod paymentDetails = new PaymentMethod();
                List<StatusCode> statusCodeList = new List<StatusCode>();
                var orderData = BsonSerializer.Deserialize<OrderInfo>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo").Result);
                foreach (var detail in orderData.PaymentDetails.Status)
                {
                    statusCodeList.Add(detail);
                }
                statusCodeList.Add(new StatusCode { StatusId = 3, Description = "Payment Failed", Date = DateTime.UtcNow });
                paymentDetails.Status = statusCodeList;
                var updatePaymentDetails = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentDetails", paymentDetails));
                return Redirect(GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("redirectfailure").First().Value);
            }
            else
            {
                return Redirect(GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("redirectfailure").First().Value);
            }
        }

        /// <summary>Cancel responce from the PayU payment gateway</summary>
        /// <param name="paymentResponse">Responce data from PayU</param>
        [HttpPost("cancel")]
        public async Task<ActionResult> Paymentcancelled(IFormCollection paymentResponse)
        {
            if (paymentResponse != null)
            {
                string responseHash = paymentResponse["hash"];
                PaymentModel paymentModel = new PaymentModel
                {
                    Email = paymentResponse["email"],
                    OrderId = Convert.ToInt16(paymentResponse["udf1"]),
                    UserName = paymentResponse["udf2"],
                    ProductInfo = paymentResponse["productinfo"],
                    FirstName = paymentResponse["firstname"],
                    Amount = paymentResponse["amount"],
                };
                var updatePaymentMethod = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentMethod", paymentResponse["mode"].ToString()));
                PaymentMethod paymentDetails = new PaymentMethod();
                List<StatusCode> statusCodeList = new List<StatusCode>();
                var orderData = BsonSerializer.Deserialize<OrderInfo>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo").Result);
                foreach (var detail in orderData.PaymentDetails.Status)
                {
                    statusCodeList.Add(detail);
                }
                statusCodeList.Add(new StatusCode { StatusId = 4, Description = "Payment Cancelled", Date = DateTime.UtcNow });
                paymentDetails.Status = statusCodeList;
                var updatePaymentDetails = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", paymentModel.OrderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentDetails", paymentDetails));
                return Redirect(GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("redirectcancelled").First().Value);
            }
            else
            {
                return Redirect(GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("redirectcancelled").First().Value);
            }
        }

    }
}
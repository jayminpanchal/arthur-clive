using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Arthur_Clive.Data;
using Arthur_Clive.Logger;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Arthur_Clive.Helper
{
    /// <summary>Helper method for Amazon SES service</summary>
    public class EmailHelper
    {
        /// <summary>Get Amazon SES credentials from xml file</summary>
        /// <param name="key"></param>
        public static string GetCredentials(string key)
        {
            try
            {
                var xx = GlobalHelper.ReadXML();
                var result = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("Yes")).Descendants(key);
                return result.First().Value;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "GetCredentials", ex.Message);
                return "Failed";
            }
        }

        /// <summary>Send email using Amazon SES service</summary>
        /// <param name="fullname"></param>
        /// <param name="emailReceiver"></param>
        /// <param name="message"></param>
        public static async Task<string> SendEmail_ToSubscribedUsers(string fullname, string emailReceiver, string message)
        {
            string emailSender = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsender").First().Value;
            string link = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("websitelink").First().Value;
            using (var client = new AmazonSimpleEmailServiceClient(GetCredentials("accesskey"), GetCredentials("secretkey"), Amazon.RegionEndpoint.USWest2))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = emailSender,
                    Destination = new Destination { ToAddresses = new List<string> { emailReceiver } },
                    Message = new Message
                    {
                        Subject = new Content(GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsubject1").First().Value),
                        Body = new Body
                        {
                            Html = new Content(CreateEmailBody_SendMessageToSubscribedUsers(fullname, "<a href ='" + link + "'>Click Here</a>", message))
                        }
                    }
                };
                try
                {
                    var responce = await client.SendEmailAsync(sendRequest);
                    return "Success";
                }
                catch (Exception ex)
                {
                    LoggerDataAccess.CreateLog("EmailHelper", "SendEmail_ToSubscribedUsers", ex.Message);
                    return "Failed";
                }
            }
        }

        /// <summary>Create email body</summary>
        /// <param name="fullname"></param>
        /// <param name="link"></param>
        /// <param name="message"></param>
        public static string CreateEmailBody_SendMessageToSubscribedUsers(string fullname, string link, string message)
        {
            try
            {
                string emailBody;
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(dir, "EmailTemplate/MessageFromAdmin.html");
                using (StreamReader reader = File.OpenText(path))
                {
                    emailBody = reader.ReadToEnd();
                }
                emailBody = emailBody.Replace("{FullName}", fullname);
                emailBody = emailBody.Replace("{Message}", message);
                emailBody = emailBody.Replace("{Link}", link);
                return emailBody;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "CreateEmailBody_SendMessageToSubscribedUsers", ex.Message);
                return "Failed";
            }
        }

        /// <summary>Send email to admin is the orders product quantity is higher than the product stock</summary>
        public static async Task<string> SendEmailToAdmin(string userName, string email, string productInfo, long orderQuantity, long productStock, long orderId)
        {
            string emailSender = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsender").First().Value;
            string emailReceiver = email;
            string link = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("websitelink").First().Value;
            string emailSubject = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsubject3").First().Value + "OrderId : " + orderId + "&" + "UserName : " + userName;
            using (var client = new AmazonSimpleEmailServiceClient(GetCredentials("accesskey"), GetCredentials("secretkey"), Amazon.RegionEndpoint.USWest2))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = emailSender,
                    Destination = new Destination { ToAddresses = new List<string> { emailReceiver } },
                    Message = new Message
                    {
                        Subject = new Content(emailSubject),
                        Body = new Body
                        {
                            Html = new Content(CreateEmailBody_ErrorReport(userName, productInfo, orderQuantity, productStock, orderId.ToString()))
                        }
                    }
                };
                try
                {
                    var responce = await client.SendEmailAsync(sendRequest);
                    return "Success";
                }
                catch (Exception ex)
                {
                    LoggerDataAccess.CreateLog("EmailHelper", "SendEmail", ex.Message);
                    return "Failed";
                }
            }
        }

        /// <summary>Create email body to be sent to admin reporting a problem</summary>
        /// <param name="userName"></param>
        /// <param name="productInfo"></param>
        /// <param name="orderQuantity"></param>
        /// <param name="productStock"></param>
        /// <param name="orderId"></param>
        public static string CreateEmailBody_ErrorReport(string userName, string productInfo, long orderQuantity, long productStock, string orderId)
        {
            try
            {
                string emailBody;
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(dir, "EmailTemplate/ErrorReport.html");
                using (StreamReader reader = File.OpenText(path))
                {
                    emailBody = reader.ReadToEnd();
                }
                emailBody = emailBody.Replace("{OrderId}", orderId);
                emailBody = emailBody.Replace("{UserName}", userName);
                emailBody = emailBody.Replace("{ProductInfo}", productInfo);
                emailBody = emailBody.Replace("{OrderQuantity}", orderQuantity.ToString());
                emailBody = emailBody.Replace("{ProductStock}", productStock.ToString());
                return emailBody;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "CreateEmailBody_ErrorReport", ex.Message);
                return "Failed";
            }
        }

        /// <summary>Send gift by email</summary>
        /// <param name="orderId">Id of order</param>
        /// <param name="productInfo">Info of product</param>
        public static async Task<string> SendGift(long orderId, string productInfo)
        {
            try
            {
                var productArray = productInfo.Split(":");
                var checkOrder = MongoHelper.GetSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", orderId), "OrderDB", "OrderInfo").Result;
                if (checkOrder != null)
                {
                    var orderInfo = BsonSerializer.Deserialize<OrderInfo>(checkOrder);
                    foreach (var product in productArray)
                    {
                        if (product.Contains("Gifts"))
                        {
                            foreach (var info in orderInfo.ProductDetails)
                            {
                                if (product == info.ProductSKU)
                                {
                                    Random generator = new Random();
                                    var couponCode = "CU" + generator.Next(0, 1000000).ToString("D6");
                                    Coupon coupon = new Coupon
                                    {
                                        Code = couponCode,
                                        ApplicableFor = "All",
                                        UsageCount = 1,
                                        Percentage = false,
                                        Value = info.ProductInCart.ProductPrice,
                                        ExpiryTime = DateTime.UtcNow.AddYears(10)
                                    };
                                    //Insert coupon to db
                                    await MongoHelper._client.GetDatabase("CouponDB").GetCollection<Coupon>("Coupon").InsertOneAsync(coupon);
                                    var user = MongoHelper.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", orderInfo.UserName), "Authentication", "Authentication").Result;
                                    if (user == null)
                                    {
                                        return "User not found";
                                    }
                                    var userData = BsonSerializer.Deserialize<RegisterModel>(user);
                                    string emailSender = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsender").First().Value;
                                    using (var client = new AmazonSimpleEmailServiceClient(GetCredentials("accesskey"), GetCredentials("secretkey"), Amazon.RegionEndpoint.USWest2))
                                    {
                                        var sendRequest = new SendEmailRequest
                                        {
                                            Source = emailSender,
                                            Destination = new Destination { ToAddresses = new List<string> { info.ProductInCart.ProductFor } },
                                            Message = new Message
                                            {
                                                Subject = new Content(GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsubject4").First().Value),
                                                Body = new Body
                                                {
                                                    Html = new Content(CreateEmailBody_SendGiftCard(info.ProductInCart.ProductPrice.ToString(), couponCode, info.ProductInCart.ProductDescription, userData.FullName, info.ProductSKU))
                                                }
                                            }
                                        };
                                        var responce = await client.SendEmailAsync(sendRequest);
                                    }
                                }
                            }
                        }
                    }
                    return "Success";
                }
                else
                {
                    return "Order info not found";
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "SendGiftCard", ex.Message);
                return "Failed";
            }
        }

        /// <summary>Create email body to send gift through email</summary>
        /// <param name="value"></param>
        /// <param name="couponCode"></param>
        /// <param name="message"></param>
        /// <param name="fullName"></param>
        /// <param name="objectName"></param>
        public static string CreateEmailBody_SendGiftCard(string value, string couponCode, string message, string fullName, string objectName)
        {
            try
            {
                string emailBody;
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(dir, "EmailTemplate/ECard.html");
                using (StreamReader reader = File.OpenText(path))
                {
                    emailBody = reader.ReadToEnd();
                }
                List<string> matchedString = GlobalHelper.StringBetweenTwoCharacters(objectName, "All-Gifts-", "-NA-");
                var replaceName = "You have received an e-gift from " + fullName;
                var imageUrl = "https://s3.ap-south-1.amazonaws.com/acemailtemplate/Gift-Banner-" + matchedString[0] + ".jpg";
                var replaceImage = "<img src=" + imageUrl + " width='600' height='124'>";
                var replacedValue = "Value of gift is &#8377;" + value;
                emailBody = emailBody.Replace("{GiftValue}", replacedValue);
                emailBody = emailBody.Replace("{CouponCode}", couponCode);
                emailBody = emailBody.Replace("{Message}", message);
                emailBody = emailBody.Replace("{FullName}", replaceName);
                emailBody = emailBody.Replace("{Image}", replaceImage);
                return emailBody;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "CreateEmailBody_SendGiftCard", ex.Message);
                return "Failed";
            }
        }

        /// <summary>Send email to user who subscribes for news letter service</summary>
        /// <param name="emailReceiver"></param>
        public static async Task<string> SendEmail_NewsLetterService(string emailReceiver)
        {
            try
            {
                string emailSender = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsender").First().Value;
                using (var client = new AmazonSimpleEmailServiceClient(GetCredentials("accesskey"), GetCredentials("secretkey"), Amazon.RegionEndpoint.USWest2))
                {
                    var sendRequest = new SendEmailRequest
                    {
                        Source = emailSender,
                        Destination = new Destination { ToAddresses = new List<string> { emailReceiver } },
                        Message = new Message
                        {
                            Subject = new Content(GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsubject5").First().Value),
                            Body = new Body
                            {
                                Html = new Content(CreateEmailBody_NewsLetterService())
                            }
                        }
                    };
                    var responce = await client.SendEmailAsync(sendRequest);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "SendEmail_NewsLetterService", ex.Message);
                return "Failed";
            }
        }

        /// <summary>Create email body to send email to user who subscribes for news letter service</summary>
        public static string CreateEmailBody_NewsLetterService()
        {
            try
            {
                string emailBody;
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(dir, "EmailTemplate/Newsletter.html");
                using (StreamReader reader = File.OpenText(path))
                {
                    emailBody = reader.ReadToEnd();
                }
                return emailBody;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "CreateEmailBody_NewsLetterService", ex.Message);
                return "Failed";
            }
        }

        /// <summary>Send email to user who subscribes for news letter service</summary>
        /// <param name="emailReceiver"></param>
        /// <param name="orderId"></param>
        public static async Task<string> SendEmail_ProductDetails(string emailReceiver, long orderId)
        {
            try
            {
                string emailSender = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsender").First().Value;
                var orderInfo = BsonSerializer.Deserialize<OrderInfo>(MongoHelper.CheckForDatas("OrderId", orderId, null, null, "OrderDB", "OrderInfo"));
                foreach (var product in orderInfo.ProductDetails)
                {
                    Address billingAddress = new Address();
                    Address deliveryAddress = new Address();
                    foreach (var address in orderInfo.Address)
                    {
                        if (address.BillingAddress == true)
                        {
                            billingAddress = address;
                        }
                        if (address.ShippingAddress == true)
                        {
                            deliveryAddress = address;
                        }
                    }
                    using (var client = new AmazonSimpleEmailServiceClient(GetCredentials("accesskey"), GetCredentials("secretkey"), Amazon.RegionEndpoint.USWest2))
                    {
                        var sendRequest = new SendEmailRequest
                        {
                            Source = emailSender,
                            Destination = new Destination { ToAddresses = new List<string> { emailReceiver } },
                            Message = new Message
                            {
                                Subject = new Content(GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsubject6").First().Value),
                                Body = new Body
                                {
                                    Html = new Content(CreateEmailBody_ProductDetails(orderInfo, product.ProductSKU, billingAddress, deliveryAddress, product))
                                }
                            }
                        };
                        var responce = await client.SendEmailAsync(sendRequest);
                    }
                }
                return "Success";
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "SendEmail_ProductDetails", ex.Message);
                return "Failed";
            }
        }

        /// <summary>Create email body to send email to user who subscribes for news letter service</summary>
        /// <param name="orderInfo"></param>
        /// <param name="productSKU"></param>
        /// <param name="billingAddress"></param>
        /// <param name="deliveryAddress"></param>
        /// <param name="productDetails"></param>
        public static string CreateEmailBody_ProductDetails(OrderInfo orderInfo, string productSKU, Address billingAddress, Address deliveryAddress, ProductDetails productDetails)
        {
            try
            {
                string emailBody;
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(dir, "EmailTemplate/OrderDetails.html");
                using (StreamReader reader = File.OpenText(path))
                {
                    emailBody = reader.ReadToEnd();
                }
                var imageUrl = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/" + productSKU + ".jpg";
                var replaceImage = "<img src=" + imageUrl + " width='250' height='300'>";
                var colourCircle = "<span style=\"border-radius:50%; border-style:double; padding:20px; background-color:" + productDetails.ProductInCart.ProductColour + "; display:block; width:20px; height:20px\">";
                var replaceSize = "";
                if (productDetails.ProductInCart.ProductSize.Contains("_"))
                {
                    replaceSize = productDetails.ProductInCart.ProductSize.Replace("_", "-");
                }
                else
                {
                    replaceSize = productDetails.ProductInCart.ProductSize;
                }
                emailBody = emailBody.Replace("{BA_AddressLines}", billingAddress.AddressLines);
                emailBody = emailBody.Replace("{BA_City}", billingAddress.City);
                emailBody = emailBody.Replace("{BA_State}", billingAddress.State);
                emailBody = emailBody.Replace("{BA_PinCode}", billingAddress.PinCode);
                emailBody = emailBody.Replace("{BA_Country}", billingAddress.Country);
                emailBody = emailBody.Replace("{DA_AddressLines}", deliveryAddress.AddressLines);
                emailBody = emailBody.Replace("{DA_City}", deliveryAddress.City);
                emailBody = emailBody.Replace("{DA_State}", deliveryAddress.State);
                emailBody = emailBody.Replace("{DA_PinCode}", deliveryAddress.PinCode);
                emailBody = emailBody.Replace("{DA_Country}", deliveryAddress.Country);
                emailBody = emailBody.Replace("{Invoice Number}", orderInfo.OrderId.ToString());
                emailBody = emailBody.Replace("{Order Number}", "ACODR-" + orderInfo.OrderId.ToString());
                emailBody = emailBody.Replace("{Order Date}", productDetails.StatusCode[0].Date.Date.ToString());
                emailBody = emailBody.Replace("{Payment Method}", orderInfo.PaymentMethod);
                emailBody = emailBody.Replace("{Image}", replaceImage);
                emailBody = emailBody.Replace("{P_For}", productDetails.ProductInCart.ProductFor);
                emailBody = emailBody.Replace("{P_Type}", productDetails.ProductInCart.ProductType);
                emailBody = emailBody.Replace("{P_Tshirt}", productDetails.ProductInCart.ProductDesign);
                emailBody = emailBody.Replace("{Colour_Image}", colourCircle);
                emailBody = emailBody.Replace("{P_Size}", replaceSize);
                emailBody = emailBody.Replace("{Quantity}", productDetails.ProductInCart.ProductQuantity.ToString());
                emailBody = emailBody.Replace("{UnitPrice}", productDetails.ProductInCart.ProductDiscountPrice.ToString());
                emailBody = emailBody.Replace("{ItemTotal}", (productDetails.ProductInCart.ProductDiscountPrice * productDetails.ProductInCart.ProductQuantity).ToString());
                emailBody = emailBody.Replace("{Tax}", (((productDetails.ProductInCart.ProductPrice * productDetails.ProductInCart.ProductQuantity * 5) / 100)).ToString());
                emailBody = emailBody.Replace("{Discount}", productDetails.ProductInCart.ProductDiscount.ToString());
                emailBody = emailBody.Replace("{GrandTotal}", ((productDetails.ProductInCart.ProductPrice * productDetails.ProductInCart.ProductQuantity) + (((productDetails.ProductInCart.ProductPrice * productDetails.ProductInCart.ProductQuantity * 5) / 100))).ToString());
                return emailBody;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "CreateEmailBody_ProductDetails", ex.Message);
                return "Failed";
            }
        }
    }
}

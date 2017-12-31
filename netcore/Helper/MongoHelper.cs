using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Arthur_Clive.Data;
using Arthur_Clive.Logger;
using MongoDB.Bson;
using MongoDB.Driver;
using AH = Arthur_Clive.Helper.AmazonHelper;
using WH = Arthur_Clive.Helper.MinioHelper;
using MH = Arthur_Clive.Helper.MongoHelper;
using MongoDB.Bson.Serialization;

namespace Arthur_Clive.Helper
{
    /// <summary>Helper method for MongoDB</summary>
    public class MongoHelper
    {
        /// <summary></summary>
        public static IMongoDatabase _mongodb;
        /// <summary>Client for MongoDB</summary>
        public static MongoClient _client = GetClient();
        /// <summary></summary>
        public static FilterDefinition<BsonDocument> filter;
        /// <summary>Get client for MongoDB</summary>
        public static MongoClient GetClient()
        {
            try
            {
                var ip = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("ip").First().Value;
                var user = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("user").First().Value;
                var password = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("password").First().Value;
                var db = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("db").First().Value;
                var connectionString = "mongodb://" + user + ":" + password + "@" + ip + ":27017/" + db;
                var mongoClient = new MongoClient(connectionString);
                return mongoClient;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "GetClient", ex.Message);
                return null;
            }
        }

        /// <summary>Get single object from MongoDB</summary>
        /// <param name="filter"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        public static async Task<BsonDocument> GetSingleObject(FilterDefinition<BsonDocument> filter, string dbName, string collectionName)
        {
            try
            {
                _mongodb = _client.GetDatabase(dbName);
                var collection = _mongodb.GetCollection<BsonDocument>(collectionName);
                IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter);
                return cursor.FirstOrDefault();
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "GetSingleObject", ex.Message);
                return null;
            }
        }

        /// <summary>Get list of objects from MongoDB</summary>
        /// <param name="filterField1"></param>
        /// <param name="filterField2"></param>
        /// <param name="filterField3"></param>
        /// <param name="filterData1"></param>
        /// <param name="filterData2"></param>
        /// <param name="filterData3"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        public static async Task<List<BsonDocument>> GetListOfObjects(string filterField1, dynamic filterData1, string filterField2, dynamic filterData2, string filterField3, dynamic filterData3, string dbName, string collectionName)
        {
            try
            {
                var db = _client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>(collectionName);
                if (filterField1 == null & filterField2 == null & filterField3 == null)
                {
                    filter = FilterDefinition<BsonDocument>.Empty;
                }
                else if (filterField1 != null & filterField2 == null & filterField3 == null)
                {
                    filter = Builders<BsonDocument>.Filter.Eq(filterField1, filterData1);
                }
                else if (filterField1 != null & filterField2 != null & filterField3 == null)
                {
                    filter = Builders<BsonDocument>.Filter.Eq(filterField1, filterData1) & Builders<BsonDocument>.Filter.Eq(filterField2, filterData2);
                }
                else if (filterField1 != null & filterField2 != null & filterField3 != null)
                {
                    filter = Builders<BsonDocument>.Filter.Eq(filterField1, filterData1) & Builders<BsonDocument>.Filter.Eq(filterField2, filterData2) & Builders<BsonDocument>.Filter.Eq(filterField3, filterData3);
                }
                IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter);
                return cursor.ToList();
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "GetListOfObjects", ex.Message);
                return null;
            }
        }

        /// <summary>Update single object in MongoDB </summary>
        /// <param name="filter"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="update"></param>
        public static async Task<bool?> UpdateSingleObject(FilterDefinition<BsonDocument> filter, string dbName, string collectionName, UpdateDefinition<BsonDocument> update)
        {
            try
            {
                _mongodb = _client.GetDatabase(dbName);
                var collection = _mongodb.GetCollection<BsonDocument>(collectionName);
                var cursor = await collection.UpdateOneAsync(filter, update);
                return cursor.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "UpdateSingleObject", ex.Message);
                return null;
            }
        }

        /// <summary>Delete single object from MongoDB</summary>
        /// <param name="filter"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        public static bool? DeleteSingleObject(FilterDefinition<BsonDocument> filter, string dbName, string collectionName)
        {
            try
            {
                var data = GetSingleObject(filter, dbName, collectionName).Result;
                var collection = _mongodb.GetCollection<BsonDocument>(collectionName);
                var response = collection.DeleteOneAsync(data);
                return response.Result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "DeleteSingleObject", ex.Message);
                return null;
            }
        }

        /// <summary>Check if a data is present in MongoDB</summary>
        /// <param name="filterField1"></param>
        /// <param name="filterData1"></param>
        /// <param name="filterField2"></param>
        /// <param name="filterData2"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        public static BsonDocument CheckForDatas(string filterField1, dynamic filterData1, string filterField2, dynamic filterData2, string dbName, string collectionName)
        {
            try
            {
                FilterDefinition<BsonDocument> filter;
                if (filterField2 == null)
                {
                    filter = Builders<BsonDocument>.Filter.Eq(filterField1, filterData1);
                }
                else
                {
                    filter = Builders<BsonDocument>.Filter.Eq(filterField1, filterData1) & Builders<BsonDocument>.Filter.Eq(filterField2, filterData2);
                }
                return GetSingleObject(filter, dbName, collectionName).Result;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "CheckForDatas", ex.Message);
                return null;
            }
        }

        /// <summary>Get product list  from MongoDB</summary>
        /// <param name="productSKU"></param>
        /// <param name="product_db"></param>
        public async static Task<List<Product>> GetProducts(string productSKU, IMongoDatabase product_db)
        {
            try
            {
                IAsyncCursor<Product> productCursor = await product_db.GetCollection<Product>("Product").FindAsync(Builders<Product>.Filter.Eq("ProductSKU", productSKU));
                var products = productCursor.ToList();
                return products;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "GetProducts", ex.Message);
                return null;
            }
        }

        /// <summary></summary>
        /// <param name="objectId"></param>
        /// <param name="updateData"></param>
        /// <param name="updateField"></param>
        /// <param name="objectName"></param>
        public async static Task<bool?> UpdateProductDetails(dynamic objectId, dynamic updateData, string updateField, string objectName)
        {
            try
            {
                var update = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("_id", objectId), "ProductDB", "Product", Builders<BsonDocument>.Update.Set(updateField, updateData));
                string MinioObject_URL;
                //MinioObject_URL = WH.GetMinioObject("arthurclive-products", objectName).Result;
                //MinioObject_URL = AH.GetAmazonS3Object("arthurclive-products", objectName);
                MinioObject_URL = AH.GetS3Object("arthurclive-products", objectName);
                var update1 = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("_id", objectId), "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductSKU", objectName));
                var update2 = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("_id", objectId), "ProductDB", "Product", Builders<BsonDocument>.Update.Set("MinioObject_URL", MinioObject_URL));
                if (update1 == true & update2 == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "UpdateProductDetails", ex.Message);
                return null;
            }
        }

        /// <summary></summary>
        /// <param name="objectId"></param>
        /// <param name="productFor"></param>
        /// <param name="productType"></param>
        /// <param name="updateData"></param>
        /// <param name="updateField"></param>
        /// <param name="objectName"></param>
        public async static Task<bool?> UpdateCategoryDetails(dynamic objectId, string productFor, string productType, dynamic updateData, string updateField, string objectName)
        {
            try
            {
                var update = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("_id", objectId), "ProductDB", "Category", Builders<BsonDocument>.Update.Set(updateField, updateData));
                string MinioObject_URL;
                //MinioObject_URL = WH.GetMinioObject("products", objectName).Result;
                //MinioObject_URL = AH.GetAmazonS3Object("product-category", objectName);
                MinioObject_URL = AH.GetS3Object("product-category", objectName);
                var update1 = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("_id", objectId), "ProductDB", "Category", Builders<BsonDocument>.Update.Set("MinioObject_URL", MinioObject_URL));
                return update1;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "UpdateCategoryDetails", ex.Message);
                return null;
            }
        }

        /// <summary>Update order details</summary>
        /// <param name="orderId"></param>
        public async static Task<string> UpdatePaymentDetails(long orderId)
        {
            try
            {
                PaymentMethod paymentDetails = new PaymentMethod();
                List<StatusCode> statusCodeList = new List<StatusCode>();
                var orderData = BsonSerializer.Deserialize<OrderInfo>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", orderId), "OrderDB", "OrderInfo").Result);
                foreach (var detail in orderData.PaymentDetails.Status)
                {
                    statusCodeList.Add(detail);
                }
                statusCodeList.Add(new StatusCode { StatusId = 2, Description = "Payment Received", Date = DateTime.UtcNow });
                paymentDetails.Status = statusCodeList;
                var updatePaymentDetails = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("OrderId", orderId), "OrderDB", "OrderInfo", Builders<BsonDocument>.Update.Set("PaymentDetails", paymentDetails));
                return "Success";
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "UpdatePaymentDetails", ex.Message);
                return null;
            }
        }

        /// <summary>Remove product in a particular cart</summary>
        /// <param name="orderId"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        public async static Task<string> RemoveCartItems(long orderId, string userName, string email)
        {
            try
            {
                IAsyncCursor<Cart> cartCursor = await _client.GetDatabase("UserInfo").GetCollection<Cart>("Cart").FindAsync(Builders<Cart>.Filter.Eq("UserName", userName));
                var cartDatas = cartCursor.ToList();
                foreach (var cart in cartDatas)
                {
                    foreach (var product in GetProducts(cart.ProductSKU, _client.GetDatabase("ProductDB")).Result)
                    {
                        long updateQuantity = product.ProductStock - cart.ProductQuantity;
                        if (product.ProductStock - cart.ProductQuantity < 0)
                        {
                            updateQuantity = 0;
                            var emailResponce = EmailHelper.SendEmailToAdmin(userName.ToString(), email.ToString(), cart.ProductSKU, cart.ProductQuantity, product.ProductStock, orderId).Result;
                        }
                        var result = MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("ProductSKU", cart.ProductSKU), "ProductDB", "Product", Builders<BsonDocument>.Update.Set("ProductStock", updateQuantity)).Result;
                    }
                }
                var removeCartItems = _client.GetDatabase("UserInfo").GetCollection<Cart>("Cart").DeleteMany(Builders<Cart>.Filter.Eq("UserName", userName));
                return "Success";
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "RemoveCartItems", ex.Message);
                return null;
            }
        }
    }
}

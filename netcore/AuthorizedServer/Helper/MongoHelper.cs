using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AuthorizedServer.Logger;
using AuthorizedServer.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MH = AuthorizedServer.Helper.MongoHelper;

namespace AuthorizedServer.Helper
{
    /// <summary>Helper for MongoDB operations</summary>
    public class MongoHelper
    {
        /// <summary></summary>
        public static IMongoDatabase _mongodb;
        /// <summary></summary>
        public static MongoClient _client = GetClient();
        /// <summary></summary>
        public static FilterDefinition<BsonDocument> filter;
        /// <summary>Get Mongo Client</summary>
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
                _mongodb = MH._client.GetDatabase(dbName);
                var collection = _mongodb.GetCollection<BsonDocument>(collectionName);
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

        /// <summary>
        /// Update single object in MongoDB
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="update"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Chech MongoDB for specific data
        /// </summary>
        /// <param name="filterField1"></param>
        /// <param name="filterData1"></param>
        /// <param name="filterField2"></param>
        /// <param name="filterData2"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static BsonDocument CheckForDatas(string filterField1, string filterData1, string filterField2, string filterData2, string dbName, string collectionName)
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

        /// <summary>To record invalid login attempts</summary>
        /// <param name="filter"></param>
        public static string RecordLoginAttempts(FilterDefinition<BsonDocument> filter)
        {
            try
            {
                var verifyUser = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(filter, "Authentication", "Authentication").Result);
                if (verifyUser.WrongAttemptCount < 10)
                {
                    var update = Builders<BsonDocument>.Update.Set("WrongAttemptCount", verifyUser.WrongAttemptCount + 1);
                    var result = MH.UpdateSingleObject(filter, "Authentication", "Authentication", update).Result;
                    return "Login Attempt Recorded";
                }
                else
                {
                    var update = Builders<BsonDocument>.Update.Set("Status", "Revoked");
                    var result = MH.UpdateSingleObject(filter, "Authentication", "Authentication", update).Result;
                    return "Account Blocked";
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("MongoHelper", "RecordLoginAttempts", ex.Message);
                return "Failed";
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
    }
}

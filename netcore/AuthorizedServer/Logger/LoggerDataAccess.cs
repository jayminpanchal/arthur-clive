using MongoDB.Bson;
using MongoDB.Driver;
using WH = AuthorizedServer.Helper.MongoHelper;

namespace AuthorizedServer.Logger
{
    /// <summary>Data access for logger</summary>
    public class LoggerDataAccess
    {
        /// <summary>Get Mongo Database</summary>
        public static IMongoDatabase _db = WH._client.GetDatabase("ArthurCliveLogDB");

        /// <summary>Create log for server side error</summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="errorDescription"></param>
        public static void CreateLog(string className, string methodName, string errorDescription)
        {
            ApplicationLogger logger =
                new ApplicationLogger
                {
                    Project = "Arthur_Clive",
                    Class = className,
                    Method = methodName,
                    Description = errorDescription
                };
            var collection = _db.GetCollection<ApplicationLogger>("ServerLog");
            collection.InsertOneAsync(logger);
        }
    }

    /// <summary>Data to be inserted to server side log</summary>
    public class ApplicationLogger
    {
        /// <summary>ObjectId given by MongoDB</summary>
        public ObjectId Id { get; set; }
        /// <summary>Name of controller where error occurs</summary>
        public string Project { get; set; }
        /// <summary>Method where error occurs</summary>
        public string Class { get; set; }
        /// <summary>Method name where error occurs</summary>
        public string Method { get; set; }
        /// <summary>Description of error</summary>
        public string Description { get; set; }
    }
}

using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using AuthorizedServer;
using AuthorizedServer.Controllers;
using AuthorizedServer.Helper;
using AuthorizedServer.Models;
using AuthorizedServer.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json.Linq;

namespace UnitTest_AuthorizedServer.Controller
{
    class Integrationtest_AuthorizedServerController_Helper
    {
        public static IOptions<Audience> _settings;
        public static IRTokenRepository _repo;

        public Integrationtest_AuthorizedServerController_Helper(IOptions<Audience> settings, IRTokenRepository repo)
        {
            _settings = settings;
            _repo = repo;
        }

        public static AuthController GetAuthController()
        {
            AuthController authController = new AuthController(_settings,_repo);
            return authController;
        }

        public static TokenController GetTokenController()
        {
            Mock<IOptions<Audience>> mockedSettings = new Mock<IOptions<Audience>>();
            Mock<IRTokenRepository> mockedRepo = new Mock<IRTokenRepository>();
            TokenController tokenController = new TokenController(mockedSettings.Object,mockedRepo.Object);
            return tokenController; 
        }

        public static ResponceData DeserializedResponceData(dynamic data)
        {
            ActionResultModel deserializedResponce = new ActionResultModel();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedResponce.GetType());
            deserializedResponce = ser.ReadObject(ms) as ActionResultModel;
            ms.Close();
            return deserializedResponce.Value;
        }

        public static IMongoDatabase db = MongoHelper._client.GetDatabase("Authentication");

        public async static Task<string> InsertRegiterModeldata(RegisterModel registerModel)
        {
            await db.GetCollection<RegisterModel>("Authentication").InsertOneAsync(registerModel);
            return "Success";
        }

        public static ResponceData_AuthController_GetUserInfo DeserializedResponceData_AuthController_GetUserInfo(dynamic jsonData)
        {
            ActionResultModel_AuthController_GetUserInfo deserializedResponce = new ActionResultModel_AuthController_GetUserInfo();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedResponce.GetType());
            deserializedResponce = ser.ReadObject(ms) as ActionResultModel_AuthController_GetUserInfo;
            ms.Close();
            return deserializedResponce.Value;
        }
    }
}

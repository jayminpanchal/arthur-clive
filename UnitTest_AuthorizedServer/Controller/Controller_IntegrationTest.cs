using System.Threading.Tasks;
using AuthorizedServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using TH = UnitTest_AuthorizedServer.Controller.Integrationtest_AuthorizedServerController_Helper;
using MH = AuthorizedServer.Helper.MongoHelper;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using AuthorizedServer;
using System;
using Microsoft.AspNetCore.Identity;

namespace UnitTest_AuthorizedServer
{
    [TestClass]
    public class AuthController_IntegrationTest
    {
        [TestMethod]
        public void AuthController_Register_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "asd123",
                UserLocation = "IN"
            };
            var expectedCode = "200";
            var expectedMessage = "User Registered";

            //Act
            var result = TH.GetAuthController().Register(registerModel) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.Title, registerModel.Title);
            Assert.AreEqual(insertedData.FullName, registerModel.FullName);
            Assert.AreEqual(insertedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(insertedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(insertedData.Email, registerModel.Email);
            Assert.AreEqual(insertedData.Password, registerModel.Password);
            Assert.AreEqual(insertedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(insertedData.UserName, username);
            Assert.AreEqual(insertedData.UserRole, "User");
            Assert.AreEqual(insertedData.Title, registerModel.Title);
            Assert.IsNull(insertedData.SocialId);
            Assert.IsNotNull(insertedData.VerificationCode);
            Assert.AreEqual(insertedData.Status, "Registered");
            Assert.AreEqual(insertedData.WrongAttemptCount, 0);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }

        //[TestMethod]
        public void AuthController_RegisterVerification_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            var otp = "123456";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "SamplePassword",
                UserLocation = "IN",
                Status = "Registered",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().RegisterVerification(username, otp) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(insertedData.Title, registerModel.Title);
            Assert.AreEqual(insertedData.FullName, registerModel.FullName);
            Assert.AreEqual(insertedData.UserName, username);
            Assert.AreEqual(insertedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(insertedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(insertedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(insertedData.Email, registerModel.Email);
            Assert.AreEqual(insertedData.Password, registerModel.Password);
            Assert.AreEqual(insertedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(insertedData.Status, "Verified");
            Assert.AreEqual(insertedData.OTPExp, registerModel.OTPExp);
            Assert.AreEqual(insertedData.VerificationCode, registerModel.VerificationCode);
            Assert.IsNull(insertedData.SocialId);
            Assert.AreEqual(insertedData.WrongAttemptCount, 0);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }

        public PasswordHasher<RegisterModel> passwordHasher = new PasswordHasher<RegisterModel>();

        //[TestMethod]
        public void AuthController_Login_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username1 = "12341234";
            var username2 = "sample@gmail.com";
            var password = "asd123";
            RegisterModel registerModel1 = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username1,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            RegisterModel registerModel2 = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username2,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBGfXjSd9Gv/ljTXODfpDS9jZ6WOSSlquxll1iZM0z86fEfBHsGkbrBcZT767CBB3Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            LoginModel loginModel1 = new LoginModel
            {
                UserName = username1,
                Password = password
            };
            //Model with incorrect password
            LoginModel loginModel2 = new LoginModel
            {
                UserName = username2,
                Password = "asd122"
            };

            //Insert test data
            var insert1 = TH.InsertRegiterModeldata(registerModel1).Result;
            var insert2 = TH.InsertRegiterModeldata(registerModel2).Result;

            //Act
            var result1 = TH.GetAuthController().Login(loginModel1) as ActionResult;
            var responseData1 = TH.DeserializedResponceData(result1.ToJson());
            var result2 = TH.GetAuthController().Login(loginModel2) as ActionResult;
            var responseData2 = TH.DeserializedResponceData(result2.ToJson());

            //Check inserted data
            var insertedData1 = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username1), "Authentication", "Authentication").Result);
            var insertedData2 = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username2), "Authentication", "Authentication").Result);

            //Assert
            Assert.IsNotNull(result1);
            Assert.AreEqual(insertedData1.Title, registerModel1.Title);
            Assert.AreEqual(insertedData1.FullName, registerModel1.FullName);
            Assert.AreEqual(insertedData1.UserName, username1);
            Assert.AreEqual(insertedData1.UserRole, registerModel1.UserRole);
            Assert.AreEqual(insertedData1.DialCode, registerModel1.DialCode);
            Assert.AreEqual(insertedData1.PhoneNumber, registerModel1.PhoneNumber);
            Assert.AreEqual(insertedData1.Email, registerModel1.Email);
            Assert.AreEqual(insertedData1.Password, registerModel1.Password);
            Assert.AreEqual(insertedData1.UserLocation, registerModel1.UserLocation);
            Assert.AreEqual(insertedData1.Status, registerModel1.Status);
            Assert.AreEqual(insertedData1.VerificationCode, registerModel1.VerificationCode);
            Assert.IsNull(insertedData1.SocialId);
            Assert.AreEqual(insertedData1.WrongAttemptCount, 0);
            Assert.IsNotNull(result2);
            Assert.AreEqual(insertedData2.Title, registerModel2.Title);
            Assert.AreEqual(insertedData2.FullName, registerModel2.FullName);
            Assert.AreEqual(insertedData2.UserName, username2);
            Assert.AreEqual(insertedData2.UserRole, registerModel2.UserRole);
            Assert.AreEqual(insertedData2.DialCode, registerModel2.DialCode);
            Assert.AreEqual(insertedData2.PhoneNumber, registerModel2.PhoneNumber);
            Assert.AreEqual(insertedData2.Email, registerModel2.Email);
            Assert.AreEqual(insertedData2.Password, registerModel2.Password);
            Assert.AreEqual(insertedData2.UserLocation, registerModel2.UserLocation);
            Assert.AreEqual(insertedData2.Status, registerModel2.Status);
            Assert.AreEqual(insertedData2.VerificationCode, registerModel2.VerificationCode);
            Assert.IsNull(insertedData2.SocialId);
            Assert.AreEqual(insertedData2.WrongAttemptCount, 1);

            //Delete inserted test data
            var checkData1 = MH.CheckForDatas("UserName", username1, null, null, "Authentication", "Authentication");
            if (checkData1 != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username1), "Authentication", "Authentication");
            }
            var checkData2 = MH.CheckForDatas("UserName", username2, null, null, "Authentication", "Authentication");
            if (checkData2 != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username2), "Authentication", "Authentication");
            }
        }

        [TestMethod]
        public void AuthController_ForgotPassword_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            ForgotPasswordModel forgotPasswordModel = new ForgotPasswordModel
            {
                UserName = username,
                UserLocation = "IN"
            };
            var expectedCode = "200";
            var expectedMessage = "Success";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().ForgotPassword(forgotPasswordModel) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.Title, registerModel.Title);
            Assert.AreEqual(insertedData.FullName, registerModel.FullName);
            Assert.AreEqual(insertedData.UserName, username);
            Assert.AreEqual(insertedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(insertedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(insertedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(insertedData.Email, registerModel.Email);
            Assert.AreEqual(insertedData.Password, registerModel.Password);
            Assert.AreEqual(insertedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(insertedData.Status, "Not Verified");
            Assert.IsNotNull(insertedData.VerificationCode);
            Assert.IsNull(insertedData.SocialId);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }

        //[TestMethod]
        public void AuthController_ForgotPasswordVerification_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            var otp = "123456";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Not Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().ForgotPasswordVerification(username, otp) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(insertedData.Title, registerModel.Title);
            Assert.AreEqual(insertedData.FullName, registerModel.FullName);
            Assert.AreEqual(insertedData.UserName, username);
            Assert.AreEqual(insertedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(insertedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(insertedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(insertedData.Email, registerModel.Email);
            Assert.AreEqual(insertedData.Password, registerModel.Password);
            Assert.AreEqual(insertedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(insertedData.Status, "Verified");
            Assert.AreEqual(insertedData.VerificationCode, registerModel.VerificationCode);
            Assert.IsNull(insertedData.SocialId);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }

        [TestMethod]
        public void AuthController_ChangePassword_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            var newPassword = "qwe123";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            ChangePassword_ForgotPasswordModel model = new ChangePassword_ForgotPasswordModel
            {
                UserName = username,
                Password = newPassword
            };
            bool checkPassword = false;
            var expectedCode = "200";
            var expectedMessage = "Password Changed Successfully";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().ChangePassword(model) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);

            //Check updated password
            RegisterModel data = new RegisterModel { UserName = username, Password = newPassword };
            if (passwordHasher.VerifyHashedPassword(data, insertedData.Password, newPassword).ToString() == "Success")
            {
                checkPassword = true;
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.Title, registerModel.Title);
            Assert.AreEqual(insertedData.FullName, registerModel.FullName);
            Assert.AreEqual(insertedData.UserName, username);
            Assert.AreEqual(insertedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(insertedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(insertedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(insertedData.Email, registerModel.Email);
            Assert.IsNotNull(insertedData.Password);
            Assert.AreEqual(insertedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(insertedData.Status, registerModel.Status);
            Assert.IsNotNull(insertedData.VerificationCode);
            Assert.IsNull(insertedData.SocialId);
            Assert.AreEqual(checkPassword, true);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }

        [TestMethod]
        public void AuthController_ChangePasswordWhenLoggedIn_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            var oldPassword = "asd123";
            var newPassword = "qwe123";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            ChangePasswordModel changePasswordModel = new ChangePasswordModel
            {
                UserName = username,
                OldPassword = oldPassword,
                NewPassword = newPassword
            };
            bool checkPassword = false;
            var expectedCode = "200";
            var expectedMessage = "Password Changed Successfully";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().ChangePasswordWhenLoggedIn(changePasswordModel) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);

            //Check updated password
            RegisterModel data = new RegisterModel { UserName = username, Password = newPassword };
            if (passwordHasher.VerifyHashedPassword(data, insertedData.Password, newPassword).ToString() == "Success")
            {
                checkPassword = true;
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.Title, registerModel.Title);
            Assert.AreEqual(insertedData.FullName, registerModel.FullName);
            Assert.AreEqual(insertedData.UserName, username);
            Assert.AreEqual(insertedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(insertedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(insertedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(insertedData.Email, registerModel.Email);
            Assert.IsNotNull(insertedData.Password);
            Assert.AreEqual(insertedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(insertedData.Status, registerModel.Status);
            Assert.IsNotNull(insertedData.VerificationCode);
            Assert.IsNull(insertedData.SocialId);
            Assert.AreEqual(checkPassword, true);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }

        [TestMethod]
        public void AuthController_DeactivateAccount_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            var password = "asd123";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            DeactivateAccountModel deactivateAccountModel = new DeactivateAccountModel
            {
                UserName = username,
                Password = password
            };
            var expectedCode = "200";
            var expectedMessage = "User Deactivated";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().DeactivateAccount(deactivateAccountModel) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check if user account is deactivated
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code,expectedCode);
            Assert.AreEqual(responseData.Message,expectedMessage);
            Assert.IsNull(checkData);

        }

        //Pending
        //[TestMethod]
        public void AuthController_GoogleLogin_IntegrationTest_AuthorizedServer()
        {

        }

        //Pending
        //[TestMethod]
        public void AuthController_FaceBookLogin_IntegrationTest_AuthorizedServer()
        {

        }

        //Pending
        //[TestMethod]
        public void AuthController_FaceBookLoginCheck_IntegrationTest_AuthorizedServer()
        {

        }

        [TestMethod]
        public void AuthController_GetUserInfo_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            var expectedCode = "200";
            var expectedMessage = "Success";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().GetUserInfo(username) as ActionResult;
            var responseData = TH.DeserializedResponceData_AuthController_GetUserInfo(result.ToJson());

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(responseData.Data.FullName,registerModel.FullName);
            Assert.AreEqual(responseData.Data.UserName, registerModel.UserName);
            Assert.AreEqual(responseData.Data.DialCode, registerModel.DialCode);
            Assert.AreEqual(responseData.Data.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(responseData.Data.Email, registerModel.Email);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }
        
        [TestMethod]
        public void AuthController_UpdateFullName_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            FullNameUpdateModel fullNameUpdateModel = new FullNameUpdateModel
            {
                FullName = "Updated FullName"
            };
            var expectedCode = "200";
            var expectedMessage = "FullName updated successfully";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().UpdateFullName(fullNameUpdateModel,username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check updated data
            var updatedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);
            
            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(updatedData.Title,registerModel.Title);
            Assert.AreEqual(updatedData.FullName, fullNameUpdateModel.FullName);
            Assert.AreEqual(updatedData.UserName, registerModel.UserName);
            Assert.AreEqual(updatedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(updatedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(updatedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(updatedData.Email, registerModel.Email);
            Assert.AreEqual(updatedData.Password, registerModel.Password);
            Assert.AreEqual(updatedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(updatedData.Status, registerModel.Status);
            Assert.AreEqual(updatedData.VerificationCode, registerModel.VerificationCode);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }

        [TestMethod]
        public void AuthController_UpdatePhoneNumber_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            PhoneNumberUpdateModel phoneNumberUpdateModel = new PhoneNumberUpdateModel
            {
                DialCode = "+11",
                PhoneNumber = "23452345"
            };
            var expectedCode = "200";
            var expectedMessage = "PhoneNumber, dialcode and username updated successfully";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().UpdatePhoneNumber(phoneNumberUpdateModel, username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check updated data
            var updatedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", phoneNumberUpdateModel.PhoneNumber), "Authentication", "Authentication").Result);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(updatedData.Title, registerModel.Title);
            Assert.AreEqual(updatedData.FullName, registerModel.FullName);
            Assert.AreEqual(updatedData.UserName, phoneNumberUpdateModel.PhoneNumber);
            Assert.AreEqual(updatedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(updatedData.DialCode, phoneNumberUpdateModel.DialCode);
            Assert.AreEqual(updatedData.PhoneNumber, phoneNumberUpdateModel.PhoneNumber);
            Assert.AreEqual(updatedData.Email, registerModel.Email);
            Assert.AreEqual(updatedData.Password, registerModel.Password);
            Assert.AreEqual(updatedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(updatedData.Status, registerModel.Status);
            Assert.AreEqual(updatedData.VerificationCode, registerModel.VerificationCode);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", phoneNumberUpdateModel.PhoneNumber, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", phoneNumberUpdateModel.PhoneNumber), "Authentication", "Authentication");
            }
        }

        [TestMethod]
        public void AuthController_UpdateEmailId_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "sample@gmail.com";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            EmailUpdateModel emailUpdateModel = new EmailUpdateModel
            {
                Email = "update@gmail.com"
            };
            var expectedCode = "200";
            var expectedMessage = "Email and username updated successfully";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().UpdateEmailId(emailUpdateModel, username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check updated data
            var updatedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", emailUpdateModel.Email), "Authentication", "Authentication").Result);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(updatedData.Title, registerModel.Title);
            Assert.AreEqual(updatedData.FullName, registerModel.FullName);
            Assert.AreEqual(updatedData.UserName, emailUpdateModel.Email);
            Assert.AreEqual(updatedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(updatedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(updatedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(updatedData.Email, emailUpdateModel.Email);
            Assert.AreEqual(updatedData.Password, registerModel.Password);
            Assert.AreEqual(updatedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(updatedData.Status, registerModel.Status);
            Assert.AreEqual(updatedData.VerificationCode, registerModel.VerificationCode);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", emailUpdateModel.Email, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", emailUpdateModel.Email), "Authentication", "Authentication");
            }
        }

        [TestMethod]
        public void AuthController_UpdatePassword_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var username = "12341234";
            RegisterModel registerModel = new RegisterModel
            {
                Title = "Mr",
                FullName = "SampleName",
                UserName = username,
                UserRole = "User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "AQAAAAEAACcQAAAAEBvKk8jgvNFzFduUBnMgagNfmDjfNofNwRw4uT6OkcVb3d4UzgLy6oM6RgtLCrsD5Q==",
                UserLocation = "IN",
                Status = "Verified",
                OTPExp = DateTime.UtcNow.AddMinutes(4),
                VerificationCode = "AQAAAAEAACcQAAAAEDpGr4+u/Oik7F6OLHd3Tr03AX+jYRRqNeo48Il9md5wcPFBl+1xpDQLkimghNMogg=="
            };
            PasswordUpdateModel passwordUpdateModel = new PasswordUpdateModel
            {
                CurrentPassword = "asd123",
                NewPassword = "qwe123"
            };
            var expectedCode = "200";
            var expectedMessage = "Password Changed Successfully";

            //Insert test data
            var insert = TH.InsertRegiterModeldata(registerModel).Result;

            //Act
            var result = TH.GetAuthController().UpdatePassword(passwordUpdateModel, username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check updated data
            var updatedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);

            //Check if password is correct
            bool passwordMatched = false;
            if (passwordHasher.VerifyHashedPassword(new RegisterModel { UserName = registerModel.UserName, Password = registerModel.Password }, updatedData.Password, passwordUpdateModel.NewPassword).ToString() == "Success")
            {
                passwordMatched = true;
            }

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(updatedData.Title, registerModel.Title);
            Assert.AreEqual(updatedData.FullName, registerModel.FullName);
            Assert.AreEqual(updatedData.UserName, registerModel.UserName);
            Assert.AreEqual(updatedData.UserRole, registerModel.UserRole);
            Assert.AreEqual(updatedData.DialCode, registerModel.DialCode);
            Assert.AreEqual(updatedData.PhoneNumber, registerModel.PhoneNumber);
            Assert.AreEqual(updatedData.Email, registerModel.Email);
            Assert.AreEqual(updatedData.UserLocation, registerModel.UserLocation);
            Assert.AreEqual(updatedData.Status, registerModel.Status);
            Assert.AreEqual(updatedData.VerificationCode, registerModel.VerificationCode);
            Assert.AreEqual(passwordMatched,true);

                //Delete inserted test data
                var checkData = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }
    }

    [TestClass]
    public class TokenController_IntegrationTest
    {
        //[TestMethod]
        public void TokenController_Auth_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            Parameters parameters1 = new Parameters
            {
                grant_type = "password",
                username = "SampleUser1",
                fullname = "SampleName1"
            };
            var expectedCode1 = "200";
            var expectedMessage1 = "User Registered";

            //Act

            var result1 = TH.GetTokenController().Auth(parameters1) as ActionResult;
            var responseData1 = TH.DeserializedResponceData(result1.ToJson());
            Parameters parameters2 = new Parameters
            {
                grant_type = "refresh_token",
                username = parameters1.username,
                fullname = parameters1.fullname
            };
            var result2 = TH.GetTokenController().Auth(parameters2) as ActionResult;
            var responseData2 = TH.DeserializedResponceData(result2.ToJson());

            //Check if user is unsubscribed
            //var insertedData = BsonSerializer.Deserialize<RegisterModel>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication").Result);

            //Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
        }
    }
}

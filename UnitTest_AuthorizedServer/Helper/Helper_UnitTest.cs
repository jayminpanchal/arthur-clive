using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AuthorizedServer;
using AuthorizedServer.Helper;
using AuthorizedServer.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

namespace UnitTest_AuthorizedServer.Helper
{
    [TestClass]
    public class AuthHelper_UnitTest
    {
        public AuthHelper authHelper = new AuthHelper();

        //Pending
        //[TestMethod]
        public void AuthHelper_DoPassword_UnitTest_AuthorizedServer()
        {
            //Arrange
            var repo = new Mock<IRTokenRepository>();
            var settings = new Mock<IOptions<Audience>>();
            Parameters parameters = new Parameters();
            parameters.username = "sample@gmail.com";
            parameters.fullname = "Sample User";

            //Act
            var result = authHelper.DoPassword(parameters, repo.Object, settings.Object);

            //Assert
            Assert.IsNotNull(result);
            //Assert.AreEqual("999", result.Code);
            //Assert.IsNotNull(result.Data);
            //Assert.AreEqual("Ok", result.Message);
        }

        //Pending
        //[TestMethod]
        public void AuthHelper_DoRefreshToken_UnitTest_AuthorizedServer()
        {
            //Arrange
            var repo = new Mock<IRTokenRepository>();
            var settings = new Mock<IOptions<Audience>>();
            Parameters parameters = new Parameters();
            parameters.username = "sample@gmail.com";
            parameters.fullname = "Sample User";

            //Act
            var result = authHelper.DoRefreshToken(parameters, repo.Object, settings.Object);

            //Assert
            Assert.IsNotNull(result);
            //Assert.AreEqual("999", result.Code);
            //Assert.IsNotNull(result.Data);
            //Assert.AreEqual("Ok", result.Message);
        }

        //Pending
        //[TestMethod]
        public void AuthHelper_GetJWT_UnitTest_AuthorizedServer()
        {
            //Arrange
            var refresh_token = Guid.NewGuid().ToString().Replace("-", "");
            var client_id = "sample@gmail.com";
            var roleName = "";
            var settings = new Mock<IOptions<Audience>>();

            //Act
            var result = authHelper.GetJwt(client_id, refresh_token, settings.Object, roleName) as string;

            //Assert
            Assert.IsNotNull(result);
        }
    }

    [TestClass]
    public class EmailHelper_UnitTest
    {
        [TestMethod]
        public void EmailHelper_GetCredentials_UnitTest_AuthorizedServer()
        {
            //Arrange 
            var accesskeyFromXML = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("accesskey").FirstOrDefault().Value;
            var secretkeyFromXML = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("secretkey").FirstOrDefault().Value;
            var expectedAccessKey = "AKIAIQRMI2NXYVDB7UKA";
            var expectedSecretKey = "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";

            //Act

            //Assert
            Assert.IsNotNull(accesskeyFromXML);
            Assert.IsNotNull(secretkeyFromXML);
            Assert.AreEqual(expectedAccessKey,accesskeyFromXML);
            Assert.AreEqual(expectedSecretKey,secretkeyFromXML);
        }

        //Pending
        //[TestMethod]
        public void EmailHelper_SendEmail_UnitTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void EmailHelper_CreateEmailBody_UnitTest_AuthorizedServer()
        {
            //    //Arrange
            //    var linkFromXML = GlobalHelper.ReadXML().Elements("ipconfig").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("authorizedserver2");
            //    var fullname = "Sample user";
            //    var link = "<a href ='" + GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("websitelink").First().Value + "'>Click Here</a>";
            //    var expectedEmail = "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n\r\n<head>\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n    <title>Confirmation Email from Team ArthurClive</title>\r\n</head>\r\n\r\n<body>\r\n    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n        <tr>\r\n            <td align=\"center\" valign=\"top\" bgcolor=\"#ffe77b\" style=\"background-color:#dec786;\">\r\n                <br>\r\n                <br>\r\n                <table width=\"600\" height=\"150\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                    <tr>\r\n                        <td align=\"left\" valign=\"top\" bgcolor=\"#564319\" style=\"background-color:#0c0d0d; font-family:Arial, Helvetica, sans-serif; padding:10px;\">\r\n                            <div align=center style=\"font-size:36px; color:#ffffff;\">\r\n                                <img src=\"D:\\Arthur_Clive\\netcore\\AuthorizedServer\\EmailTemplate\\logo-caption.png\" width=\"200px\" height=\"120px\" />\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <span>\r\n                                    <b>Sample user</b>\r\n                                </span>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>Congratulations! You are registered...</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>Team Arthur Clive Welcomes you...</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>To verify your Arthur Clive account click on the link given below</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <span>\r\n                                    <b><a href ='https://artwear.in/'>Click Here</a></b>\r\n                                </span>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n\r\n</html>";
            //    string emailBody;

            //    //Act
            //    var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //    var path = Path.Combine(dir, "EmailVerification.html");
            //    using (StreamReader reader = File.OpenText(path))
            //    {
            //        emailBody = reader.ReadToEnd();
            //    }
            //    emailBody = emailBody.Replace("{FullName}", fullname);
            //    emailBody = emailBody.Replace("{Link}", link);
            //    var result = EmailHelper.CreateEmailBody(fullname, link) as string;

            //    //Assert
            //    Assert.IsNotNull(result);
            //    Assert.AreEqual(expectedEmail, result);
            //    Assert.IsNotNull(linkFromXML);
        }
    }

    [TestClass]
    public class GlobalHelper_UnitTest
    {
        [TestMethod]
        public void GlobalHelper_GetCurrentDir_UnitTest_AuthorizedServer()
        {
            //Arrange
            var expectedPath = "D:\\Arthur_Clive\\UnitTest_AuthorizedServer\\bin\\Debug\\netcoreapp2.0";

            //Act
            var result = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPath,result);
        }
        
        [TestMethod]
        public void GlobalHelper_ReadXML_UnitTest_AuthorizedServer()
        {
            //Arrange
            var dir = "D:\\Arthur_Clive\\UnitTest_AuthorizedServer\\bin\\Debug\\netcoreapp2.0";
            var expectedXElement = "<credentials>\r\n  <!-- Mongo Credentials -->\r\n  <mongo>\r\n    <current>Yes</current>\r\n    <ip>localhost</ip>\r\n    <db>admin</db>\r\n    <user>Ragu</user>\r\n    <password>123ragu</password>\r\n  </mongo>\r\n  <!-- Contents related to email service -->\r\n  <email>\r\n    <current>Yes</current>\r\n    <emailsender>sales@artwear.in</emailsender>\r\n    <websitelink>https://artwear.in/</websitelink>\r\n    <emailsubject1>Message from the Arthur Clive Admin.</emailsubject1>\r\n    <emailsubject2>Verification of your ArthurClive account.</emailsubject2>\r\n  </email>\r\n  <!-- IPconfigurations for the project -->\r\n  <ipconfig>\r\n    <current>Yes</current>\r\n    <arthurclive>http://192.168.0.117:5000/</arthurclive>\r\n    <authorizedserver>http://192.168.0.117:5001/</authorizedserver>\r\n    <authorizedserver2>http://localhost:3000/#/verifyemail/</authorizedserver2>\r\n  </ipconfig>\r\n  <ipconfig>\r\n    <current>No</current>\r\n    <arthurclive>http://192.168.0.117:52922/</arthurclive>\r\n    <authorizedserver>http://192.168.0.117:56872/</authorizedserver>\r\n  </ipconfig>\r\n  <!-- Amazon S3 credentials -->\r\n  <amasons3>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAIUAYVIL7A7I6XECA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amasons3>\r\n  <!-- Amazon SNS credentials -->\r\n  <amazonsns>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAJCDU2723BYGUAHCA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonsns>\r\n  <amazonsns>\r\n    <current>No</current>\r\n    <accesskey>AKIAJ3B7P4FXGYSUXMYA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonsns>\r\n  <!-- Amazon SES credentials -->\r\n  <amazonses>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAIQRMI2NXYVDB7UKA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <amazonses>\r\n    <current>No</current>\r\n    <accesskey>AKIAJ3UPJB4KZY3GTNSA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <amazonses>\r\n    <current>test</current>\r\n    <accesskey>AKIAJZK4MPI7AEGQGUIQ</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <!-- Minio credentials -->\r\n  <minioclient>\r\n    <current>Yes</current>\r\n    <host>localhost:9000</host>\r\n    <accesskey>MinioServer</accesskey>\r\n    <secretkey>123654789@Ragu</secretkey>\r\n  </minioclient>\r\n</credentials>";

            //Act
            var xmlStr = File.ReadAllText(Path.Combine(dir, "AmazonKeys.xml"));
            var result = XElement.Parse(xmlStr);
            var xElementString = result.ToString();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedXElement,xElementString);
        }

        [TestMethod]
        public void GlobalHelper_GetIpConfig_UnitTest_AuthorizedServer()
        {
            //Arrange
            var result = GlobalHelper.ReadXML().Elements("ipconfig").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("authorizedserver2").First().Value;
            var expectedIp = "http://localhost:3000/#/verifyemail/";

            //Act

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedIp,result);
        }

    }

    [TestClass]
    public class MongoHelper_UnitTest
    {
        [TestMethod]
        public void MongoHelper_GetClient_UnitTest_AuthorizedServer()
        {
            //Arrange
            var ipFromXML = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("ip").First().Value;
            var userFromXML = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("user").First().Value;
            var passwordFromXML = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("password").First().Value;
            var dbFromXML = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("db").First().Value;
            var connectionString = "mongodb://" + userFromXML + ":" + passwordFromXML+ "@" + ipFromXML+ ":27017/" + dbFromXML;
            var expectedIp = "localhost";
            var expectedUser = "Ragu";
            var expectedPassword = "123ragu";
            var expectedDb = "admin";
            var expectedConnectionString = "mongodb://Ragu:123ragu@localhost:27017/admin";

            //Act
            var result = new MongoClient(connectionString);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(ipFromXML);
            Assert.IsNotNull(userFromXML);
            Assert.IsNotNull(passwordFromXML);
            Assert.IsNotNull(dbFromXML);
            Assert.AreEqual(expectedIp, ipFromXML);
            Assert.AreEqual(expectedUser, userFromXML);
            Assert.AreEqual(expectedPassword, passwordFromXML);
            Assert.AreEqual(expectedDb, dbFromXML);
            Assert.AreEqual(expectedConnectionString, connectionString);
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_GetSingleObject_UnitTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_GetListOfObjects_UnitTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_UpdateSingleObject_UnitTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_CheckForDatas_UnitTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_RecordLoginAttempts_UnitTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }
    }

    [TestClass]
    public class SMSHelper_UnitTest
    {
        [TestMethod]
        public void SMSHlper_GetCredentials_UnitTest_AuthorizedServer()
        {
            //Arrange
            var accessKeyFromXMl = GlobalHelper.ReadXML().Elements("amazonsns").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("accesskey").First().Value;
            var secretKeyFromXMl = GlobalHelper.ReadXML().Elements("amazonsns").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("secretkey").First().Value;
            var expectedAccessKey = "AKIAJCDU2723BYGUAHCA";
            var expectedSecretKey = "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";

            //Act

            //Assert
            Assert.IsNotNull(accessKeyFromXMl);
            Assert.IsNotNull(secretKeyFromXMl);
            Assert.AreEqual(expectedAccessKey,accessKeyFromXMl);
            Assert.AreEqual(expectedSecretKey,secretKeyFromXMl);
            
        }

        //Pending
        //[TestMethod]
        public void SMSHlper_SendSMS_UnitTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }
    }
}

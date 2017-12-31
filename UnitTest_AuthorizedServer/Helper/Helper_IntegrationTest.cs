using AuthorizedServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuthorizedServer.Helper;
using AuthorizedServer.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using SH = AuthorizedServer.Helper.SMSHelper;
using System;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Driver;

namespace UnitTest_AuthorizedServer.Helper
{
    [TestClass]
    public class AuthHelper_IntegrationTest
    {
        public AuthHelper authHelper = new AuthHelper();

        //Pending
        //[TestMethod]
        public void AuthHelper_DoPassword_IntegrationTest_AuthorizedServer()
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
        public void AuthHelper_DoRefreshToken_IntegrationTest_AuthorizedServer()
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
        public void AuthHelper_GetJWT_IntegrationTest_AuthorizedServer()
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
    public class EmailHelper_IntegrationTest
    {
        [TestMethod]
        public void EmailHelper_GetCredentials_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var key1 = "accesskey";
            var key2 = "secretkey";

            //Act
            var accessKey = EmailHelper.GetCredentials(key1) as string;
            var secretKey = EmailHelper.GetCredentials(key2) as string;
            var accesskeyFromXML = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("Yes")).Descendants(key1);
            var secretkeyFromXML = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("Yes")).Descendants(key2);

            //Assert
            Assert.IsNotNull(accessKey);
            Assert.IsNotNull(secretKey);
            Assert.IsNotNull(accesskeyFromXML);
            Assert.IsNotNull(secretkeyFromXML);
        }

        //Pending
        //[TestMethod]
        public void EmailHelper_SendEmail_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        [TestMethod]
        public void EmailHelper_CreateEmailBody_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var linkFromXML = GlobalHelper.ReadXML().Elements("ipconfig").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("authorizedserver2");
            var fullname = "Sample user";
            var link = "<a href ='" + GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("websitelink").First().Value + "'>Click Here</a>";
            var expectedEmail = "<!DOCTYPE html>\n<!-- saved from url=(0068)http://nowisthefuture.co.uk/arthurclive/5/emailtemplate-welcome.html -->\n<html lang=\"en\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\n\t\n\t<title>Arthur Clive</title>\n</head>\n<body>\n\t<table width=\"650\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" align=\"center\" bgcolor=\"#fff\">\n\t\t<tbody><tr>\n\t\t\t<td align=\"center\" valign=\"top\" style=\"background:#111; border-bottom:1px solid #cecece; padding:5px 0 5px;\">\n\t\t\t\t<a href=\"http://www.artwear.in/\"><img src=\"https://artwear.in/assets/img/logo-arthur-clive-mobile.png\" width=\"70px\" alt=\"\"></a>\n\t\t\t</td>\n\t\t</tr>\n\t\t<tr>\n\t\t\t<td valign=\"top\" align=\"center\" style=\"padding:5px 25px; border-bottom: 2px solid #cecece; font:normal 16px &#39;Myriad Pro&#39;, Arial, Helvetica, sans-serif; color:#2a2c2e;\">\n\t\t\t\t<p style=\"font-size:20px;\">Dear <span><b>Sample user</b></span></p>\n\t\t\t\t<p style=\"font-size:20px;\">Welcome to Arthur Clive</p>\n\t\t\t\t<p>&nbsp;</p>\n\t\t\t\t<p>Thank you for registering with us.</p>\n\t\t\t\t<p>Please verify the email address to activate your account.</p>\n\t\t\t\t<p>&nbsp;</p>\n\t\t\t\t<p><span><a href ='https://artwear.in/'>Click Here</a><span></p>\n\t\t\t\t<p>&nbsp;</p>\n\t\t\t</td>\n\t\t</tr>\n\t\t<tr>\n\t\t\t<td align=\"center\" valign=\"top\" style=\"padding:15px 0; font:normal 14px &#39;Myriad Pro&#39;, Arial, Helvetica, sans-serif; color:#2a2c2e; line-height:20px;\">\n\t\t\t\t<table>\n\t\t\t\t\t<tbody><tr>\n\t\t\t\t\t\t<td align=\"center\">\n\t\t\t\t\t\t\t<strong>Registered Address:</strong> Arthur Clive  .  2nd Floor  .  2/54 Azadgarh  .  Kolkata  .  PIN 700040  .  India\n\t\t\t\t\t\t</td>\n\t\t\t\t\t</tr>\n\t\t\t\t\t<tr>\n\t\t\t\t\t\t<td align=\"center\">\n\t\t\t\t\t\t\t<span><strong>Call:</strong>+91 9433978080</span>&nbsp;\n\t\t\t\t\t\t\t<span><strong>Email:</strong> <a href=\"mailto:sales@artwear.in\" target=\"_parent\" style=\"color:#2a2c2e;\">sales@artwear.in</a></span>\n\t\t\t\t\t\t</td>\n\t\t\t\t\t</tr>\n\t\t\t\t</tbody></table>\n\t\t\t</td>\n\t\t</tr>\n\t</tbody></table>\n\n</body></html>";

            //Act
            var result = EmailHelper.CreateEmailBody(fullname, link) as string;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedEmail, result);
            Assert.IsNotNull(linkFromXML);
        }
    }

    [TestClass]
    public class GlobalHelper_IntegrationTest
    {
        [TestMethod]
        public void GlobalHelper_GetCurrentDir_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act
            var result = GlobalHelper.GetCurrentDir() as string;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GlobalHelper_ReadXML_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act
            var result = GlobalHelper.ReadXML() as XElement;

            //Assert
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void GlobalHelper_GetIpConfig_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act
            var result = GlobalHelper.GetIpConfig() as string;

            //Assert
            Assert.IsNotNull(result);
        }
    }

    [TestClass]
    public class MongoHelper_IntegrationTest
    {
        [TestMethod]
        public void MongoHelper_GetClient_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var ipFromXML = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("ip").First().Value;
            var userFromXML = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("user").First().Value;
            var passwordFromXML = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("password").First().Value;
            var dbFromXML = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("db").First().Value;

            //Act
            var result = MongoHelper.GetClient() as MongoClient;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(ipFromXML);
            Assert.IsNotNull(userFromXML);
            Assert.IsNotNull(passwordFromXML);
            Assert.IsNotNull(dbFromXML);
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_GetSingleObject_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_GetListOfObjects_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_UpdateSingleObject_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_CheckForDatas_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_RecordLoginAttempts_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }
    }

    [TestClass]
    public class SMSHelper_IntegrationTest
    {
        [TestMethod]
        public void SMSHlper_GetCredentials_IntegrationTest_AuthorizedServer()
        {
            //Arrange
            var key1 = "accesskey";
            var key2 = "secretkey";

            //Act
            var accessKey = SMSHelper.GetCredentials(key1) as string;
            var secretKey = SMSHelper.GetCredentials(key2) as string;

            //Assert
            Assert.IsNotNull(accessKey);
            Assert.IsNotNull(secretKey);
        }

        //Pending
        //[TestMethod]
        public void SMSHlper_SendSMS_IntegrationTest_AuthorizedServer()
        {
            //Arrange

            //Act

            //Assert
        }
    }

}

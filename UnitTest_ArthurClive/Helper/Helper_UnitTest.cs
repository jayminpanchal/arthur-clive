using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Arthur_Clive.Data;
using Arthur_Clive.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minio;
using MongoDB.Driver;
using Moq;

namespace UnitTest_ArthurClive.Helper
{
    [TestClass]
    public class AmazonHelper_UnitTest
    {
        [TestMethod]
        public void AmazonHelper_GetAmazonS3Client_UnitTest_ArthurClive()
        {
            //Arrange
            var accessKeyFromXML = GlobalHelper.ReadXML().Elements("amasons3").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("accesskey").FirstOrDefault().Value;
            var secretKeyFromXML = GlobalHelper.ReadXML().Elements("amasons3").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("secretkey").FirstOrDefault().Value;
            var expectedAccessKey = "AKIAIUAYVIL7A7I6XECA";
            var expectedSecretKey = "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";

            //Act
            AmazonS3Client s3Client = new AmazonS3Client(accessKeyFromXML, secretKeyFromXML, Amazon.RegionEndpoint.APSouth1);

            //Assert
            Assert.IsNotNull(s3Client);
            Assert.IsNotNull(accessKeyFromXML);
            Assert.IsNotNull(secretKeyFromXML);
            Assert.AreEqual(expectedAccessKey,accessKeyFromXML);
            Assert.AreEqual(expectedSecretKey,secretKeyFromXML);
        }

        //Pending
        //[TestMethod]
        public void AmazonHelper_GetAmazonS3Object_UnitTest_ArthurClive()
        {
            //Arrange
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
            {
                BucketName = "bucketName",
                Key = "objectKey",
                Verb = HttpVerb.GET,
                Expires = DateTime.Now.AddMinutes(5)
            };
            Mock<AmazonS3Client> s3Client = new Mock<AmazonS3Client>();

            //Act
            //var result = s3Client.GetPreSignedURL(request);

            //Assert
        }

        [TestMethod]
        public void AmazonHelper_GetS3Object_UnitTest_ArthurClive()
        {
            //Arrange
            var bucketName = "bucketname";
            var objectName = "objectname";
            var expectedUrl = "https://s3.ap-south-1.amazonaws.com/bucketname/objectname";

            //Act
            var presignedUrl = "https://s3.ap-south-1.amazonaws.com/" + bucketName + "/" + objectName;

            //Assert
            Assert.IsNotNull(presignedUrl);
            Assert.AreEqual(expectedUrl, presignedUrl);
        }
    }

    [TestClass]
    public class EmailHelper_UnitTest
    {
        [TestMethod]
        public void EmailHelper_GetCredentials_UnitTest_ArthurClive()
        {
            //Arrange
            var accesskeyFromXml = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("accesskey").FirstOrDefault().Value;
            var secretkeyFromXml = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("secretkey").FirstOrDefault().Value;
            var expectedAccessKey = "AKIAIQRMI2NXYVDB7UKA";
            var expectedSecretKey = "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";

            //Act

            //Assert
            Assert.IsNotNull(accesskeyFromXml);
            Assert.IsNotNull(secretkeyFromXml);
            Assert.AreEqual(expectedAccessKey,accesskeyFromXml);
            Assert.AreEqual(expectedSecretKey,secretkeyFromXml);
        }
        
        //Pending
        //[TestMethod]
        public void EmailHelper_SendEmail_UnitTest_ArthurClive()
        {
            //Arrange
            string emailSender = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsender").First().Value;
            string link = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("websitelink").First().Value;
            var fullname = "Sample user";
            var message = "Message needed to be sent by email";
            string emailReceiver = "ragu9060@gmail.com";
            string result;
            var expectedEmailSender = "sales@artwear.in";
            var expectedLink = "https://artwear.in/"; 

            //Act
            using (var client = new AmazonSimpleEmailServiceClient(EmailHelper.GetCredentials("accesskey"), EmailHelper.GetCredentials("secretkey"), Amazon.RegionEndpoint.USWest2))
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
                            Html = new Content(EmailHelper.CreateEmailBody(fullname, "<a href ='" + link + "'>Click Here</a>", message))
                        }
                    }
                };
                //var responce = await Mock<AmazonS3Client>.SendEmailAsync(sendRequest);
                result = "Success";
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedEmailSender,emailSender);
            Assert.AreEqual(expectedLink,link);
        }
        
        [TestMethod]
        public void EmailHelper_CreateEmailBody_UnitTest_ArthurClive()
        {
            //Arrange
            var fullname = "Sample user";
            var message = "Body of email sent to the receiver";
            var link = "<a href ='" + GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("websitelink").First().Value + "'>Click Here</a>";
            var expectedEmail = "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n\r\n<head>\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n    <title>Message from team Arthur Clive</title>\r\n</head>\r\n\r\n<body>\r\n    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n        <tr>\r\n            <td align=\"center\" valign=\"top\" bgcolor=\"#ffe77b\" style=\"background-color:#dec786;\">\r\n                <br>\r\n                <br>\r\n                <table width=\"600\" height=\"150\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                    <tr>\r\n                        <td align=\"left\" valign=\"top\" bgcolor=\"#564319\" style=\"background-color:#0c0d0d; font-family:Arial, Helvetica, sans-serif; padding:10px;\">\r\n                            <div align=center style=\"font-size:36px; color:#ffffff;\">\r\n                                <img src=\"D:\\Arthur_Clive\\netcore\\AuthorizedServer\\EmailTemplate\\logo-caption.png\" width=\"200px\" height=\"120px\" />\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <span>\r\n                                    <b>Sample user</b>\r\n                                </span>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>Team Arthur Clive Welcomes you back...</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>This is a message from team Arthur Clive</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;color:red\">\r\n                                <span>\r\n                                    <b>Body of email sent to the receiver</b>\r\n                                </span>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>To go back to the website click the below given link.</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <span>\r\n                                    <b><a href ='https://artwear.in/'>Click Here</a></b>\r\n                                </span>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n\r\n</html>";
            string emailBody;

            //Act
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(dir, "PublicPostEmailTemplate.html");
            using (StreamReader reader = File.OpenText(path))
            {
                emailBody = reader.ReadToEnd();
            }
            emailBody = emailBody.Replace("{FullName}", fullname);
            emailBody = emailBody.Replace("{Message}", message);
            emailBody = emailBody.Replace("{Link}", link);

            //Assert
            Assert.IsNotNull(emailBody);
            Assert.AreEqual(expectedEmail, emailBody);
        }
    }

    [TestClass]
    public class GlobalHelper_UnitTest
    {
        [TestMethod]
        public void GlobalHelper_GetCurrentDir_UnitTest_ArthurClive()
        {
            //Arrange
            var expectedPath = "D:\\Arthur_Clive\\UnitTest_ArthurClive\\bin\\Debug\\netcoreapp2.0";

            //Act
            var result = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPath,result);
        }
        
        [TestMethod]
        public void GlobalHelper_ReadXML_UnitTest_ArthurClive()
        {
            //Arrange
            var dir = "D:\\Arthur_Clive\\UnitTest_ArthurClive\\bin\\Debug\\netcoreapp2.0";
            var expectedXElement = "<credentials>\r\n  <!-- Mongo Credentials -->\r\n  <mongo>\r\n    <current>Yes</current>\r\n    <ip>localhost</ip>\r\n    <db>admin</db>\r\n    <user>Ragu</user>\r\n    <password>123ragu</password>\r\n  </mongo>\r\n  <!-- Contents related to email service -->\r\n  <email>\r\n    <current>Yes</current>\r\n    <emailsender>sales@artwear.in</emailsender>\r\n    <emailreceiver>thecrazycub@gmail.com</emailreceiver>\r\n    <websitelink>https://artwear.in/</websitelink>\r\n    <emailsubject1>Message from the Arthur Clive Admin.</emailsubject1>\r\n    <emailsubject2>Verification of your ArthurClive account.</emailsubject2>\r\n    <emailsubject3>Reporting error occured when a user placed order.</emailsubject3>\r\n  </email>\r\n  <!-- IPconfigurations for the project -->\r\n  <ipconfig>\r\n    <current>Yes</current>\r\n    <arthurclive>http://192.168.0.113:5000/</arthurclive>\r\n    <authorizedserver>http://192.168.0.113:5001/</authorizedserver>\r\n    <authorizedserver2>http://localhost:3000/#/verifyemail/</authorizedserver2>\r\n  </ipconfig>\r\n  <ipconfig>\r\n    <current>No</current>\r\n    <arthurclive>http://192.168.0.113:52922/</arthurclive>\r\n    <authorizedserver>http://192.168.0.113:56872/</authorizedserver>\r\n  </ipconfig>\r\n  <!-- PayUMOney Credentials -->\r\n  <payu>\r\n    <current>Yes</current>\r\n    <key>gtKFFx</key>\r\n    <saltkey>eCwWELxi</saltkey>\r\n    <url>https://test.payu.in</url>\r\n    <successurl>http://192.168.0.113:5000/api/payment/success</successurl>\r\n    <failureurl>http://192.168.0.113:5000/api/payment/failed</failureurl>\r\n    <cancelurl>http://192.168.0.113:5000/api/payment/cancel</cancelurl>\r\n    <redirectsuccess>http://localhost:3000/#/paymentsuccess</redirectsuccess>\r\n    <redirectfailure>http://localhost:3000/#/paymenterror</redirectfailure>\r\n    <redirectcancelled>http://localhost:3000/#/paymentcancelled</redirectcancelled>\r\n  </payu>\r\n  <!-- Amazon S3 credentials -->\r\n  <amasons3>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAIUAYVIL7A7I6XECA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amasons3>\r\n  <!-- Amazon SNS credentials -->\r\n  <amazonsns>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAJCDU2723BYGUAHCA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonsns>\r\n  <amazonsns>\r\n    <current>No</current>\r\n    <accesskey>AKIAJ3B7P4FXGYSUXMYA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonsns>\r\n  <!-- Amazon SES credentials -->\r\n  <amazonses>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAIQRMI2NXYVDB7UKA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <amazonses>\r\n    <current>No</current>\r\n    <accesskey>AKIAJ3UPJB4KZY3GTNSA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <amazonses>\r\n    <current>test</current>\r\n    <accesskey>AKIAJZK4MPI7AEGQGUIQ</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <!-- Minio credentials -->\r\n  <minioclient>\r\n    <current>Yes</current>\r\n    <host>localhost:9000</host>\r\n    <accesskey>MinioServer</accesskey>\r\n    <secretkey>123654789@Ragu</secretkey>\r\n  </minioclient>\r\n</credentials>";

            //Act
            var xmlStr = File.ReadAllText(Path.Combine(dir, "AmazonKeys.xml"));
            var result = XElement.Parse(xmlStr);
            var xElementString = result.ToString();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(xElementString,expectedXElement);
        }
    }

    [TestClass]
    public class MinioHelper_UnitTest
    {
        [TestMethod]
        public void MinioHelper_GetMinioClient_UnitTest_ArthurClive()
        {
            //Arrange
            var hostFromXMl = GlobalHelper.ReadXML().Elements("minioclient").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("host").First().Value;
            var accesskeyFromXML = GlobalHelper.ReadXML().Elements("minioclient").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("accesskey").First().Value;
            var secretkeyFromXML = GlobalHelper.ReadXML().Elements("minioclient").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("secretkey").First().Value;
            var expectedAccessKey = "MinioServer";
            var expectedSecretKey = "123654789@Ragu";

            //Act
            var result = new MinioClient(hostFromXMl,accesskeyFromXML,secretkeyFromXML);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(hostFromXMl);
            Assert.IsNotNull(accesskeyFromXML);
            Assert.IsNotNull(secretkeyFromXML);
            Assert.AreEqual(expectedAccessKey,accesskeyFromXML);
            Assert.AreEqual(expectedSecretKey,secretkeyFromXML);
        }

        //Pending
        //[TestMethod]
        public void MinioHelper_GetMinioObject_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }
    }

    [TestClass]
    public class MongoHelper_UnitTest
    {
        [TestMethod]
        public void MongoHelper_GetClient_UnitTest_ArthurClive()
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
            Assert.AreEqual(expectedIp,ipFromXML);
            Assert.AreEqual(expectedUser,userFromXML);
            Assert.AreEqual(expectedPassword,passwordFromXML);
            Assert.AreEqual(expectedDb,dbFromXML);
            Assert.AreEqual(expectedConnectionString,connectionString);
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_GetSingleObject_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_GetListOfObjects_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_UpdateSingleObject_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_DeleteSingleObject_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_CheckForDatas_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_GetOrders_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_GetProducts_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_UpdateProductDetails_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_UpdateCategoryDetails_UnitTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }
    }

    [TestClass]
    public class PayUHelper_UnitTest
    {
        [TestMethod]
        public void PayUHelper_Generatehash512_UnitTest_ArthurClive()
        {
            //Arrange
            string text = "Text to be hashed";
            string hashedText = "9e2c41a3a78cdd2a6d506d6c91ed585c3cc69710aeb3a45c1bc4f3e1891afdbd445a462693065804e0f0f5944dcf3a8b5a0dbc855d3008cadd435487c98e4c4b";
            byte[] message = Encoding.UTF8.GetBytes(text);
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hash = "";

            //Act
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hash += String.Format("{0:x2}", x);
            }
            var result = hash;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(hashedText, result);
        }

        [TestMethod]
        public void PayUHelper_PreparePOSTForm_UnitTest_ArthurClive()
        {
            //Arrange
            var url = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("url").First().Value + "/_payment";
            Hashtable data = new Hashtable();
            data.Add("hash", "9e2c41a3a78cdd2a6d506d6c91ed585c3cc69710aeb3a45c1bc4f3e1891afdbd445a462693065804e0f0f5944dcf3a8b5a0dbc855d3008cadd435487c98e4c4b");
            data.Add("txnid", "477d56ca6f1c3d22552a");
            data.Add("key", GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("key").First().Value);
            string AmountForm = Convert.ToDecimal("100").ToString("g29");
            data.Add("amount", AmountForm);
            data.Add("firstname", "Sample");
            data.Add("email", "sample@gmail.com");
            data.Add("phone", "12341234");
            data.Add("productinfo", "Tshirt");
            data.Add("surl", "SuccessUrl");
            data.Add("furl", "FailureUrl");
            data.Add("lastname", "User");
            data.Add("curl", "");
            data.Add("address1", "SGR street");
            data.Add("address2", "Saravanampati");
            data.Add("city", "Coimbatore");
            data.Add("state", "TamilNade");
            data.Add("country", "India");
            data.Add("zipcode", "641035");
            data.Add("udf1", "");
            data.Add("udf2", "");
            data.Add("udf3", "");
            data.Add("udf4", "");
            data.Add("udf5", "");
            data.Add("pg", "");
            data.Add("service_provider", "PayUMoney");
            string formID = "PostForm";
            var expectedUrl = "https://test.payu.in/_payment";

            //Act 
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");
            foreach (DictionaryEntry key in data)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + key.Key + "\" value=\"" + key.Value + "\">");
            }
            strForm.Append("</form>");
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            strForm.Append(strScript);
            var result = strForm;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUrl,url);
        }

        [TestMethod]
        public void PayUHelper_GetTxnId_UnitTest_ArthurClive()
        {
            //Arrange
            Random random = new Random();

            //Act
            string strHash = PayUHelper.Generatehash512(random.ToString() + DateTime.Now);
            string txnId = strHash.ToString().Substring(0, 20);
            var result = txnId;

            //Assert
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void PayUHelper_GetHashString_UnitTest_ArthurClive()
        {
            //Arrange
            var keyFromXML = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("key").First().Value;
            var saltkeyFromXML = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("saltkey").First().Value;
            var txnId = "477d56ca6f1c3d22552a";
            PaymentModel paymentModel = new PaymentModel
            {
                Amount = "100",
                ProductInfo = "Tshirt",
                FirstName = "Sample",
                Email = "sample@gmail.com"
            };
            var requiredHashString = "gtKFFx|477d56ca6f1c3d22552a|100|Tshirt|Sample|sample@gmail.com|||||||||||eCwWELxi";
            string[] hashSequence = ("key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10").Split('|');
            string hashString = "";
            var expectedKey = "gtKFFx";
            var expectedSaltKey = "eCwWELxi";
            var expectHashString = "gtKFFx|477d56ca6f1c3d22552a|100|Tshirt|Sample|sample@gmail.com|||||||||||eCwWELxi";

            //Act string hashString = "";
            foreach (string hash_var in hashSequence)
            {
                if (hash_var == "key")
                {
                    hashString = hashString + GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("key").First().Value;
                    hashString = hashString + '|';
                }
                else if (hash_var == "txnid")
                {
                    hashString = hashString + txnId;
                    hashString = hashString + '|';
                }
                else if (hash_var == "amount")
                {
                    hashString = hashString + Convert.ToDecimal(paymentModel.Amount).ToString("g29");
                    hashString = hashString + '|';
                }
                else if (hash_var == "productinfo")
                {
                    hashString = hashString + paymentModel.ProductInfo;
                    hashString = hashString + '|';
                }
                else if (hash_var == "firstname")
                {
                    hashString = hashString + paymentModel.FirstName;
                    hashString = hashString + '|';
                }
                else if (hash_var == "email")
                {
                    hashString = hashString + paymentModel.Email;
                    hashString = hashString + '|';
                }
                else
                {
                    hashString = hashString + "";
                    hashString = hashString + '|';
                }
            }
            hashString += GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("saltkey").First().Value;
            var result = hashString;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(requiredHashString, result);
            Assert.IsNotNull(keyFromXML);
            Assert.IsNotNull(saltkeyFromXML);
            Assert.AreEqual(expectedKey,keyFromXML);
            Assert.AreEqual(expectedSaltKey,saltkeyFromXML);
            Assert.AreEqual(expectHashString,hashString);
        }
    }
}

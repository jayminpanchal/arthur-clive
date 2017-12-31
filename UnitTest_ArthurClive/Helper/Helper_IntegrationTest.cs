using Arthur_Clive.Helper;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using Minio;
using Amazon.S3;
using MongoDB.Driver;
using MongoDB.Bson;
using Arthur_Clive.Data;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;

namespace UnitTest_ArthurClive.Helper
{
    [TestClass]
    public class AmazonHelper_IntegrationTest
    {
        [TestMethod]
        public void AmazonHelper_GetAmazonS3Client_IntegrationTest_ArthurClive()
        {
            //Arrange
            var accessKeyFromXML = GlobalHelper.ReadXML().Elements("amasons3").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("accesskey").FirstOrDefault().Value;
            var secretKeyFromXML = GlobalHelper.ReadXML().Elements("amasons3").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("secretkey").FirstOrDefault().Value;
            var expectedAccessKey = "AKIAIUAYVIL7A7I6XECA";
            var expectedSecretKey = "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";

            //Act
            var result = AmazonHelper.GetAmazonS3Client() as IAmazonS3;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(accessKeyFromXML);
            Assert.IsNotNull(secretKeyFromXML);
            Assert.AreEqual(expectedAccessKey, accessKeyFromXML);
            Assert.AreEqual(expectedSecretKey, secretKeyFromXML);
        }

        //[TestMethod]
        public void AmazonHelper_GetAmazonS3Object_IntegrationTest_ArthurClive()
        {
            //Arrange
            var bucketName = "arthurclive-products";
            var objectName = "All-Art-Bangalore-Black-.jpg";

            //Act
            var result = AmazonHelper.GetAmazonS3Object(bucketName, objectName) as string;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AmazonHelper_GetS3Object_IntegrationTest_ArthurClive()
        {
            //Arrange
            var bucketName = "arthurclive-products";
            var objectName = "All-Art-Bangalore-Black-.jpg";
            var expectedResult = "https://s3.ap-south-1.amazonaws.com/" + bucketName + "/" + objectName;

            //Act
            var result = AmazonHelper.GetS3Object(bucketName, objectName) as string;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }
    }

    [TestClass]
    public class EmailHelper_IntegrationTest
    {
        [TestMethod]
        public void EmailHelper_GetCredentials_IntegrationTest_ArthurClive()
        {
            //Arrange
            var key1 = "accesskey";
            var key2 = "secretkey";
            var accesskeyFromXml = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("test")).Descendants(key1).FirstOrDefault().Value;
            var secretkeyFromXml = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("test")).Descendants(key2).FirstOrDefault().Value;

            //Act
            var accessKey = EmailHelper.GetCredentials(key1) as string;
            var secretKey = EmailHelper.GetCredentials(key2) as string;

            //Assert
            Assert.IsNotNull(accessKey);
            Assert.IsNotNull(secretKey);
            Assert.IsNotNull(accesskeyFromXml);
            Assert.IsNotNull(secretKey);
        }

        //[TestMethod]
        public void EmailHelper_SendEmail_IntegrationTest_ArthurClive()
        {
            //Arrange
            var fullNameOfReceiver = "SampleUser";
            var emailOfReceiver = "sample@gmail.com";
            var message = "Message to be sent to user through email";

            //Act
            var result = EmailHelper.SendEmail_ToUsers(fullNameOfReceiver, emailOfReceiver, message).Result as string;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, "Success");
        }

        [TestMethod]
        public void EmailHelper_CreateEmailBody_IntegrationTest_ArthurClive()
        {
            //Arrange
            var fullname = "Sample user";
            var message = "Body of email sent to the receiver";
            var link = "<a href ='" + GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("websitelink").First().Value + "'>Click Here</a>";
            var expectedEmail = "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n\r\n<head>\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n    <title>Message from team Arthur Clive</title>\r\n</head>\r\n\r\n<body>\r\n    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n        <tr>\r\n            <td align=\"center\" valign=\"top\" bgcolor=\"#ffe77b\" style=\"background-color:#dec786;\">\r\n                <br>\r\n                <br>\r\n                <table width=\"600\" height=\"150\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                    <tr>\r\n                        <td align=\"left\" valign=\"top\" bgcolor=\"#564319\" style=\"background-color:#0c0d0d; font-family:Arial, Helvetica, sans-serif; padding:10px;\">\r\n                            <div align=center style=\"font-size:36px; color:#ffffff;\">\r\n                                <img src=\"D:\\Arthur_Clive\\netcore\\AuthorizedServer\\EmailTemplate\\logo-caption.png\" width=\"200px\" height=\"120px\" />\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <span>\r\n                                    <b>Sample user</b>\r\n                                </span>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>Team Arthur Clive Welcomes you back...</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>This is a message from team Arthur Clive</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;color:red\">\r\n                                <span>\r\n                                    <b>Body of email sent to the receiver</b>\r\n                                </span>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <b>To go back to the website click the below given link.</b>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=center style=\"background-color:#ffffff;padding:15px; font-family:Arial, Helvetica, sans-serif;\">\r\n                            <div style=\"font-size:20px;\">\r\n                                <span>\r\n                                    <b><a href ='https://artwear.in/'>Click Here</a></b>\r\n                                </span>\r\n                            </div>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n\r\n</html>";

            //Act
            var result = EmailHelper.CreateEmailBody(fullname, link, message) as string;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedEmail, result);
        }
    }


    [TestClass]
    public class GlobalHelper_IntegrationTest
    {
        [TestMethod]
        public void GlobalHelper_GetCurrentDir_IntegrationTest_ArthurClive()
        {
            //Arrange

            //Act
            var result = GlobalHelper.GetCurrentDir() as string;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GlobalHelper_ReadXML_IntegrationTest_ArthurClive()
        {
            //Arrange
            var expectedXElement = "<credentials>\r\n  <!-- Mongo Credentials -->\r\n  <mongo>\r\n    <current>Yes</current>\r\n    <ip>localhost</ip>\r\n    <db>admin</db>\r\n    <user>Ragu</user>\r\n    <password>123ragu</password>\r\n  </mongo>\r\n  <!-- Contents related to email service -->\r\n  <email>\r\n    <current>Yes</current>\r\n    <emailsender>sales@artwear.in</emailsender>\r\n    <emailreceiver>thecrazycub@gmail.com</emailreceiver>\r\n    <websitelink>https://artwear.in/</websitelink>\r\n    <emailsubject1>Message from the Arthur Clive Admin.</emailsubject1>\r\n    <emailsubject2>Verification of your ArthurClive account.</emailsubject2>\r\n    <emailsubject3>Reporting error occured when a user placed order.</emailsubject3>\r\n  </email>\r\n  <!-- IPconfigurations for the project -->\r\n  <ipconfig>\r\n    <current>Yes</current>\r\n    <arthurclive>http://192.168.0.113:5000/</arthurclive>\r\n    <authorizedserver>http://192.168.0.113:5001/</authorizedserver>\r\n    <authorizedserver2>http://localhost:3000/#/verifyemail/</authorizedserver2>\r\n  </ipconfig>\r\n  <ipconfig>\r\n    <current>No</current>\r\n    <arthurclive>http://192.168.0.113:52922/</arthurclive>\r\n    <authorizedserver>http://192.168.0.113:56872/</authorizedserver>\r\n  </ipconfig>\r\n  <!-- PayUMOney Credentials -->\r\n  <payu>\r\n    <current>Yes</current>\r\n    <key>gtKFFx</key>\r\n    <saltkey>eCwWELxi</saltkey>\r\n    <url>https://test.payu.in</url>\r\n    <successurl>http://192.168.0.113:5000/api/payment/success</successurl>\r\n    <failureurl>http://192.168.0.113:5000/api/payment/failed</failureurl>\r\n    <cancelurl>http://192.168.0.113:5000/api/payment/cancel</cancelurl>\r\n    <redirectsuccess>http://localhost:3000/#/paymentsuccess</redirectsuccess>\r\n    <redirectfailure>http://localhost:3000/#/paymenterror</redirectfailure>\r\n    <redirectcancelled>http://localhost:3000/#/paymentcancelled</redirectcancelled>\r\n  </payu>\r\n  <!-- Amazon S3 credentials -->\r\n  <amasons3>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAIUAYVIL7A7I6XECA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amasons3>\r\n  <!-- Amazon SNS credentials -->\r\n  <amazonsns>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAJCDU2723BYGUAHCA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonsns>\r\n  <amazonsns>\r\n    <current>No</current>\r\n    <accesskey>AKIAJ3B7P4FXGYSUXMYA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonsns>\r\n  <!-- Amazon SES credentials -->\r\n  <amazonses>\r\n    <current>Yes</current>\r\n    <accesskey>AKIAIQRMI2NXYVDB7UKA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <amazonses>\r\n    <current>No</current>\r\n    <accesskey>AKIAJ3UPJB4KZY3GTNSA</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <amazonses>\r\n    <current>test</current>\r\n    <accesskey>AKIAJZK4MPI7AEGQGUIQ</accesskey>\r\n    <secretkey>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</secretkey>\r\n  </amazonses>\r\n  <!-- Minio credentials -->\r\n  <minioclient>\r\n    <current>Yes</current>\r\n    <host>localhost:9000</host>\r\n    <accesskey>MinioServer</accesskey>\r\n    <secretkey>123654789@Ragu</secretkey>\r\n  </minioclient>\r\n</credentials>";

            //Act
            var result = GlobalHelper.ReadXML() as XElement;
            var xElementString = result.ToString();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedXElement, xElementString);
        }
    }

    [TestClass]
    public class MinioHelper_IntegrationTest
    {
        [TestMethod]
        public void MinioHelper_GetMinioClient_IntegrationTest_ArthurClive()
        {
            //Arrange
            var hostFromXMl = GlobalHelper.ReadXML().Elements("minioclient").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("host").First().Value;
            var accesskeyFromXML = GlobalHelper.ReadXML().Elements("minioclient").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("accesskey").First().Value;
            var secretkeyFromXML = GlobalHelper.ReadXML().Elements("minioclient").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("secretkey").First().Value;

            //Act
            var result = MinioHelper.GetMinioClient() as MinioClient;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(hostFromXMl);
            Assert.IsNotNull(accesskeyFromXML);
            Assert.IsNotNull(secretkeyFromXML);
        }

        //[TestMethod]
        public void MinioHelper_GetMinioObject_IntegrationTest_ArthurClive()
        {
            //Arrange
            var bucketName = "products";
            var objectName = "All-Art-Om-Black-.jpg";

            //Act
            var result = MinioHelper.GetMinioObject(bucketName, objectName).Result as string;

            //Assert
            Assert.IsNotNull(result);
        }
    }

    [TestClass]
    public class MongoHelper_IntegrationTest
    {
        public MongoHelper_IntegrationTest()
        {
            IntegrationTest_ArthurCliveHelper_Helper.InsertData_SampleCategory();
            IntegrationTest_ArthurCliveHelper_Helper.InsertData_SampleOrder();
            IntegrationTest_ArthurCliveHelper_Helper.InsertData_SampleProduct();
        }

        [TestMethod]
        public void MongoHelper_GetClient_IntegrationTest_ArthurClive()
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

        //[TestMethod]
        public void MongoHelper_GetSingleObject_IntegrationTest_ArthurClive()
        {
            //Arrange
            IMongoDatabase db = MongoHelper._client.GetDatabase("UnitTestDB");
            var collection = db.GetCollection<Category>("Category");
            var productFor = "Boys";
            var productType = "Tshirt";
            var filter = Builders<BsonDocument>.Filter.Eq("ProductFor", productFor) & Builders<BsonDocument>.Filter.Eq("ProductType", productType);
            var dbName = "ProductDB";
            var collectionName = "Category";
            var expectedUrl = "https://s3.ap-south-1.amazonaws.com/product-category/Boys-Tshirt.jpg";
            var expectedDescription = "Tshirts for boys";
            var expectedProductFor = "Boys";
            var expectedProductType = "Tshirt";

            //Act
            var result = BsonSerializer.Deserialize<Category>(MongoHelper.GetSingleObject(filter, dbName, collectionName).Result) as Category;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUrl, result.MinioObject_URL);
            Assert.AreEqual(expectedDescription, result.Description);
            Assert.AreEqual(expectedProductFor, result.ProductFor);
            Assert.AreEqual(expectedProductType, result.ProductType);
        }

        //[TestMethod]
        public void MongoHelper_GetListOfObjects_IntegrationTest_ArthurClive()
        {
            //Arrange
            var dbName = "RolesDB";
            var collectionName = "Roles";
            var roleList = new List<Roles>();
            var expectedRoleCount = 3;

            //Act
            var result = MongoHelper.GetListOfObjects(null, null, null, null, null, null, dbName, collectionName).Result as List<BsonDocument>;
            foreach (var data in result)
            {
                roleList.Add(BsonSerializer.Deserialize<Roles>(data));
            }

            //Assert
            Assert.IsNotNull(roleList);
            Assert.AreEqual(expectedRoleCount, roleList.Count());
        }

        //[TestMethod]
        public void MongoHelper_UpdateSingleObject_IntegrationTest_ArthurClive()
        {
            //Arrange
            var filter = Builders<BsonDocument>.Filter.Eq("ProductFor", "Men") & Builders<BsonDocument>.Filter.Eq("ProductType", "Tshirt");
            var updateDefinition = Builders<BsonDocument>.Update.Set("ProductFor", "Women");

            //Act
            var result = MongoHelper.UpdateSingleObject(filter, "UnitTestDB", "Category", updateDefinition).Result as bool?;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, true);
        }

        //[TestMethod]
        public void MongoHelper_DeleteSingleObject_IntegrationTest_ArthurClive()
        {
            //Arrange
            var filter = Builders<BsonDocument>.Filter.Eq("ProductFor", "Women") & Builders<BsonDocument>.Filter.Eq("ProductType", "Tshirt");
            var dbName = "UnitTestDB";
            var collectionName = "Category";

            //Act
            var result = MongoHelper.DeleteSingleObject(filter, dbName, collectionName) as bool?;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, true);
        }

        //[TestMethod]
        public void MongoHelper_CheckForDatas_IntegrationTest_ArthurClive()
        {
            //Arrange
            //UnitTestHelper.InsertData_SampleCategory();
            var dbName = "UnitTestDB";
            var collectionName = "Category";

            //Act
            var result = BsonSerializer.Deserialize<Category>(MongoHelper.CheckForDatas("ProductFor", "Men", "ProductType", "Tshirt", dbName, collectionName)) as Category;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ProductFor, "Men");
            Assert.AreEqual(result.ProductType, "Tshirt");
        }
        
        //[TestMethod]
        public void MongoHelper_GetProducts_IntegrationTest_ArthurClive()
        {
            //Arrange
            IMongoDatabase db = MongoHelper._client.GetDatabase("UnitTestDB");
            var productSKU = "";

            //Act
            var result = MongoHelper.GetProducts(productSKU,db).Result as List<Product>;

            //Assert
            Assert.IsNotNull(result);
            foreach(var product in result)
            {
                Assert.AreEqual(product.ProductSKU,productSKU);
            }
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_UpdateProductDetails_IntegrationTest_ArthurClive()
        {
            //Arrange
            var productData = BsonSerializer.Deserialize<Product>(MongoHelper.CheckForDatas("ProductSKU", "All-Art-Bangalore-Black-",null,null,"UnitTestDB","Product"));
            var objectId = productData.Id;
            var updateDesign = "Calcutta";
            var updateSKU = "All-Art-Calcutta-Black-";

            //Act
            var result = MongoHelper.UpdateProductDetails(objectId,updateDesign,"ProductDesign",updateSKU).Result as bool?;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result,true);
        }

        //Pending
        //[TestMethod]
        public void MongoHelper_UpdateCategoryDetails_IntegrationTest_ArthurClive()
        {
            //Arrange

            //Act

            //Assert
        }
    }

    [TestClass]
    public class PayUHelper_IntegrationTest
    {
        [TestMethod]
        public void PayUHelper_Generatehash512_IntegrationTest_ArthurClive()
        {
            //Arrange
            string text = "Text to be hashed";
            string hashedText = "9e2c41a3a78cdd2a6d506d6c91ed585c3cc69710aeb3a45c1bc4f3e1891afdbd445a462693065804e0f0f5944dcf3a8b5a0dbc855d3008cadd435487c98e4c4b";

            //Act
            var result = PayUHelper.Generatehash512(text) as string;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(hashedText, result);
        }

        [TestMethod]
        public void PayUHelper_PreparePOSTForm_IntegrationTest_ArthurClive()
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

            //Act 
            var result = PayUHelper.PreparePOSTForm(url, data) as string;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PayUHelper_GetTxnId_IntegrationTest_ArthurClive()
        {
            //Arrange

            //Act
            var result = PayUHelper.GetTxnId() as string;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PayUHelper_GetHashString_IntegrationTest_ArthurClive()
        {
            //Arrange
            var keyFromXML = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("key").First().Value;
            var salekeyFromXML = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("saltkey").First().Value;
            var txnId = "477d56ca6f1c3d22552a";
            PaymentModel paymentModel = new PaymentModel
            {
                Amount = "100",
                ProductInfo = "Tshirt",
                FirstName = "Sample",
                Email = "sample@gmail.com"
            };
            var hastString = "gtKFFx|477d56ca6f1c3d22552a|100.00|Tshirt|Sample|sample@gmail.com|0||||||||||eCwWELxi";

            //Act
            var result = PayUHelper.GetHashString(txnId, paymentModel) as string;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(hastString, result);
            Assert.IsNotNull(keyFromXML);
            Assert.IsNotNull(salekeyFromXML);
        }
    }
}

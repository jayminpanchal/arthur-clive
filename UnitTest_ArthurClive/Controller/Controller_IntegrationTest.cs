using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arthur_Clive.Controllers;
using Arthur_Clive.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MH = Arthur_Clive.Helper.MongoHelper;
using TH = UnitTest_ArthurClive.Controller.Integrationtest_ArthurCliveController_Helper;
using PH = Arthur_Clive.Helper.PayUHelper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace UnitTest_ArthurClive.Controller
{
    [TestClass]
    public class AdminController_IntegrationTest
    {
        public AdminController controller = new AdminController();
        
        [TestMethod]
        public void AdminController_Subscribe_IntegrationTest_ArthurClive()
        {
            //Arrange
            var username = "sample@gmail.com";
            var expectedCode = "200";
            var expectedMessage = "Subscribed Succesfully";

            //Act
            var result = controller.Subscribe(username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<Subscribe>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "SubscribeDB", "SubscribedUsers").Result);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.UserName, username);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "SubscribeDB", "SubscribedUsers");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "SubscribeDB", "SubscribedUsers");
            }
        }

        [TestMethod]
        public void AdminController_Unsubscribe_IntegrationTest_ArthurClive()
        {
            //Arrange
            var username = "sample@gmail.com";
            var expectedCode = "200";
            var expectedMessage = "Unsubscribed Succesfully";

            //Insert data to db for testing
            var subscribe = TH.DeserializedResponceData(controller.Subscribe(username).Result.ToJson());

            //Act
            var result = controller.Unsubscribe(username) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check if user is unsubscribed
            var unsubscribedUser = MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "SubscribeDB", "SubscribedUsers").Result;

            //Assert
            Assert.IsNotNull(subscribe);
            Assert.AreEqual(subscribe.Code, expectedCode);
            Assert.AreEqual(subscribe.Message, "Subscribed Succesfully");
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.IsNull(unsubscribedUser);
        }

        [TestMethod]
        public void AdminController_PublicPost_IntegrationTest_ArthurClive()
        {
            //Arrange
            var username = "raguvarthan.n@turingminds.com";
            var message = "This is a message from the admin of ArthuClive";
            var expectedCode = "200";
            var expectedMessage = "Email sent to all subscribed users";

            //Insert data to db for testing
            var subscribe = TH.DeserializedResponceData(controller.Subscribe(username).Result.ToJson());

            //Act
            var result = controller.PublicPost(message) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Assert
            Assert.IsNotNull(subscribe);
            Assert.AreEqual(subscribe.Code, expectedCode);
            Assert.AreEqual(subscribe.Message, "Subscribed Succesfully");
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "SubscribeDB", "SubscribedUsers");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "SubscribeDB", "SubscribedUsers");
            }
        }
    }

    [TestClass]
    public class CategoryController_IntegrationTest
    {
        public CategoryController controller = new CategoryController();

        [TestMethod]
        public void CategoryController_Get_IntegrationTest_ArthurClive()
        {
            //Arrange
            var expectedCode = "200";
            var expectedMessage = "Success";
            var expectedDataCount = 6;

            //Act
            var result = controller.Get() as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData_CategoryController_Get(result.Result.ToJson());

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.IsNotNull(responseData.Data);
            Assert.AreEqual(responseData.Data._v.Count, expectedDataCount);
        }

        [TestMethod]
        public void CategoryController_Post_IntegrationTest_ArthurClive()
        {
            //Arrange
            Arthur_Clive.Data.Category category = new Arthur_Clive.Data.Category
            {
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                Description = "SampleDescription"
            };
            var expectedCode = "200";
            var expectedMessage = "Inserted";
            var expectedUrl = "https://s3.ap-south-1.amazonaws.com/product-category/SampleFor-SampleType.jpg";

            //Act
            var result = controller.Post(category) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<Arthur_Clive.Data.Category>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("ProductFor", category.ProductFor) & Builders<BsonDocument>.Filter.Eq("ProductType", category.ProductType), "ProductDB", "Category").Result);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.ProductFor, category.ProductFor);
            Assert.AreEqual(insertedData.ProductType, category.ProductType);
            Assert.AreEqual(insertedData.Description, category.Description);
            Assert.AreEqual(insertedData.MinioObject_URL, expectedUrl);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("ProductFor", category.ProductFor, "ProductType", category.ProductType, "ProductDB", "Category");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("ProductFor", category.ProductFor) & Builders<BsonDocument>.Filter.Eq("ProductType", category.ProductType), "ProductDB", "Category");
            }
        }

        [TestMethod]
        public void CategoryController_Delete_IntegrationTest_ArthurClive()
        {
            //Arrange
            Arthur_Clive.Data.Category category = new Arthur_Clive.Data.Category
            {
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                Description = "SampleDescription"
            };
            var expectedCode = "200";
            var expectedMessage = "Deleted";

            //Insert data to db for testing
            var insertCategory = TH.DeserializedResponceData(controller.Post(category).Result.ToJson());

            //Act
            var result = controller.Delete(category.ProductFor, category.ProductType) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check if data is deleted
            var deletedData = MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("ProductFor", category.ProductFor) & Builders<BsonDocument>.Filter.Eq("ProductType", category.ProductType), "ProductDB", "Category").Result;

            //Assert
            Assert.IsNotNull(insertCategory);
            Assert.AreEqual(insertCategory.Code, expectedCode);
            Assert.AreEqual(insertCategory.Message, "Inserted");
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.IsNull(deletedData);
        }

        [TestMethod]
        public void CategoryController_Update_IntegrationTest_ArthurClive()
        {

            //Arrange
            Arthur_Clive.Data.Category category = new Arthur_Clive.Data.Category
            {
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                Description = "SampleDescription"
            };
            Arthur_Clive.Data.UpdateCategory updateCategory = new Arthur_Clive.Data.UpdateCategory
            {
                ProductFor = "UpdatedSampleFor",
                ProductType = "UpdatedSampleType",
                Description = "UpdatedSampleDescription"
            };
            var expectedCode = "200";
            var expectedMessage = "Updated";
            var expectedUrl = "https://s3.ap-south-1.amazonaws.com/product-category/UpdatedSampleFor-UpdatedSampleType.jpg";

            //Insert data to db for testing
            var insertCategory = TH.DeserializedResponceData(controller.Post(category).Result.ToJson());

            //Act
            var result = controller.Update(updateCategory, category.ProductFor, category.ProductType) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var updatedData = BsonSerializer.Deserialize<Arthur_Clive.Data.Category>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("ProductFor", updateCategory.ProductFor) & Builders<BsonDocument>.Filter.Eq("ProductType", updateCategory.ProductType), "ProductDB", "Category").Result);

            //Assert
            Assert.IsNotNull(insertCategory);
            Assert.AreEqual(insertCategory.Code, expectedCode);
            Assert.AreEqual(insertCategory.Message, "Inserted");
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.IsNotNull(updatedData);
            Assert.AreEqual(updatedData.ProductFor, updateCategory.ProductFor);
            Assert.AreEqual(updatedData.ProductType, updateCategory.ProductType);
            Assert.AreEqual(updatedData.Description, updateCategory.Description);
            Assert.AreEqual(updatedData.MinioObject_URL, expectedUrl);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("ProductFor", updateCategory.ProductFor, "ProductType", updateCategory.ProductType, "ProductDB", "Category");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("ProductFor", updateCategory.ProductFor) & Builders<BsonDocument>.Filter.Eq("ProductType", updateCategory.ProductType), "ProductDB", "Category");
            }
        }
    }

    [TestClass]
    public class ProductController_IntegrationTest
    {
        public ProductController controller = new ProductController();

        [TestMethod]
        public void ProductController_Get_IntegrationTest_ArthurClive()
        {
            //Arrange
            var expectedCode = "200";
            var expectedMessage = "Success";
            var expectedDataCount = 67;

            //Act
            var result = controller.Get() as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData_ProductController_Get(result.Result.ToJson());

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(responseData.Data._v.Count, expectedDataCount);
        }

        [TestMethod]
        public void ProductController_Post_IntegrationTest_ArthurClive()
        {
            //Arrange
            Arthur_Clive.Data.Product product = new Arthur_Clive.Data.Product
            {
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                ProductDesign = "SampleDesign",
                ProductBrand = "Arthur Clive",
                ProductPrice = 100,
                ProductDiscount = 10,
                ProductStock = 1,
                ProductSize = "S",
                ProductColour = "Black",
                RefundApplicable = true,
                ReplacementApplicable = true,
                ProductDescription = "SampleDescription",
                ProductMaterial = "Cotton",
            };
            var productSKU = "SampleFor-SampleType-SampleDesign-Black-S";
            var expectedCode = "200";
            var expectedMessage = "Inserted";
            var expectedUrl = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/SampleFor-SampleType-SampleDesign-Black-S.jpg";
            var expectedDiscountPrice = 90;

            //Act
            var result = controller.Post(product) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<Arthur_Clive.Data.Product>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("ProductSKU", product.ProductSKU), "ProductDB", "Product").Result);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.ProductFor, product.ProductFor);
            Assert.AreEqual(insertedData.ProductType, product.ProductType);
            Assert.AreEqual(insertedData.ProductDesign, product.ProductDesign);
            Assert.AreEqual(insertedData.ProductBrand, product.ProductBrand);
            Assert.AreEqual(insertedData.ProductPrice, product.ProductPrice);
            Assert.AreEqual(insertedData.ProductDiscount, product.ProductDiscount);
            Assert.AreEqual(insertedData.ProductStock, product.ProductStock);
            Assert.AreEqual(insertedData.ProductSize, product.ProductSize);
            Assert.AreEqual(insertedData.ProductColour, product.ProductColour);
            Assert.AreEqual(insertedData.RefundApplicable, product.RefundApplicable);
            Assert.AreEqual(insertedData.ReplacementApplicable, product.ReplacementApplicable);
            Assert.AreEqual(insertedData.ProductDescription, product.ProductDescription);
            Assert.AreEqual(insertedData.ProductMaterial, product.ProductMaterial);
            Assert.AreEqual(insertedData.ProductSKU, productSKU);
            Assert.AreEqual(insertedData.MinioObject_URL, expectedUrl);
            Assert.AreEqual(insertedData.ProductDiscountPrice, expectedDiscountPrice);
            Assert.AreEqual(insertedData.ProductRating, 0);
            Assert.IsNull(insertedData.ProductReviews);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("ProductSKU", product.ProductSKU, null, null, "ProductDB", "Product");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("ProductSKU", product.ProductSKU), "ProductDB", "Product");
            }
        }

        [TestMethod]
        public void ProductController_Delete_IntegrationTest_ArthurClive()
        {
            //Arrange
            Arthur_Clive.Data.Product product = new Arthur_Clive.Data.Product
            {
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                ProductDesign = "SampleDesign",
                ProductBrand = "Arthur Clive",
                ProductPrice = 100,
                ProductDiscount = 10,
                ProductStock = 1,
                ProductSize = "S",
                ProductColour = "Black",
                RefundApplicable = true,
                ReplacementApplicable = true,
                ProductDescription = "SampleDescription",
                ProductMaterial = "Cotton",
            };
            var expectedCode = "200";
            var expectedMessage = "Deleted";

            //Insert data to db for testing
            var insertProduct = TH.DeserializedResponceData(controller.Post(product).Result.ToJson());

            //Act
            var result = controller.Delete(product.ProductSKU) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check if data is deleted
            var deletedData = MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("ProductSKU", product.ProductSKU), "ProductDB", "Product").Result;

            //Assert
            Assert.IsNotNull(insertProduct);
            Assert.AreEqual(insertProduct.Code, expectedCode);
            Assert.AreEqual(insertProduct.Message, "Inserted");
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.IsNull(deletedData);
        }

        [TestMethod]
        public void ProductController_Update_IntegrationTest_ArthurClive()
        {

            //Arrange
            Arthur_Clive.Data.Product product = new Arthur_Clive.Data.Product
            {
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                ProductDesign = "SampleDesign",
                ProductBrand = "Arthur Clive",
                ProductPrice = 100,
                ProductDiscount = 10,
                ProductStock = 1,
                ProductSize = "S",
                ProductColour = "Black",
                RefundApplicable = true,
                ReplacementApplicable = true,
                ProductDescription = "SampleDescription",
                ProductMaterial = "Cotton",
            };
            UpdateProduct updateProduct = new UpdateProduct
            {
                ProductFor = "UpdatedSampleFor",
                ProductType = "UpdatedSampleType",
                ProductDesign = "UpdatedSampleDesign",
                ProductBrand = "Arthur Clive",
                ProductPrice = 200,
                ProductDiscount = 20,
                ProductStock = 2,
                ProductSize = "L",
                ProductColour = "White",
                RefundApplicable = false,
                ReplacementApplicable = false,
                ProductDescription = "UpdatedSampleDescription",
                ProductMaterial = "Cotton",
            };
            var expectedCode = "200";
            var expectedMessage = "Updated";
            var expectedUrl = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/UpdatedSampleFor-UpdatedSampleType-UpdatedSampleDesign-White-L.jpg";
            var expectedDiscountPrice = 160;
            var updatedProductSKU = "UpdatedSampleFor-UpdatedSampleType-UpdatedSampleDesign-White-L";

            //Insert data to db for testing
            var insertProduct = TH.DeserializedResponceData(controller.Post(product).Result.ToJson());

            //Act
            var result = controller.Update(updateProduct, product.ProductSKU) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var updatedData = BsonSerializer.Deserialize<Arthur_Clive.Data.Product>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("ProductSKU", updatedProductSKU), "ProductDB", "Product").Result);

            //Assert
            Assert.IsNotNull(insertProduct);
            Assert.AreEqual(insertProduct.Code, expectedCode);
            Assert.AreEqual(insertProduct.Message, "Inserted");
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.IsNotNull(updatedData);
            Assert.AreEqual(updatedData.ProductFor, updateProduct.ProductFor);
            Assert.AreEqual(updatedData.ProductType, updateProduct.ProductType);
            Assert.AreEqual(updatedData.ProductDesign, updateProduct.ProductDesign);
            Assert.AreEqual(updatedData.ProductBrand, updateProduct.ProductBrand);
            Assert.AreEqual(updatedData.ProductPrice, updateProduct.ProductPrice);
            Assert.AreEqual(updatedData.ProductDiscount, updateProduct.ProductDiscount);
            Assert.AreEqual(updatedData.ProductStock, updateProduct.ProductStock);
            Assert.AreEqual(updatedData.ProductSize, updateProduct.ProductSize);
            Assert.AreEqual(updatedData.ProductColour, updateProduct.ProductColour);
            Assert.AreEqual(updatedData.RefundApplicable, updateProduct.RefundApplicable);
            Assert.AreEqual(updatedData.ReplacementApplicable, updateProduct.ReplacementApplicable);
            Assert.AreEqual(updatedData.ProductDescription, updateProduct.ProductDescription);
            Assert.AreEqual(updatedData.ProductMaterial, updateProduct.ProductMaterial);
            Assert.AreEqual(updatedData.MinioObject_URL, expectedUrl);
            Assert.AreEqual(updatedData.ProductDiscountPrice, expectedDiscountPrice);
            Assert.AreEqual(updatedData.MinioObject_URL, expectedUrl);
            Assert.AreEqual(updatedData.ProductDiscountPrice, expectedDiscountPrice);
            Assert.AreEqual(updatedData.ProductRating, 0);
            Assert.IsNull(updatedData.ProductReviews);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("ProductSKU", updatedProductSKU, null, null, "ProductDB", "Product");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("ProductSKU", updatedProductSKU), "ProductDB", "Product");
            }
        }
    }

    [TestClass]
    public class SubCategoryController_IntegrationTest
    {
        public SubCategoryController controller = new SubCategoryController();

        [TestMethod]
        public void SubCategoryController_Get_IntegrationTest_ArthurClive()
        {
            //Arrange
            var expectedCode = "200";
            var expectedMessage = "Success";
            var expectedDataCount_Men_Tshirt = 20;
            var expectedDataCount_Women_Tshirt = 20;
            var expectedDataCount_Boys_Tshirt = 8;
            var expectedDataCount_Girls_Tshirt = 8;
            var expectedDataCount_WallArt = 6;
            var expectedDataCount_Gifts = 5;
            CategoryController categoryController = new CategoryController();

            //Act
            var categories = TH.DeserializedResponceData_CategoryController_Get(categoryController.Get().Result.ToJson()).Data._v;
            foreach(var category in categories)
            {
                var result = controller.Get(category.ProductFor, category.ProductType) as Task<ActionResult>;
                var responseData = TH.DeserializedResponceData_ProductController_Get(result.Result.ToJson());

                //Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(responseData.Code, expectedCode);
                Assert.AreEqual(responseData.Message, expectedMessage);
                if (category.ProductFor == "Men" & category.ProductType == "Tshirt")
                {
                    Assert.AreEqual(responseData.Data._v.Count, expectedDataCount_Men_Tshirt);
                }
                else if (category.ProductFor == "Women" & category.ProductType == "Tshirt")
                {
                    Assert.AreEqual(responseData.Data._v.Count, expectedDataCount_Women_Tshirt);
                }
                else if (category.ProductFor == "Boys" & category.ProductType == "Tshirt")
                {
                    Assert.AreEqual(responseData.Data._v.Count, expectedDataCount_Boys_Tshirt);
                }
                else if (category.ProductFor == "Girls" & category.ProductType == "Tshirt")
                {
                    Assert.AreEqual(responseData.Data._v.Count, expectedDataCount_Girls_Tshirt);
                }
                else if (category.ProductFor == "All" & category.ProductType == "Art")
                {
                    Assert.AreEqual(responseData.Data._v.Count, expectedDataCount_WallArt);
                }
                else if (category.ProductFor == "All" & category.ProductType == "Gifts")
                {
                    Assert.AreEqual(responseData.Data._v.Count, expectedDataCount_Gifts);
                }
            }
        }
    }

    [TestClass]
    public class CouponController_IntegrationTest
    {
        public CouponController controller = new CouponController();

        [TestMethod]
        public void CouponController_InsertCoupon_IntegrationTest_ArthurClive()
        {
            //Arrange
            Coupon coupon = new Coupon
            {
                Code = "SampleCODE1",
                ApplicableFor = "All",
                ExpiryTime = DateTime.UtcNow.AddMonths(1),
                UsageCount = 10,
                Value = 10,
                Percentage = true,
            };
            var expectedCode = "200";
            var expectedMessage = "Coupon inserted successfully";

            //Act
            var result = controller.InsertCoupon(coupon) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<Coupon>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("Code", coupon.Code), "CouponDB", "Coupon").Result);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.Code, coupon.Code);
            Assert.AreEqual(insertedData.ApplicableFor, coupon.ApplicableFor);
            Assert.AreEqual(insertedData.UsageCount, coupon.UsageCount);
            Assert.AreEqual(insertedData.Value, coupon.Value);
            Assert.AreEqual(insertedData.Percentage, coupon.Percentage);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("Code", coupon.Code, null, null, "CouponDB", "Coupon");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("Code", coupon.Code), "CouponDB", "Coupon");
            }
        }

        [TestMethod]
        public void CouponController_CheckCoupon_IntegrationTest_ArthurClive()
        {
            //Arrange
            Coupon coupon1 = new Coupon
            {
                Code = "SampleCODE1",
                ApplicableFor = "All",
                ExpiryTime = DateTime.UtcNow.AddMonths(1),
                UsageCount = 10,
                Value = 10,
                Percentage = true,
            };
            Coupon coupon2 = new Coupon
            {
                Code = "SampleCODE2",
                ApplicableFor = "SampleUser",
                ExpiryTime = DateTime.UtcNow.AddMonths(1),
                UsageCount = 1,
                Value = 10,
                Percentage = true,
            };
            var userName = "SampleUser";
            var expectedCode = "200";
            var expectedMessage = "Coupon is valid";

            //Insert data to db for testing
            var insertCoupon1 = TH.DeserializedResponceData(controller.InsertCoupon(coupon1).Result.ToJson());
            var insertCoupon2 = TH.DeserializedResponceData(controller.InsertCoupon(coupon2).Result.ToJson());

            //Act
            var result1 = controller.CheckCoupon(userName, coupon1.Code) as ActionResult;
            var responseData1 = TH.DeserializedResponceData(result1.ToJson());
            var result2 = controller.CheckCoupon(userName, coupon2.Code) as ActionResult;
            var responseData2 = TH.DeserializedResponceData(result2.ToJson());

            //Assert
            Assert.IsNotNull(insertCoupon1);
            Assert.IsNotNull(insertCoupon2);
            Assert.AreEqual(insertCoupon1.Code, expectedCode);
            Assert.AreEqual(insertCoupon2.Code, expectedCode);
            Assert.AreEqual(insertCoupon1.Message, "Coupon inserted successfully");
            Assert.AreEqual(insertCoupon2.Message, "Coupon inserted successfully");
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.AreEqual(responseData1.Code, expectedCode);
            Assert.AreEqual(responseData2.Code, expectedCode);
            Assert.AreEqual(responseData1.Message, expectedMessage);
            Assert.AreEqual(responseData2.Message, expectedMessage);

            //Delete inserted test data
            var checkData1 = MH.CheckForDatas("Code", coupon1.Code, null, null, "CouponDB", "Coupon");
            if (checkData1 != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("Code", coupon1.Code), "CouponDB", "Coupon");
            }
            var checkData2 = MH.CheckForDatas("Code", coupon2.Code, null, null, "CouponDB", "Coupon");
            if (checkData2 != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("Code", coupon2.Code), "CouponDB", "Coupon");
            }
        }

        [TestMethod]
        public void CouponController_UpdateCoupon_IntegrationTest_ArthurClive()
        {
            //Arrange
            Coupon coupon = new Coupon
            {
                Code = "SampleCODE1",
                ApplicableFor = "All",
                ExpiryTime = DateTime.UtcNow.AddMonths(1),
                UsageCount = 10,
                Value = 10,
                Percentage = true,
            };
            UpdateCoupon updateCoupon = new UpdateCoupon
            {
                ApplicableFor = "SampleUser",
                ExpiryTime = DateTime.UtcNow.AddMonths(1),
                UsageCount = 9,
                Value = 10,
                Percentage = true,
            };
            var expectedCode = "200";
            var expectedMessage = "Coupon updated successfully";
            var expectedUsageCount = 1;

            //Insert data to db for testing
            var insertCoupon = TH.DeserializedResponceData(controller.InsertCoupon(coupon).Result.ToJson());

            //Act
            var result = controller.UpdateCoupon(updateCoupon, coupon.Code) as ActionResult;
            var responseData = TH.DeserializedResponceData(result.ToJson());

            //Check inserted data
            var updatedData = BsonSerializer.Deserialize<Coupon>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("Code", coupon.Code), "CouponDB", "Coupon").Result);

            //Assert
            Assert.IsNotNull(insertCoupon);
            Assert.AreEqual(insertCoupon.Code, expectedCode);
            Assert.AreEqual(insertCoupon.Message, "Coupon inserted successfully");
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(updatedData.Code, coupon.Code);
            Assert.AreEqual(updatedData.ApplicableFor, updateCoupon.ApplicableFor);
            Assert.AreEqual(updatedData.UsageCount, expectedUsageCount);
            Assert.AreEqual(updatedData.Value, updateCoupon.Value);
            Assert.AreEqual(updatedData.Percentage, updateCoupon.Percentage);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("Code", coupon.Code, null, null, "CouponDB", "Coupon");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("Code", coupon.Code), "CouponDB", "Coupon");
            }
        }
    }

    [TestClass]
    public class OrderController_IntegrationTest
    {
        public OrderController controller = new OrderController();
        public UserController userController = new UserController();
        public ProductController productController = new ProductController();

        [TestMethod]
        public void OrderController_InsertCoupon_IntegrationTest_ArthurClive()
        {
            //Arrange
            var username = "SampleUser";
            List<Arthur_Clive.Data.Address> addressList = new List<Arthur_Clive.Data.Address>();
            Arthur_Clive.Data.Address address = new Arthur_Clive.Data.Address
            {
                UserName = username,
                Name = "SampleName",
                PhoneNumber = "12341234",
                AddressLines = "SampleAddressLines",
                PostOffice = "SamplePostOffice",
                City = "CBE",
                State = "TN",
                Country = "India",
                PinCode = "641035",
                Landmark = "SampleLandMark",
                BillingAddress = true,
                ShippingAddress = true,
                DefaultAddress = true
            };
            addressList.Add(address);
            Arthur_Clive.Data.AddressList ListOfAddress = new Arthur_Clive.Data.AddressList();
            ListOfAddress.ListOfAddress = addressList;
            Arthur_Clive.Data.Product product = new Arthur_Clive.Data.Product
            {
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                ProductDesign = "SampleDesign",
                ProductBrand = "Arthur Clive",
                ProductPrice = 100,
                ProductDiscount = 10,
                ProductStock = 1,
                ProductSize = "S",
                ProductColour = "Black",
                RefundApplicable = true,
                ReplacementApplicable = true,
                ProductDescription = "SampleDescription",
                ProductMaterial = "Cotton",
            };
            List<Arthur_Clive.Data.Cart> cartList = new List<Arthur_Clive.Data.Cart>();
            Arthur_Clive.Data.Cart cart = new Arthur_Clive.Data.Cart
            {
                UserName = username,
                ProductSKU = "SampleFor-SampleType-SampleDesign-Black-S",
                MinioObject_URL = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/SampleFor-SampleType-SampleDesign-Black-S.jpg",
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                ProductDesign = "SampleDesign",
                ProductBrand = "Arthur Clive",
                ProductPrice = 100,
                ProductDiscount = 10,
                ProductDiscountPrice = 90,
                ProductQuantity = 1,
                ProductSize = "S",
                ProductColour = "Black",
                ProductDescription = "SampleDescription"
            };
            cartList.Add(cart);
            Arthur_Clive.Data.CartList ListOfCart = new Arthur_Clive.Data.CartList();
            ListOfCart.ListOfProducts = cartList;
            Arthur_Clive.Data.OrderInfo orderInfo = new Arthur_Clive.Data.OrderInfo { CouponDiscount = 0 , TotalAmount = 95 , EstimatedTax = 5};
            Arthur_Clive.Data.RegisterModel registerModel = new Arthur_Clive.Data.RegisterModel
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
                Status = "Verified"
            };
            var expectedCode = "200";
            var expectedMessage = "Order Placed";

            //Insert data to db for testing
            var insertAddress = TH.DeserializedResponceData(userController.RefreshUserInfo(ListOfAddress, username).Result.ToJson());
            var insertProduct = TH.DeserializedResponceData(productController.Post(product).Result.ToJson());
            var insertCart = TH.DeserializedResponceData(userController.RefreshCart(ListOfCart, username).Result.ToJson());
            TH.InsertRegiterModeldata(registerModel);

            //Act
            var result = controller.PlaceOrder(orderInfo,username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<Arthur_Clive.Data.OrderInfo>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName",username), "OrderDB", "OrderInfo").Result);

            //Assert
            Assert.IsNotNull(insertAddress);
            Assert.IsNotNull(insertProduct);
            Assert.IsNotNull(insertCart);
            Assert.AreEqual(insertAddress.Code, expectedCode);
            Assert.AreEqual(insertProduct.Code, expectedCode);
            Assert.AreEqual(insertCart.Code, expectedCode);
            Assert.AreEqual(insertAddress.Message, "Inserted");
            Assert.AreEqual(insertProduct.Message, "Inserted");
            Assert.AreEqual(insertCart.Message, "Inserted");
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code,expectedCode);
            Assert.AreEqual(responseData.Message,expectedMessage);
            Assert.IsNotNull(responseData.Data);
            Assert.IsNotNull(insertedData);
            Assert.AreEqual(insertedData.UserName,username);
            Assert.AreEqual(insertedData.TotalAmount,orderInfo.TotalAmount);
            Assert.AreEqual(insertedData.EstimatedTax, orderInfo.EstimatedTax);
            Assert.AreEqual(insertedData.PaymentMethod, orderInfo.PaymentMethod);
            Assert.AreEqual(insertedData.PaymentDetails.Status.Count, 1);
            Assert.AreEqual(insertedData.PaymentDetails.Status[0].StatusId, 1);
            Assert.AreEqual(insertedData.PaymentDetails.Status[0].Description, "Payment Initiated");
            Assert.AreEqual(insertedData.CouponDiscount, orderInfo.CouponDiscount);
            Assert.AreEqual(insertedData.Address.Count, 1);
            Assert.AreEqual(insertedData.Address[0].UserName,address.UserName);
            Assert.AreEqual(insertedData.Address[0].Name, address.Name);
            Assert.AreEqual(insertedData.Address[0].PhoneNumber, address.PhoneNumber);
            Assert.AreEqual(insertedData.Address[0].AddressLines, address.AddressLines);
            Assert.AreEqual(insertedData.Address[0].PostOffice, address.PostOffice);
            Assert.AreEqual(insertedData.Address[0].City, address.City);
            Assert.AreEqual(insertedData.Address[0].State, address.State);
            Assert.AreEqual(insertedData.Address[0].Country, address.Country);
            Assert.AreEqual(insertedData.Address[0].PinCode, address.PinCode);
            Assert.AreEqual(insertedData.Address[0].Landmark, address.Landmark);
            Assert.AreEqual(insertedData.Address[0].BillingAddress, address.BillingAddress);
            Assert.AreEqual(insertedData.Address[0].ShippingAddress, address.ShippingAddress);
            Assert.AreEqual(insertedData.Address[0].DefaultAddress, address.DefaultAddress);
            Assert.AreEqual(insertedData.ProductDetails.Count, 1);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductSKU,cart.ProductSKU);
            Assert.AreEqual(insertedData.ProductDetails[0].Status, "Order Placed");
            Assert.AreEqual(insertedData.ProductDetails[0].StatusCode.Count, 1);
            Assert.AreEqual(insertedData.ProductDetails[0].StatusCode[0].StatusId, 1);
            Assert.AreEqual(insertedData.ProductDetails[0].StatusCode[0].Description, "Order Placed");
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.UserName, username);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductSKU, cart.ProductSKU);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.MinioObject_URL, cart.MinioObject_URL);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductFor, cart.ProductFor);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductType, cart.ProductType);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductDesign, cart.ProductDesign);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductBrand, cart.ProductBrand);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductPrice, cart.ProductPrice);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductDiscount, cart.ProductDiscount);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductDiscountPrice, cart.ProductDiscountPrice);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductQuantity, cart.ProductQuantity);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductSize, cart.ProductSize);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductColour, cart.ProductColour);
            Assert.AreEqual(insertedData.ProductDetails[0].ProductInCart.ProductDescription, cart.ProductDescription);

            //Delete inserted test data
            var checkData1 = MH.CheckForDatas("UserName",username , null, null, "UserInfo", "UserInfo");
            if (checkData1 != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "UserInfo", "UserInfo");
            }
            var checkData2 = MH.CheckForDatas("ProductSKU", cart.ProductSKU, null, null, "ProductDB", "Product");
            if (checkData2 != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("ProductSKU", cart.ProductSKU), "ProductDB", "Product");
            }
            var checkData3 = MH.CheckForDatas("UserName", username, null, null, "UserInfo", "Cart");
            if (checkData3 != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "UserInfo", "Cart");
            }
            var checkData4 = MH.CheckForDatas("UserName", username, null, null, "OrderDB", "OrderInfo");
            if (checkData4!= null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "OrderDB", "OrderInfo");
            }
            var checkData5 = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
            if (checkData5 != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication");
            }
        }

        //[TestMethod]
        public void OrderController_GetOrdersOfUser_IntegrationTest_ArthurClive()
        {
            //Arrange
            var expectedCode = "200";
            var expectedMessage = "Success";
            var expectedDataCount = 6;

            //Act
            var result = controller.GetOrdersOfUser("") as ActionResult;
            var responseData = TH.DeserializedResponceData_CategoryController_Get(result.ToJson());

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.IsNotNull(responseData.Data);
            Assert.AreEqual(responseData.Data._v.Count, expectedDataCount);
        }

    }

    [TestClass]
    public class UserController_IntegrationTest
    {
        public UserController controller = new UserController();

        [TestMethod]
        public void UserController_RefreshUserInfo_IntegrationTest_ArthurClive()
        {
            //Arrange
            var username = "SampleUser";
            List<Address> addressList = new List<Address>();
            Address address = new Address
            {
                UserName = username,
                Name = "SampleName",
                PhoneNumber = "12341234",
                AddressLines = "SampleAddressLines",
                PostOffice = "SamplePostOffice",
                City = "CBE",
                State = "TN",
                Country = "India",
                PinCode = "641035",
                Landmark = "SampleLandMark",
                BillingAddress = true,
                ShippingAddress = true,
                DefaultAddress = true
            };
            addressList.Add(address);
            AddressList ListOfAddress = new AddressList();
            ListOfAddress.ListOfAddress = addressList;
            var expectedCode = "200";
            var expectedMessage = "Inserted";

            //Act
            var result = controller.RefreshUserInfo(ListOfAddress, username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<Address>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "UserInfo", "UserInfo").Result);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code,expectedCode);
            Assert.AreEqual(responseData.Message,expectedMessage);
            Assert.AreEqual(insertedData.UserName,username);
            Assert.AreEqual(insertedData.Name, address.Name);
            Assert.AreEqual(insertedData.PhoneNumber, address.PhoneNumber);
            Assert.AreEqual(insertedData.AddressLines, address.AddressLines);
            Assert.AreEqual(insertedData.PostOffice, address.PostOffice);
            Assert.AreEqual(insertedData.City, address.City);
            Assert.AreEqual(insertedData.State, address.State);
            Assert.AreEqual(insertedData.Country, address.Country);
            Assert.AreEqual(insertedData.PinCode, address.PinCode);
            Assert.AreEqual(insertedData.Landmark, address.Landmark);
            Assert.AreEqual(insertedData.BillingAddress, address.BillingAddress);
            Assert.AreEqual(insertedData.ShippingAddress, address.ShippingAddress);
            Assert.AreEqual(insertedData.DefaultAddress, address.DefaultAddress);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "UserInfo", "UserInfo");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "UserInfo", "UserInfo");
            }
        }

        [TestMethod]
        public void UserController_RefreshCart_IntegrationTest_ArthurClive()
        {
            //Arrange
            var username = "SampleUser";
            List<Cart> cartList = new List<Cart>();
            Cart cart = new Cart
            {
                UserName = username,
                ProductSKU = "SampleFor-SampleType-SampleDesign-Black-S",
                MinioObject_URL = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/SampleFor-SampleType-SampleDesign-Black-S.jpg",
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                ProductDesign = "SampleDesign",
                ProductBrand = "Arthur Clive",
                ProductPrice = 100,
                ProductDiscount = 10,
                ProductDiscountPrice = 90,
                ProductQuantity = 1,
                ProductSize = "S",
                ProductColour = "Black",
                ProductDescription = "SampleDescription"
            };
            cartList.Add(cart);
            CartList ListOfCart = new CartList();
            ListOfCart.ListOfProducts = cartList;
            var expectedCode = "200";
            var expectedMessage = "Inserted";

            //Act
            var result = controller.RefreshCart(ListOfCart, username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<Cart>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "UserInfo", "Cart").Result);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.UserName, username);
            Assert.AreEqual(insertedData.ProductSKU, cart.ProductSKU);
            Assert.AreEqual(insertedData.MinioObject_URL, cart.MinioObject_URL);
            Assert.AreEqual(insertedData.ProductFor, cart.ProductFor);
            Assert.AreEqual(insertedData.ProductType, cart.ProductType);
            Assert.AreEqual(insertedData.ProductDesign, cart.ProductDesign);
            Assert.AreEqual(insertedData.ProductBrand, cart.ProductBrand);
            Assert.AreEqual(insertedData.ProductPrice, cart.ProductPrice);
            Assert.AreEqual(insertedData.ProductDiscount, cart.ProductDiscount);
            Assert.AreEqual(insertedData.ProductDiscountPrice, cart.ProductDiscountPrice);
            Assert.AreEqual(insertedData.ProductQuantity, cart.ProductQuantity);
            Assert.AreEqual(insertedData.ProductSize, cart.ProductSize);
            Assert.AreEqual(insertedData.ProductColour, cart.ProductColour);
            Assert.AreEqual(insertedData.ProductDescription, cart.ProductDescription);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "UserInfo", "Cart");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "UserInfo", "Cart");
            }
        }

        [TestMethod]
        public void UserController_RefreshWishList_IntegrationTest_ArthurClive()
        {
            //Arrange
            var username = "SampleUser";
            List<WishList> wishlistList = new List<WishList>();
            WishList wishlist = new WishList
            {
                UserName = username,
                ProductSKU = "SampleFor-SampleType-SampleDesign-Black-S",
                MinioObject_URL = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/SampleFor-SampleType-SampleDesign-Black-S.jpg",
                ProductFor = "SampleFor",
                ProductType = "SampleType",
                ProductDesign = "SampleDesign",
                ProductBrand = "Arthur Clive",
                ProductPrice = 100,
                ProductDiscount = 10,
                ProductDiscountPrice = 90,
                ProductSize = "S",
                ProductColour = "Black",
                ProductDescription = "SampleDescription"
            };
            wishlistList.Add(wishlist);
            WishlistList ListOfWishlist = new WishlistList();
            ListOfWishlist.ListOfProducts = wishlistList;
            var expectedCode = "200";
            var expectedMessage = "Inserted";

            //Act
            var result = controller.RefreshWishList(ListOfWishlist, username) as Task<ActionResult>;
            var responseData = TH.DeserializedResponceData(result.Result.ToJson());

            //Check inserted data
            var insertedData = BsonSerializer.Deserialize<WishList>(MH.GetSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "UserInfo", "WishList").Result);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(responseData.Code, expectedCode);
            Assert.AreEqual(responseData.Message, expectedMessage);
            Assert.AreEqual(insertedData.UserName, username);
            Assert.AreEqual(insertedData.ProductSKU, wishlist.ProductSKU);
            Assert.AreEqual(insertedData.MinioObject_URL, wishlist.MinioObject_URL);
            Assert.AreEqual(insertedData.ProductFor, wishlist.ProductFor);
            Assert.AreEqual(insertedData.ProductType, wishlist.ProductType);
            Assert.AreEqual(insertedData.ProductDesign, wishlist.ProductDesign);
            Assert.AreEqual(insertedData.ProductBrand, wishlist.ProductBrand);
            Assert.AreEqual(insertedData.ProductPrice, wishlist.ProductPrice);
            Assert.AreEqual(insertedData.ProductDiscount, wishlist.ProductDiscount);
            Assert.AreEqual(insertedData.ProductDiscountPrice, wishlist.ProductDiscountPrice);
            Assert.AreEqual(insertedData.ProductSize, wishlist.ProductSize);
            Assert.AreEqual(insertedData.ProductColour, wishlist.ProductColour);
            Assert.AreEqual(insertedData.ProductDescription, wishlist.ProductDescription);

            //Delete inserted test data
            var checkData = MH.CheckForDatas("UserName", username, null, null, "UserInfo", "WishList");
            if (checkData != null)
            {
                var delete = MH.DeleteSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "UserInfo", "WishList");
            }
        }
    }
}

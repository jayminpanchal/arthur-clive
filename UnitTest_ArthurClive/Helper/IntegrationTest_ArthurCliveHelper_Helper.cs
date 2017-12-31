using Arthur_Clive.Data;
using Arthur_Clive.Helper;
using MongoDB.Driver;

namespace UnitTest_ArthurClive.Helper
{
    class IntegrationTest_ArthurCliveHelper_Helper
    {
        public static IMongoDatabase db = MongoHelper._client.GetDatabase("UnitTestDB");

        public async static void InsertData_SampleCategory()
        {
            var insertData = new Category { MinioObject_URL = "https://s3.ap-south-1.amazonaws.com/product-category/Men-Tshirt.jpg", ProductFor = "Men", ProductType = "Tshirt" };
            var collection = db.GetCollection<Category>("Category");
            await collection.InsertOneAsync(insertData);
        }

        public async static void InsertData_SampleOrder()
        {
            var insertData = new OrderInfo { OrderId = 1, UserName = "sample@gmail.com", TotalAmount = 1000, EstimatedTax = 10 };
            var collection = db.GetCollection<OrderInfo>("OrderInfo");
            await collection.InsertOneAsync(insertData);
        }

        public async static void InsertData_SampleProduct()
        {
            var insertData = new Product { ProductFor = "All", ProductType = "Art", ProductSKU = "All-Art-Bangalore-Black-",ProductDesign= "Bangalore",ProductBrand= "Arthur Clive",ProductPrice= 395.0,ProductDiscount= 0.0,ProductStock= 10,ProductColour= "Black",ReplacementApplicable= true,ProductDescription= "Cosmopolitan Bengaluru (formerly Bangalore) is envisaged perfectly by our team of designers and we are confident that it will be a perfect match for any room.",RefundApplicable=true,MinioObject_URL= "https://s3.ap-south-1.amazonaws.com/arthurclive-products/All-Art-Bangalore-Black-.jpg",ProductDiscountPrice= 395.0,ProductMaterial= "Paper" };
            var collection = db.GetCollection<Product>("Product");
            await collection.InsertOneAsync(insertData);
        }
    }
}

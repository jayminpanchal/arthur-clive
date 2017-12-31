using System;
using System.Collections.Generic;
using Arthur_Clive.Data;
using Arthur_Clive.Helper;
using Swashbuckle.AspNetCore.Examples;

namespace Arthur_Clive.Swagger
{
    #region ProductController
    /// <summary></summary>
    public class InsertProductDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                ProductFor = "Girls",
                ProductType = "Tshirt",
                ProductDesign = "LionFace",
                ProductBrand = "Arthur Clive",
                ProductPrice = 395.0,
                ProductDiscount = 0.0,
                ProductStock = 10,
                ProductSize = "9_10Y",
                ProductColour = "Black",
                ReplacementApplicable = true,
                ProductDescription = "“Om” signifies divine knowledge, eternal peace and spirituality. An absolute fashion icon on its own, the print makes it a versatile wear. Team up with trendy jackets and denims, certainly brings about a smart street-style sensibility.",
                RefundApplicable = true,
                ProductMaterial = "Cotton"
            };
        }
    }

    /// <summary></summary>
    public class UpdateProductDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                ProductFor = "Men",
                ProductType = "Shirt",
                ProductDesign = "Om",
                ProductBrand = "Arthur Clive",
                ProductPrice = 695.0,
                ProductDiscount = 10.0,
                ProductStock = 15,
                ProductSize = "11_12Y",
                ProductColour = "White",
                ReplacementApplicable = false,
                ProductDescription = "Updated description",
                RefundApplicable = false,
                ProductMaterial = "Flax"
            };
        }
    }

    /// <summary></summary>
    public class InsertReviewDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                Name = "Sample1",
                Comment = "Good",
                Rating = 4,
                OrderId = 1
            };
        }
    }

    /// <summary></summary>
    public class UpdateReviewDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                Id = 1,
                Approved = true
            };
        }
    }

    #endregion

    #region CategoryController

    /// <summary></summary>
    public class InsertCategoryDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                ProductFor = "Women",
                ProductType = "Tops",
                Description = "Tops for women"
            };
        }
    }

    /// <summary></summary>
    public class UpdateCategoryDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                ProductFor = "Men",
                ProductType = "Shirt",
                Description = "Shirts for men"
            };
        }
    }

    #endregion

    #region CouponController

    /// <summary></summary>
    public class CouponData : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                Code = "CU111111",
                ApplicableFor = "All",
                ExpiryTime = DateTime.UtcNow.AddMonths(1),
                UsageCount = 10,
                Value = 10,
                Percentage = true,
            };
        }
    }

    /// <summary></summary>
    public class CouponUpdateData : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new UpdateCoupon
            {
                ApplicableFor = "All",
                ExpiryTime = DateTime.UtcNow,
                Value = 100,
                Percentage = false,
                UsageCount = 1
            };
        }
    }

    /// <summary></summary>
    public class UseCouponData : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new UseCoupon
            {
                UsageCount = 1,
                Amount = 100
            };
        }
    }

    #endregion

    #region UserInfoController

    /// <summary></summary>
    public class AddressDetail : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                ListOfAddress = new List<Address>
                {
                    new Address
                    {
                        UserName = "12341234",
                        Name = "Sample",
                        DialCode = "+91",
                        PhoneNumber = "12341234",
                        AddressLines = "GSK street",
                        PostOffice = "Saravanampati",
                        City = "Coimbatore",
                        State = "TamilNadu",
                        Country = "India",
                        PinCode = "641035",
                        Landmark = "Near KGISL",
                        BillingAddress = true,
                        ShippingAddress = false,
                        DefaultAddress = true
                    },
                    new Address
                    {
                        UserName = "12341234",
                        Name = "Sample",
                        DialCode = "+91",
                        PhoneNumber = "12341234",
                        AddressLines = "GSK street",
                        PostOffice = "Saravanampati",
                        City = "Coimbatore",
                        State = "TamilNadu",
                        Country = "India",
                        PinCode = "641035",
                        Landmark = "Near KGISL",
                        BillingAddress = false,
                        ShippingAddress = true,
                        DefaultAddress = true
                    }
                }
            };
        }
    }

    /// <summary></summary>
    public class CartDetail : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                ListOfProducts = new List<Cart>
                {
                    new Cart
                    {
                        UserName="sample@gamil.com",
                        ProductSKU="Men-Tshirt-Om-Black-S",
                        MinioObject_URL="https://s3.ap-south-1.amazonaws.com/arthurclive-products/Men-Tshirt-Om-Black-S.jpg",
                        ProductFor="Men",
                        ProductType="Tshirt",
                        ProductDesign = "Om",
                        ProductBrand = "Arthur Clive",
                        ProductPrice = 695,
                        ProductDiscount = 0,
                        ProductDiscountPrice = 695,
                        ProductQuantity = 1,
                        ProductSize = "S",
                        ProductColour = "Black",
                        ProductDescription = "Tshirt for men"
                    },
                    new Cart
                    {
                        UserName="sample@gamil.com",
                        ProductSKU="All-Gifts-BirthDay--",
                        MinioObject_URL="https://s3.ap-south-1.amazonaws.com/arthurclive-products/All-Gifts-BirthDay--.jpg",
                        ProductFor="recipient@gmail.com",
                        ProductType="Gifts",
                        ProductDesign = "BirthDay",
                        ProductBrand = "Arthur Clive",
                        ProductPrice = 100,
                        ProductQuantity = 1,
                        ProductDescription = "Gift for birthday"
                    }
                }
            };
        }
    }

    /// <summary></summary>
    public class WishlistDetail : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                ListOfProducts = new List<WishList>
                {
                    new WishList
                    {
                        UserName = "sample@gamil.com",
                        ProductSKU = "Men-Tshirt-Om-Black-S",
                        MinioObject_URL = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/Men-Tshirt-Om-Black-S.jpg",
                        ProductFor = "Men",
                        ProductType = "Tshirt",
                        ProductDesign = "Om",
                        ProductBrand = "Arthur Clive",
                        ProductPrice = 695,
                        ProductDiscount = 0,
                        ProductDiscountPrice = 695,
                        ProductSize = "S",
                        ProductColour = "Black",
                        ProductDescription = "Tshirt for men"
                    },
                    new WishList
                    {
                        UserName = "sample@gamil.com",
                        ProductSKU = "Men-Tshirt-Om-Black-L",
                        MinioObject_URL = "https://s3.ap-south-1.amazonaws.com/arthurclive-products/Men-Tshirt-Om-Black-L.jpg",
                        ProductFor = "Men",
                        ProductType = "Tshirt",
                        ProductDesign = "Om",
                        ProductBrand = "Arthur Clive",
                        ProductPrice = 695,
                        ProductDiscount = 0,
                        ProductDiscountPrice = 695,
                        ProductSize = "L",
                        ProductColour = "Black",
                        ProductDescription = "Tshirt for men"
                    }
                }
            };
        }
    }

    #endregion

    #region PaymentController

    /// <summary></summary>
    public class PaymentDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                FirstName = "Sample",
                LastName = "User",
                ProductInfo = "Men-Tshirt-Om-Black-S",
                Amount = "695",
                Email = "sample@gmail.com",
                PhoneNumber = "12341234",
                AddressLine1 = "No.01",
                AddressLine2 = "GRS street",
                City = "Coimbatore",
                State = "TamilNadu",
                Country = "India",
                ZipCode = "641035"
            };
        }

    }

    #endregion

    #region OrderController

    /// <summary></summary>
    public class OrderDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                CouponDiscount = 0,
                TotalAmount = 700,
                EstimatedTax = 5
            };
        }
    }

    /// <summary></summary>
    public class OrderRequestDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                OrderId = 1,
            };
        }
    }

    /// <summary></summary>
    public class StatusUpdateDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                OrderId = 1,
                Status = "Delivered",
            };
        }
    }

    #endregion

    #region AdminController

    /// <summary></summary>
    public class ImageUploadDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                LocalPath = "D'\\ac\\EmailTemplate\\Arthur Clive_files",
                BucketName = "product-category",
                ObjectName = "sampleobject"
            };
        }
    }

    /// <summary></summary>
    public class RoleDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                RoleID = 4,
                RoleName = "SampleRole",
                LevelOfAccess = new List<string> { "Level1Access", "Level2Access", "Level3Access" }
            };
        }
    }

    #endregion
}

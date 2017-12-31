using System;
using System.Collections.Generic;
using System.Text;
using Arthur_Clive.Data;
using MongoDB.Bson;

namespace UnitTest_ArthurClive.Controller
{
    public class ActionResultModel
    {
        public dynamic _t { get; set; }
        public ResponceData Value { get; set; }
        public dynamic Formatters { get; set; }
        public dynamic ContentTypes { get; set; }
        public dynamic DeclaredType { get; set; }
        public int StatusCode { get; set; }
    }

    public class ResponceData
    {
        public dynamic _t { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
        public dynamic Content { get; set; }
    }

    public class ActionResultModel_CategoryController_Get
    {
        public dynamic _t { get; set; }
        public ResponceData_CategoryController_Get Value { get; set; }
        public dynamic Formatters { get; set; }
        public dynamic ContentTypes { get; set; }
        public dynamic DeclaredType { get; set; }
        public int StatusCode { get; set; }
    }

    public class ResponceData_CategoryController_Get
    {
        public dynamic _t { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public CategoryData Data { get; set; }
        public dynamic Content { get; set; }
    }

    public class CategoryData
    {
        public dynamic _t { get; set; }
        public List<Category> _v { get; set; }
    }

    public class Category
    {
        public dynamic _t { get; set; }
        public string ProductFor { get; set; }
        public string ProductType { get; set; }
        public string MinioObject_URL { get; set; }
        public string Description { get; set; }
    }
    public class ActionResultModel_ProductController_Get
    {
        public dynamic _t { get; set; }
        public ResponceData_ProductController_Get Value { get; set; }
        public dynamic Formatters { get; set; }
        public dynamic ContentTypes { get; set; }
        public dynamic DeclaredType { get; set; }
        public int StatusCode { get; set; }
    }

    public class ResponceData_ProductController_Get
    {
        public dynamic _t { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public ProductData Data { get; set; }
        public dynamic Content { get; set; }
    }

    public class ProductData
    {
        public dynamic _t { get; set; }
        public List<Product> _v { get; set; }
    }

    public class Product
    {
        public dynamic _t { get; set; }
        public string ProductSKU { get; set; }
        public string MinioObject_URL { get; set; }
        public string ProductFor { get; set; }
        public string ProductType { get; set; }
        public string ProductDesign { get; set; }
        public string ProductBrand { get; set; }
        public double ProductPrice { get; set; }
        public double ProductDiscount { get; set; }
        public double ProductDiscountPrice { get; set; }
        public long ProductStock { get; set; }
        public string ProductSize { get; set; }
        public string ProductMaterial { get; set; }
        public double ProductRating { get; set; }
        public Review[] ProductReviews { get; set; }
        public string ProductColour { get; set; }
        public bool? RefundApplicable { get; set; }
        public bool? ReplacementApplicable { get; set; }
        public string ProductDescription { get; set; }
    }
}


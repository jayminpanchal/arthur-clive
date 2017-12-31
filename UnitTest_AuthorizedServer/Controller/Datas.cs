
using System;
using AuthorizedServer.Models;
using MongoDB.Bson;

namespace UnitTest_AuthorizedServer.Controller
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

    public class ActionResultModel_AuthController_GetUserInfo
    {
        public dynamic _t { get; set; }
        public ResponceData_AuthController_GetUserInfo Value { get; set; }
        public dynamic Formatters { get; set; }
        public dynamic ContentTypes { get; set; }
        public dynamic DeclaredType { get; set; }
        public int StatusCode { get; set; }
    }

    public class ResponceData_AuthController_GetUserInfo
    {
        public dynamic _t { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public Data_AuthController_GetUserInfo Data { get; set; }
        public dynamic Content { get; set; }
    }
    public class Data_AuthController_GetUserInfo
    {
        public string _t { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

}

﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthorizedServer.Models;
using MongoDB.Driver;
using MH = AuthorizedServer.Helper.MongoHelper;
using Microsoft.AspNetCore.Identity;
using AuthorizedServer.Helper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using AuthorizedServer.Repositories;
using Microsoft.Extensions.Options;
using AuthorizedServer.Logger;
using System.Net.Http;
using Swashbuckle.AspNetCore.Examples;
using AuthorizedServer.Swagger;

namespace AuthorizedServer.Controllers
{
    /// <summary>Contoller to Authorize user and to perform other task related to user account</summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        /// <summary></summary>
        public AuthHelper authHelper = new AuthHelper();
        /// <summary></summary>
        private IOptions<Audience> _settings;
        /// <summary></summary>
        private IRTokenRepository _repo;
        /// <summary></summary>
        public IMongoDatabase _db = MH._client.GetDatabase("Authentication");
        /// <summary></summary>
        public PasswordHasher<VerificationModel> smsHasher = new PasswordHasher<VerificationModel>();
        /// <summary></summary>
        public PasswordHasher<RegisterModel> passwordHasher = new PasswordHasher<RegisterModel>();

        /// <summary></summary>
        /// <param name="settings"></param>
        /// <param name="repo"></param>
        public AuthController(IOptions<Audience> settings, IRTokenRepository repo)
        {
            this._settings = settings;
            this._repo = repo;
        }

        /// <summary>Register new user</summary>
        /// <remarks>This api is used to register new user</remarks>
        /// <param name="data"></param>
        /// <response code="200">User successfully registered</response>
        /// <response code="401">User already registered</response>   
        /// <response code="402">Verification type empty</response>   
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("register")]
        [SwaggerRequestExample(typeof(RegisterModel), typeof(RegisterDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> Register([FromBody]RegisterModel data)
        {
            try
            {
                BsonDocument checkUser;
                string userName;
                string OTP;
                if (data.UserLocation == "IN")
                {
                    userName = data.PhoneNumber;
                }
                else
                {
                    userName = data.Email;
                }
                checkUser = MH.CheckForDatas("UserName", userName, null, null, "Authentication", "Authentication");
                if (checkUser == null)
                {
                    if (data.UserLocation != null)
                    {
                        data.UserName = userName;
                        data.UserRole = "User";
                        RegisterModel registerModel = new RegisterModel { UserName = userName, Password = data.Password };
                        data.Password = passwordHasher.HashPassword(registerModel, data.Password);
                        data.OTPExp = DateTime.UtcNow.AddMinutes(2);
                        VerificationModel smsModel = new VerificationModel();
                        smsModel.UserName = userName;
                        if (data.UserLocation == "IN")
                        {
                            Random codeGenerator = new Random();
                            OTP = codeGenerator.Next(0, 1000000).ToString("D6");
                            smsModel.VerificationCode = OTP;
                            SMSHelper.SendSMS(data.PhoneNumber, OTP);
                        }
                        else
                        {
                            OTP = Guid.NewGuid().ToString();
                            string link = GlobalHelper.GetIpConfig() + data.UserName + "/" + OTP + "/no";
                            var result = await EmailHelper.SendEmail(data.FullName, data.Email, link);
                        }
                        data.VerificationCode = smsHasher.HashPassword(smsModel, OTP);
                        data.Status = "Registered";
                        var authCollection = _db.GetCollection<RegisterModel>("Authentication");
                        await authCollection.InsertOneAsync(data);
                        return Ok(new ResponseData
                        {
                            Code = "200",
                            Message = "User Registered",
                            Data = null
                        });
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "Verification Type Cannot Be Empty",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "401",
                        Message = "User Already Registered",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "Register", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Verify user using SMS or Email</summary>
        /// <remarks>This api is used to verify registered</remarks>
        /// <param name="username">UserName of user who needs to get verified</param>
        /// <param name="otp">OTP sent to user to verify amout</param>
        /// <response code="999">User is verified and jwt token is returned</response>
        /// <response code="909">User is verified but some error occured while getting jwt token </response>   
        /// <response code="401">OTP expired </response>     
        /// <response code="402">OTP invalid </response>   
        /// <response code="404">User not found </response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpGet("register/verification/{username}/{otp}")]
        [ProducesResponseType(typeof(JsonResult), 999)]
        [ProducesResponseType(typeof(JsonResult), 909)]
        public ActionResult RegisterVerification(string username, string otp)
        {
            try
            {
                var checkUser = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
                if (checkUser != null)
                {
                    var verifyUser = BsonSerializer.Deserialize<RegisterModel>(checkUser);
                    if (verifyUser.OTPExp > DateTime.UtcNow)
                    {
                        VerificationModel smsModel = new VerificationModel { UserName = username, VerificationCode = otp };
                        if (smsHasher.VerifyHashedPassword(smsModel, verifyUser.VerificationCode, otp).ToString() == "Success")
                        {
                            var update = Builders<BsonDocument>.Update.Set("Status", "Verified");
                            var filter = Builders<BsonDocument>.Filter.Eq("UserName", username);
                            var result = MH.UpdateSingleObject(filter, "Authentication", "Authentication", update).Result;
                            Parameters parameters = new Parameters { username = username, fullname = verifyUser.FullName };
                            return Ok(Json(authHelper.DoPassword(parameters, _repo, _settings)));
                        }
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "402",
                                Message = "OTP Invalid",
                                Data = null
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "401",
                            Message = "OTP Expired",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "RegisterVerification", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Login user and to record invalid login attempts</summary>
        /// <remarks>This api is used to login user</remarks>
        /// <param name="user">Login details of user</param>
        /// <response code="999">User is logged in and jwt token is returned</response>
        /// <response code="909">User is logged in but some error occured while getting jwt token </response>   
        /// <response code="401">User info is invalid</response>     
        /// <response code="402">User not verified </response>   
        /// <response code="404">User not found</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("login")]
        [SwaggerRequestExample(typeof(LoginModel), typeof(LoginDetails))]
        [ProducesResponseType(typeof(JsonResult), 999)]
        [ProducesResponseType(typeof(JsonResult), 909)]
        public ActionResult Login([FromBody]LoginModel user)
        {
            try
            {
                BsonDocument checkUser;
                checkUser = MH.CheckForDatas("UserName", user.UserName, null, null, "Authentication", "Authentication");
                if (checkUser != null)
                {
                    var verifyUser = BsonSerializer.Deserialize<RegisterModel>(checkUser);
                    if (verifyUser.Status == "Verified")
                    {
                        RegisterModel registerModel = new RegisterModel { UserName = user.UserName, Password = user.Password };
                        if (passwordHasher.VerifyHashedPassword(registerModel, verifyUser.Password, user.Password).ToString() == "Success")
                        {
                            Parameters parameters = new Parameters { username = user.UserName, fullname = verifyUser.FullName };
                            return Ok(Json(authHelper.DoPassword(parameters, _repo, _settings)));
                        }
                        else
                        {
                            var filter = Builders<BsonDocument>.Filter.Eq("UserName", user.UserName);
                            string response = MongoHelper.RecordLoginAttempts(filter);
                            if (response != "Failed")
                                return BadRequest(new ResponseData
                                {
                                    Code = "401",
                                    Message = "Invalid User Infomation",
                                    Data = null
                                });
                            else
                            {
                                return BadRequest(new ResponseData
                                {
                                    Code = "400",
                                    Message = "Failed",
                                    Data = null
                                });
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "User Not Verified",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "Login", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Reset password when the user forgets the current password</summary>
        /// <remarks>This api is used to change password when the user forgets surrent password</remarks>
        /// <param name="data">Data required to reset password when user forgets current password</param>
        /// <response code="200">Request to change password is processed successfully</response>
        /// <response code="404">User not found</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("forgotpassword")]
        [SwaggerRequestExample(typeof(ForgotPasswordModel), typeof(ForgotPasswordDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> ForgotPassword([FromBody]ForgotPasswordModel data)
        {
            try
            {
                string OTP;
                var checkUser = MH.CheckForDatas("UserName", data.UserName, null, null, "Authentication", "Authentication");
                var filter = Builders<BsonDocument>.Filter.Eq("UserName", data.UserName);
                var user = MH.GetSingleObject(filter, "Authentication", "Authentication").Result;
                if (user != null)
                {
                    var userData = BsonSerializer.Deserialize<RegisterModel>(user);
                    VerificationModel smsModel = new VerificationModel();
                    smsModel.UserName = data.UserName;
                    if (data.UserLocation == "IN")
                    {
                        Random codeGenerator = new Random();
                        OTP = codeGenerator.Next(0, 1000000).ToString("D6");
                        smsModel.VerificationCode = OTP;
                        SMSHelper.SendSMS(data.UserName, OTP);
                    }
                    else
                    {
                        OTP = Guid.NewGuid().ToString();
                        string link = GlobalHelper.GetIpConfig() + data.UserName + "/" + OTP + "/yes";
                        await EmailHelper.SendEmail(userData.FullName, data.UserName, link);
                    }
                    var update = Builders<BsonDocument>.Update.Set("Status", "Not Verified").Set("OTPExp", DateTime.UtcNow.AddMinutes(2))
                                                              .Set("VerificationCode", smsHasher.HashPassword(smsModel, OTP));
                    var result = MH.UpdateSingleObject(filter, "Authentication", "Authentication", update).Result;
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Success",
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "ForgetPassword", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Verify the user to change password</summary>
        /// <remarks>This api is used verify user to change password</remarks>
        /// <param name="username">UserName of user who need to change password</param>
        /// <param name="otp">OTP sent to user to verify</param>
        /// <response code="201">User is verified to change password</response> 
        /// <response code="401">OTP invalid </response>     
        /// <response code="402">OTP expired </response>   
        /// <response code="404">User not found</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpGet("forgotpassword/verification/{username}/{otp}")]
        [ProducesResponseType(typeof(JsonResult), 201)]
        public ActionResult ForgotPasswordVerification(string username, string otp)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("UserName", username);
                var user = MH.GetSingleObject(filter, "Authentication", "Authentication").Result;
                if (user != null)
                {
                    var verifyUser = BsonSerializer.Deserialize<RegisterModel>(user);
                    if (verifyUser.OTPExp > DateTime.UtcNow)
                    {
                        VerificationModel model = new VerificationModel { UserName = username, VerificationCode = otp };
                        if (smsHasher.VerifyHashedPassword(model, verifyUser.VerificationCode, otp).ToString() == "Success")
                        {
                            var update = Builders<BsonDocument>.Update.Set("Status", "Verified");
                            var result = MH.UpdateSingleObject(filter, "Authentication", "Authentication", update).Result;
                            Parameters parameters = new Parameters { username = username, fullname = verifyUser.FullName };
                            var response = authHelper.DoPassword(parameters, _repo, _settings);
                            response.Code = "201";
                            response.Message = "OTP Verified";
                            return Ok(Json(response));
                        }
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "401",
                                Message = "Invalied OTP",
                                Data = null
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "OTP Expired",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "ForgotPasswordVerification", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Change the password after the user is verified to change password</summary>
        /// <remarks>This api is used change password</remarks>
        /// <param name="data">Data need to change password</param>
        /// <response code="200">Password successfully changed</response>
        /// <response code="401">User not found</response>   
        /// <response code="404">User not found</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("forgotpassword/changepassword")]
        [SwaggerRequestExample(typeof(ChangePassword_ForgotPasswordModel), typeof(ChangePassword_ForgotPassword))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult ChangePassword([FromBody]ChangePassword_ForgotPasswordModel data)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("UserName", data.UserName);
                var user = MH.GetSingleObject(filter, "Authentication", "Authentication").Result;
                if (user != null)
                {
                    var verifyUser = BsonSerializer.Deserialize<RegisterModel>(user);
                    if (verifyUser.Status == "Verified")
                    {
                        RegisterModel registerModel = new RegisterModel { UserName = data.UserName, Password = data.Password };
                        var result = MH.UpdateSingleObject(filter, "Authentication", "Authentication", Builders<BsonDocument>.Update.Set("Password", passwordHasher.HashPassword(registerModel, data.Password))).Result;
                        return Ok(new ResponseData
                        {
                            Code = "200",
                            Message = "Password Changed Successfully",
                            Data = null
                        });
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "401",
                            Message = "User Not Verified to Change Password",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "ChangePassword", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Change password for the user account
        /// </summary>
        /// <remarks>This api is used to changepassword when the user knows the current password</remarks>
        /// <param name="data">Data required to change password</param>
        /// <response code="200">Password is changed successfully</response> 
        /// <response code="401">User info invalid</response>     
        /// <response code="404">User not found</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("changepassword")]
        [SwaggerRequestExample(typeof(ChangePasswordModel), typeof(ChangePasswordDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult ChangePasswordWhenLoggedIn([FromBody]ChangePasswordModel data)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("UserName", data.UserName);
                var user = MH.GetSingleObject(filter, "Authentication", "Authentication").Result;
                if (user != null)
                {
                    var verifyUser = BsonSerializer.Deserialize<RegisterModel>(user);
                    RegisterModel registerModel = new RegisterModel { UserName = verifyUser.UserName, Password = data.OldPassword };
                    if (passwordHasher.VerifyHashedPassword(registerModel, verifyUser.Password, data.OldPassword).ToString() == "Success")
                    {
                        var update = Builders<BsonDocument>.Update.Set("Password", passwordHasher.HashPassword(verifyUser, data.NewPassword));
                        var result = MH.UpdateSingleObject(filter, "Authentication", "Authentication", update).Result;
                        return Ok(new ResponseData
                        {
                            Code = "200",
                            Message = "Password Changed Successfully",
                            Data = null
                        });

                    }
                    else
                    {
                        string response = MongoHelper.RecordLoginAttempts(filter);
                        if (response != "Failed")
                            return BadRequest(new ResponseData
                            {
                                Code = "401",
                                Message = "Invalid User Infomation",
                                Data = null
                            });
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "400",
                                Message = "Failed",
                                Data = null
                            });
                        }
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "ChangePasswordWhenLoggedIn", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Deactivate account</summary>
        /// <remarks>This api is used to deactivate account</remarks>
        /// <param name = "data" >Login data</param>
        /// <response code="200">Account deactivated successfully</response>
        /// <response code="401">Invalid username or password</response>     
        /// <response code="404">User not found</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("deactivateaccount")]
        [SwaggerRequestExample(typeof(DeactivateAccountModel), typeof(DeactivateAccountDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult DeactivateAccount([FromBody]DeactivateAccountModel data)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("PhoneNumber", data.UserName);
                var user = MH.GetSingleObject(filter, "Authentication", "Authentication").Result;
                if (user != null)
                {
                    var verifyUser = BsonSerializer.Deserialize<RegisterModel>(user);
                    RegisterModel registerModel = new RegisterModel { UserName = data.UserName, Password = data.Password };
                    if (passwordHasher.VerifyHashedPassword(registerModel, verifyUser.Password, data.Password).ToString() == "Success")
                    {
                        var authCollection = _db.GetCollection<RegisterModel>("Authentication");
                        var response = authCollection.DeleteOneAsync(user);
                        return Ok(new ResponseData
                        {
                            Code = "200",
                            Message = "User Deactivated",
                            Data = null
                        });
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "401",
                            Message = "Invalid UserName or Password",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "DeactivateAccount", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Login using google</summary>
        /// <remarks>This api is used to login user using google</remarks>
        /// <param name="data">Data needed to login user using google</param>
        /// <response code="999">User is logged in using google and jwt token is returned</response>
        /// <response code="909">User is logged in using google but some error occured while getting jwt token </response>   
        /// <response code="401">Token is empty</response>     
        /// <response code="402">Token is invalid </response>   
        /// <response code="403">ID mismatch</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("externallogin/google")]
        [SwaggerRequestExample(typeof(SocialLoginModel), typeof(SocialLoginDetails))]
        [ProducesResponseType(typeof(JsonResult), 999)]
        [ProducesResponseType(typeof(JsonResult), 909)]
        public async Task<ActionResult> GoogleLogin([FromBody]SocialLoginModel data)
        {
            try
            {
                if (data.Token != null)
                {
                    string textResult;
                    using (var client = new HttpClient())
                    {
                        var uri = new Uri("https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + data.Token);

                        var response = await client.GetAsync(uri);

                        textResult = await response.Content.ReadAsStringAsync();
                    }
                    if (textResult.Contains("Invalid Value"))
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "Invalid token",
                            Data = null
                        });
                    }
                    else
                    {
                        var result = BsonSerializer.Deserialize<GoogleVerificationModel>(textResult);
                        if (result.sub == data.ID)
                        {
                            var checkUser = MH.CheckForDatas("UserName", result.email, null, null, "Authentication", "Authentication");
                            if (checkUser == null)
                            {
                                RegisterModel registerModel = new RegisterModel
                                {
                                    UserName = result.email,
                                    UserRole = "User",
                                    SocialId = result.sub,
                                    FullName = result.name,
                                    Status = "Verified",
                                    Email = result.email
                                };
                                var authCollection = _db.GetCollection<RegisterModel>("Authentication");
                                await authCollection.InsertOneAsync(registerModel);
                            }
                            Parameters parameters = new Parameters { username = result.email, fullname = result.name };
                            return Ok(Json(authHelper.DoPassword(parameters, _repo, _settings)));
                        }
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "403",
                                Message = "ID mismatch",
                                Data = null
                            });
                        }
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "401",
                        Message = "Token is empty",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "GoogleLogin", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Login using facebook</summary>
        /// <remarks>This api is used to login user using facebook</remarks>
        /// <param name="data">Data to login user using facebook</param>
        /// <response code="999">User is logged in using facebook and jwt token is returned</response>
        /// <response code="909">User is logged in using facebook but some error occured while getting jwt token </response>   
        /// <response code="401">Token is empty </response>     
        /// <response code="402">Token is invalid </response>   
        /// <response code="403">ID mismatch</response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("externallogin/facebook")]
        [SwaggerRequestExample(typeof(SocialLoginModel), typeof(SocialLoginDetails))]
        [ProducesResponseType(typeof(JsonResult), 999)]
        [ProducesResponseType(typeof(JsonResult), 909)]
        public async Task<ActionResult> FaceBookLogin([FromBody]SocialLoginModel data)
        {
            try
            {
                if (data.Token != null)
                {
                    string textResult;
                    using (var client = new HttpClient())
                    {
                        var uri = new Uri("https://graph.facebook.com/me?locale=en_US&fields=id,name&access_token=" + data.Token);

                        var response = await client.GetAsync(uri);

                        textResult = await response.Content.ReadAsStringAsync();
                    }
                    if (textResult.Contains("An active access token must be used to query information about the current user") || textResult.Contains("Malformed access token"))
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "Invalid token",
                            Data = null
                        });
                    }
                    else
                    {
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookVerificationModel>(textResult);
                        if (result.id == data.ID)
                        {
                            var checkUser = MH.CheckForDatas("SocialId", data.ID, null, null, "Authentication", "Authentication");
                            if (checkUser == null)
                            {
                                RegisterModel registerModel = new RegisterModel
                                {
                                    UserName = result.id,
                                    UserRole = "User",
                                    SocialId = result.id,
                                    FullName = result.name,
                                    Status = "Verified",
                                    Email = data.Email
                                };
                                var authCollection = _db.GetCollection<RegisterModel>("Authentication");
                                await authCollection.InsertOneAsync(registerModel);
                            }
                            Parameters parameters = new Parameters { username = result.id, fullname = result.name };
                            return Ok(Json(authHelper.DoPassword(parameters, _repo, _settings)));
                        }
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "403",
                                Message = "ID mismatch",
                                Data = null
                            });
                        }
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "401",
                        Message = "Token is empty",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "FaceBookLogin", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Chech if the user who logins using facebook is already registered</summary>
        /// <remarks>This api is used to check the whether the user login in with facebook is already registered or not</remarks>
        /// <param name="data">Data need to check if user is registered using facebook</param>
        /// <response code="999">User is logged in using google and jwt token is returned</response>
        /// <response code="909">User is logged in using google but some error occured while getting jwt token </response>   
        /// <response code="201">User not registered using facebook</response>     
        /// <response code="401">Token is empty </response>   
        /// <response code="400">Process ran into an exception</response> 
        [HttpPost("externallogin/facebook/check")]
        [SwaggerRequestExample(typeof(SocialLoginModel), typeof(SocialLoginDetails))]
        [ProducesResponseType(typeof(JsonResult), 999)]
        [ProducesResponseType(typeof(JsonResult), 909)]
        public async Task<ActionResult> FaceBookLoginCheck([FromBody]SocialLoginModel data)
        {
            try
            {
                if (data.Token != null)
                {
                    string textResult;
                    using (var client = new HttpClient())
                    {
                        var uri = new Uri("https://graph.facebook.com/me?locale=en_US&fields=id,name&access_token=" + data.Token);

                        var response = await client.GetAsync(uri);

                        textResult = await response.Content.ReadAsStringAsync();
                    }
                    if (textResult.Contains("An active access token must be used to query information about the current user") || textResult.Contains("Malformed access token"))
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "Invalid token",
                            Data = null
                        });
                    }
                    else
                    {
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookVerificationModel>(textResult);
                        var checkUser = MH.CheckForDatas("SocialId", result.id, null, null, "Authentication", "Authentication");
                        if (checkUser == null)
                        {
                            return Ok(new ResponseData
                            {
                                Code = "201",
                                Message = "User not found",
                                Data = null
                            });
                        }
                        else
                        {
                            var user = BsonSerializer.Deserialize<RegisterModel>(checkUser);
                            Parameters parameters = new Parameters { username = result.id, fullname = user.FullName };
                            return Ok(Json(authHelper.DoPassword(parameters, _repo, _settings)));
                        }
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "401",
                        Message = "Token is empty",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "GoogleLogin", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Get User Info</summary>
        /// <remarks>This api is get user information</remarks>
        /// <param name="username">UserName of user whose information to be retrieved</param>
        /// <response code="200">User information returned</response>
        /// <response code="404">User not found </response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpGet("userinfo/{username}")]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public ActionResult GetUserInfo(string username)
        {
            try
            {
                var checkUser = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
                if (checkUser != null)
                {
                    var userModel = BsonSerializer.Deserialize<RegisterModel>(checkUser);
                    return Ok(new ResponseData
                    {
                        Code = "200",
                        Message = "Success",
                        Data = new UserInfo
                        {
                            FullName = userModel.FullName,
                            UserName = userModel.UserName,
                            Email = userModel.Email,
                            DialCode = userModel.DialCode,
                            PhoneNumber = userModel.PhoneNumber
                        }
                });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "GetUserInfo", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Update user's fullname</summary>
        /// <param name="data">Update data for fullname</param>
        /// <param name="username">UserName of user whoes fullname needs to be updated</param>
        /// <response code="200">FullName for user updated successfully</response>
        /// <response code="401">FullName update failed</response> 
        /// <response code="402">Update data for fullname not found</response> 
        /// <response code="404">User not found </response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPut("updateuserinfo/fullname/{username}")]
        [SwaggerRequestExample(typeof(FullNameUpdateModel), typeof(UpdateFullNameDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> UpdateFullName([FromBody]FullNameUpdateModel data,string username)
        {
            try
            {
                var checkUser = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
                if (checkUser != null)
                {
                    if (data.FullName != null)
                    {
                        var update = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication", Builders<BsonDocument>.Update.Set("FullName", data.FullName));
                        if (update == true)
                        {
                            return Ok(new ResponseData
                            {
                                Code = "200",
                                Message = "FullName updated successfully",
                                Data = null
                            });
                        }
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "401",
                                Message = "FullName update failed",
                                Data = null
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "Update data for fullname not found",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "UpdateFullName", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Update user's fullname</summary>
        /// <param name="data">Update data for phonenumber</param>
        /// <param name="username">UserName of user whoes phonenumber need to be updated</param>        
        /// <response code="200">PhoneNumber, dialcode and username for user updated successfully</response>
        /// <response code="401">Update data for phonenumber not found</response> 
        /// <response code="402">Update data for dialcode not found</response> 
        /// <response code="404">User not found </response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPut("updateuserinfo/phonenumber/{username}")]
        [SwaggerRequestExample(typeof(PhoneNumberUpdateModel), typeof(UpdatePhoneNumberDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> UpdatePhoneNumber([FromBody]PhoneNumberUpdateModel data, string username)
        {
            try
            {
                var checkUser = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
                if (checkUser != null)
                {
                    if (data.DialCode != null)
                    {
                        if (data.PhoneNumber != null)
                        {
                            var updateDialCode = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication", Builders<BsonDocument>.Update.Set("DialCode", data.DialCode));
                            var updatePhoneNumber = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication", Builders<BsonDocument>.Update.Set("PhoneNumber", data.PhoneNumber));
                            if(username == BsonSerializer.Deserialize<RegisterModel>(checkUser).PhoneNumber)
                            {
                                var updateUserName = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication", Builders<BsonDocument>.Update.Set("UserName", data.PhoneNumber));
                            }
                            return Ok(new ResponseData
                            {
                                Code = "200",
                                Message = "PhoneNumber, dialcode and username updated successfully",
                                Data = null
                            });
                        }
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "401",
                                Message = "Update data for phonenumber not found",
                                Data = null
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "Update data for dialcode not found",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "UpdatePhoneNumber", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Update user's email id</summary>
        /// <param name="data">Update data for email</param>
        /// <param name="username">UserName of user whoes email needs to be updated</param>
        /// <response code="200">Email and username for user updated successfully</response>
        /// <response code="401">Email update failed</response> 
        /// <response code="402">Update data for email not found</response> 
        /// <response code="404">User not found </response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPut("updateuserinfo/email/{username}")]
        [SwaggerRequestExample(typeof(EmailUpdateModel), typeof(UpdateEmailDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> UpdateEmailId([FromBody]EmailUpdateModel data,string username)
        {
            try
            {
                var checkUser = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
                if (checkUser != null)
                {
                    if (data.Email != null)
                    {
                        var update = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication", Builders<BsonDocument>.Update.Set("Email", data.Email));
                        if (username == BsonSerializer.Deserialize<RegisterModel>(checkUser).Email)
                        {
                            var updateUserName = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication", Builders<BsonDocument>.Update.Set("UserName", data.Email));
                        }
                        if (update == true)
                        {
                            return Ok(new ResponseData
                            {
                                Code = "200",
                                Message = "Email and username updated successfully",
                                Data = null
                            });
                        }
                        else
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "401",
                                Message = "FullName update failed",
                                Data = null
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "402",
                            Message = "Update data for email not found",
                            Data = null
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "UpdateEmailId", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }

        /// <summary>Update user's fullname</summary>
        /// <param name="data">Update data for password</param>
        /// <param name="username">UserName of user whoes password needs to be changed</param>
        /// <response code="200">New password for user updated successfully</response>
        /// <response code="401">Current password not found</response> 
        /// <response code="402">New password not found</response> 
        /// <response code="403">New password update failed</response> 
        /// <response code="404">User not found </response> 
        /// <response code="404">Current password incorrect </response> 
        /// <response code="400">Process ran into an exception</response> 
        [HttpPut("updateuserinfo/password/{username}")]
        [SwaggerRequestExample(typeof(PasswordUpdateModel), typeof(UpdatePasswordDetails))]
        [ProducesResponseType(typeof(ResponseData), 200)]
        public async Task<ActionResult> UpdatePassword([FromBody]PasswordUpdateModel data, string username)
        {
            try
            {
                var checkUser = MH.CheckForDatas("UserName", username, null, null, "Authentication", "Authentication");
                if (checkUser != null)
                {
                    var userData = BsonSerializer.Deserialize<RegisterModel>(checkUser);
                    if (data.CurrentPassword == null)
                    {
                        return BadRequest(new ResponseData
                        {
                            Code = "401",
                            Message = "Current password not found",
                            Data = null
                        });
                    }
                    else
                    {
                        if (data.NewPassword == null)
                        {
                            return BadRequest(new ResponseData
                            {
                                Code = "402",
                                Message = "New password not found",
                                Data = null
                            });
                        }
                        else
                        {
                            RegisterModel registerModel = new RegisterModel { UserName = username, Password = data.CurrentPassword };
                            if (passwordHasher.VerifyHashedPassword(registerModel, userData.Password, data.CurrentPassword).ToString() == "Success")
                            {
                                var updateData = Builders<BsonDocument>.Update.Set("Password", passwordHasher.HashPassword(userData, data.NewPassword));
                                var updatePassword = await MH.UpdateSingleObject(Builders<BsonDocument>.Filter.Eq("UserName", username), "Authentication", "Authentication", updateData);
                                if (updatePassword == false)
                                {
                                    return BadRequest(new ResponseData
                                    {
                                        Code = "403",
                                        Message = "New password update failed",
                                        Data = null
                                    });
                                }
                                else
                                {
                                    return Ok(new ResponseData
                                    {
                                        Code = "200",
                                        Message = "Password Changed Successfully",
                                        Data = null
                                    });
                                }
                            }
                            else
                            {
                                return BadRequest(new ResponseData
                                {
                                    Code = "405",
                                    Message = "Current password incorrect",
                                    Data = null
                                });
                            }
                        }
                    }
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        Code = "404",
                        Message = "User Not Found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("AuthController", "UpdateFullName", ex.Message);
                return BadRequest(new ResponseData
                {
                    Code = "400",
                    Message = "Failed",
                    Data = null
                });
            }
        }
    }
}

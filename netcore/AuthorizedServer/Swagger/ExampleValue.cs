using AuthorizedServer.Models;
using Swashbuckle.AspNetCore.Examples;

namespace AuthorizedServer.Swagger
{
    #region TokenController

    /// <summary></summary>
    public class ParameterDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                username = "sample@gmail.com",
                fullname = "Sample User"
            };
        }
    }

    #endregion

    #region AuthController

    /// <summary></summary>
    public class RegisterDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                Title = "Mr",
                FullName = "Sample User",
                DialCode = "+91",
                PhoneNumber = "12341234",
                Email = "sample@gmail.com",
                Password = "asd123",
                UserLocation = "IN"
            };
        }
    }

    /// <summary></summary>
    public class LoginDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                UserName = "12341234",
                Password = "asd123",
            };
        }
    }

    /// <summary></summary>
    public class ForgotPasswordDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                UserName = "12341234",
                UserLocation = "IN",
            };
        }
    }

    /// <summary></summary>
    public class ChangePassword_ForgotPassword : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                UserName = "12341234",
                Password = "qwe123",
            };
        }
    }

    /// <summary></summary>
    public class ChangePasswordDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                UserName = "12341234",
                OldPassword = "asd123",
                NewPassword = "qwe123"
            };
        }
    }

    /// <summary></summary>
    public class DeactivateAccountDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                UserName = "12341234",
                Password = "asd123",
            };
        }
    }

    /// <summary></summary>
    public class SocialLoginDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                Token = "Token received from google or facebook",
                Email = "sample@gmail.com",
                ID = "ID given by google or facebook"
            };
        }
    }

    /// <summary></summary>
    public class UpdateFullNameDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new
            {
                FullName = "Updated Sample User"
            };
        }
    }

    /// <summary></summary>
    public class UpdatePhoneNumberDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                DialCode = "+11",
                PhoneNumber = "23452345"
            };
        }
    }

    /// <summary></summary>
    public class UpdateEmailDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                Email = "updatedsample@email.com"
            };
        }
    }

    /// <summary></summary>
    public class UpdatePasswordDetails : IExamplesProvider
    {
        /// <summary></summary>
        public object GetExamples()
        {
            return new 
            {
                CurrentPassword = "asd123",
                NewPassword = "qwe123"
            };
        }
    }

    #endregion
}

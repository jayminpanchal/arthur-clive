using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace Arthur_Clive.Data
{
    /// <summary>Contains registration details of the user </summary>
    public class RegisterModel
    {
        /// <summary>ObjectId given by MongoDB</summary>
        public ObjectId _id { get; set; }
        /// <summary>Titile of the user</summary>
        [Required]
        public string Title { get; set; }
        /// <summary>Fullname of user</summary>
        [Required]
        public string FullName { get; set; }
        /// <summary>Username of user</summary>
        public string UserName { get; set; }
        /// <summary>Role for user</summary>
        public string UserRole { get; set; }
        /// <summary>Dialcode for phonenumber of the user</summary>
        [Required]
        public string DialCode { get; set; }
        /// <summary>Phonenumber of the user</summary>
        [Required]
        public string PhoneNumber { get; set; }
        /// <summary>Email of the user</summary>
        [Required]
        public string Email { get; set; }
        /// <summary>SocialId of user incase the user login using facebook and gmail</summary>
        [Required]
        public string SocialId { get; set; }
        /// <summary>Password of the user</summary>
        [Required]
        public string Password { get; set; }
        /// <summary>Location of the user</summary>
        [Required]
        public string UserLocation { get; set; }
        /// <summary>Verification code sent ot the user</summary>
        public string VerificationCode { get; set; }
        /// <summary>Status of the user</summary>
        public string Status { get; set; }
        /// <summary>Count of invalid login attempts</summary>
        public int WrongAttemptCount { get; set; }
        /// <summary>OTP expiration time</summary>
        public DateTime OTPExp { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Arthur_Clive.Data
{
    /// <summary>Contains to make payment through PayUMoney</summary>
    public class PaymentModel
    {
        /// <summary>UserName of user</summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>FirstName of user</summary>
        [Required]
        public string FirstName { get; set; }
        /// <summary>LastName of user</summary>
        [Required]
        public string LastName { get; set; }
        /// <summary>ProductInfo of product from which the payment is made</summary>
        [Required]
        public string ProductInfo { get; set; }
        /// <summary>Amount to be paid for order</summary>
        [Required]
        public string Amount { get; set; }
        /// <summary>Email of user</summary>
        [Required]
        public string Email { get; set; }
        /// <summary>PhoneNumber of user</summary>
        [Required]
        public string PhoneNumber { get; set; }
        /// <summary>First line of address </summary>
        [Required]
        public string AddressLine1 { get; set; }
        /// <summary>Secound line of address</summary>
        [Required]
        public string AddressLine2 { get; set; }
        /// <summary>City of user</summary>
        [Required]
        public string City { get; set; }
        /// <summary>State of user </summary>
        [Required]
        public string State { get; set; }
        /// <summary>Country of user </summary>
        [Required]
        public string Country { get; set; }
        /// <summary>Zipcode of user location</summary>
        [Required]
        public string ZipCode { get; set; }
        /// <summary>Id of order</summary>
        [Required]
        public long OrderId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Arthur_Clive.Data
{
    /// <summary>Details of review added by user</summary>
    public class Review
    {
        /// <summary>Id of review</summary>
        public long Id { get; set; }
        /// <summary>Name of the user who adds the review</summary>
        [Required]
        public string Name { get; set; }
        /// <summary>Review given by the user</summary>
        [Required]
        public string Comment { get; set; }
        /// <summary>Rating given by the user</summary>
        [Required]
        public double Rating { get; set; }
        /// <summary>Id of order placed</summary>
        [Required]
        public long OrderId { get; set; }
        /// <summary>Flag to determine if the review is approved by the admin or not</summary>
        public bool? Approved { get; set; }
    }

    /// <summary>Details needed to update review</summary>
    public class UpdateReview
    {
        /// <summary>Id of review</summary>
        [Required]
        public long Id { get; set; }
        /// <summary>Flag to determine if the review is approved by the admin or not</summary>
        [Required]
        public bool? Approved { get; set; }
    }

    /// <summary>Review for each product</summary>
    public class ReviewsForEachProduct
    {
        /// <summary>SKU of product</summary>
        [Required]
        public string ProductSKU { get; set; }
        /// <summary>Reviews of the product</summary>
        [Required]
        public Review[] ProductReviews { get; set; }
    }
}

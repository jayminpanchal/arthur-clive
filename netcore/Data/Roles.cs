using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace Arthur_Clive.Data
{
    /// <summary>Details of role</summary>
    public class Roles
    {
        /// <summary>Object id given by mongodb</summary>
        public ObjectId _id { get; set; }
        /// <summary>Id of user role</summary>
        [Required]
        public int RoleID { get; set; }
        /// <summary>Name of role</summary>
        [Required]
        public string RoleName { get; set; }
        /// <summary>Level of user access</summary>
        [Required]
        public List<string> LevelOfAccess { get; set; }
    }
}

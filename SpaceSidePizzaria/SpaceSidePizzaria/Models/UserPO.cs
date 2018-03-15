using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SpaceSidePizzaria.Models
{
    public class UserPO
    {
        [Required]
        [DisplayName("User ID")]
        public long UserID { get; set; }

        [Required]
        [StringLength(26, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [StringLength(26, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(70)]
        public string Email { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(30, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(30, MinimumLength = 1)]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Date Added")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateAdded { get; set; }

        [DisplayName("Role")]
        public byte RoleID { get; set; }

        public List<SelectListItem> RoleSelectListItems {get; set;}

        [StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        [StringLength(140)]
        public string Address { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        [StringLength(20)]
        public string State { get; set; }
    }
}
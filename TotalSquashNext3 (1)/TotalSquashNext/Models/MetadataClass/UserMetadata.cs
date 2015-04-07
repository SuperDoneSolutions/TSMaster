using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TotalSquashNext.Models.MetadataClasses
{
    public class UserMetadata
    {
        [Display(Name = "User ID")]
        public int id { get; set; }

        [Display(Name = "Username")]
        public string username { get; set; }

        [Display(Name = "Skill Level")]
        public int skillId { get; set; }

        [Display(Name = "Password")]
        public string password { get; set; }

        [Display(Name = "Photo")]
        public string photo { get; set; }

        [Display(Name = "Wins")]
        public Nullable<int> wins { get; set; }

        [Display(Name = "Losses")]
        public Nullable<int> losses { get; set; }

        [Display(Name = "Ties")]
        public Nullable<int> ties { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Address")]
        public string streetAddress { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "Province")]
        public string provinceId { get; set; }

        [Display(Name = "Country")]
        public int countryId { get; set; }

        [Display(Name = "Phone Number")]
        public string phoneNumber { get; set; }

        [Display(Name = "E-mail")]
        public string emailAddress { get; set; }

        [Display(Name = "Identified Gender")]
        public string gender { get; set; }

        [Display(Name = "Date of Birth")]
        public System.DateTime birthDate { get; set; }

        [Display(Name = "Account Type")]
        public int accountId { get; set; }

        [Display(Name = "Locked")]
        public bool locked { get; set; }

        [Display(Name = "Organization")]
        public int organizationId { get; set; }

        [Display(Name = "Postal Code")]
        public string postalCode { get; set; }
    }
}
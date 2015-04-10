//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TotalSquashNext.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class User
    {
        public User()
        {
            this.UserMatches = new HashSet<UserMatch>();
            this.UserLadders = new HashSet<UserLadder>();
        }

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

        [Display(Name = "Date of Birth (DD/MMM/YYYY)")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime birthDate { get; set; }

        [Display(Name = "Account Type")]
        public int accountId { get; set; }

        [Display(Name = "Locked")]
        public bool locked { get; set; }

        [Display(Name = "Organization")]
        public int organizationId { get; set; }

        [Display(Name = "Postal Code")]
        [RegularExpression(@"[A-Za-z]\d[A-Za-z] ?\d[A-Za-z]\d", ErrorMessage = "Please enter as: L7N2Y2")]
        public string postalCode { get; set; }
    
        public virtual AccountType AccountType { get; set; }
        public virtual Country Country { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Province Province { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual ICollection<UserMatch> UserMatches { get; set; }
        public virtual ICollection<UserLadder> UserLadders { get; set; }
    }
}

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
    
    public partial class Organization
    {
        public Organization()
        {
            this.Buildings = new HashSet<Building>();
            this.Users = new HashSet<User>();
        }
        [Display(Name = "Organization")]
        public int organizationId { get; set; }
        [Display(Name = "Organization Name")]
        public string orgName { get; set; }
        [Display(Name = "Organization Address")]
        public string orgAddress { get; set; }
        [Display(Name = "Organization City")]
        public string orgCity { get; set; }
        [Display(Name = "Organization Phone Number")]
        public string orgPhoneNumber { get; set; }
    
        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}

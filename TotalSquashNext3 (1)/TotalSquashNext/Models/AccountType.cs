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
    
    public partial class AccountType
    {
        public AccountType()
        {
            this.Users = new HashSet<User>();
        }

        [Display(Name = "Account")]
        public int accountId { get; set; }
       
        [Display(Name = "Description")]
        public string description { get; set; }
    
        public virtual ICollection<User> Users { get; set; }
    }
}

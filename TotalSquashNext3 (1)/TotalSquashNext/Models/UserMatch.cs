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
    
    public partial class UserMatch
    {
        [Display(Name = "User ID")]
        public int userId { get; set; }

        [Display(Name = "Game ID")]
        public int gameId { get; set; }

        public Nullable<int> score { get; set; }
    
        public virtual Match Match { get; set; }
        public virtual User User { get; set; }
    }
}

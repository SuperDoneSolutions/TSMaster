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
    
    public partial class Match
    {
        public Match()
        {
            this.UserMatches = new HashSet<UserMatch>();
        }

        [Display(Name = "Match")]
        public int matchId { get; set; }

        [Display(Name = "Booking Number")]
        public int bookingNumber { get; set; }
    
        public virtual Booking Booking { get; set; }
        public virtual ICollection<UserMatch> UserMatches { get; set; }
    }
}

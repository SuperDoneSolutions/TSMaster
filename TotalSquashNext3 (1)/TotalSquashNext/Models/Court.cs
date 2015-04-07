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
    
    public partial class Court
    {
        public Court()
        {
            this.Bookings = new HashSet<Booking>();
        }

        [Display(Name = "Court")]
        public int courtId { get; set; }

        [Display(Name = "Court Description")]
        public string courtDescription { get; set; }

        [Display(Name = "Court Image")]
        public string courtImage { get; set; }

        [Display(Name = "Doubles Court")]
        public bool doublesCourt { get; set; }

        [Display(Name = "Building")]
        public int buildingId { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual Building Building { get; set; }
    }
}

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
    
    public partial class Ladder
    {
        public Ladder()
        {
            this.UserLadders = new HashSet<UserLadder>();
        }
        [Display(Name = "Ladder")]
        public int ladderId { get; set; }
        [Display(Name = "Ladder Description")]
        public string ladderDescription { get; set; }
        [Display(Name = "Ladder Rules")]
        public int ladderRuleId { get; set; }
    
        public virtual ICollection<UserLadder> UserLadders { get; set; }
        public virtual LadderRule LadderRule { get; set; }
    }
}

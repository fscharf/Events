//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Events.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Event
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public System.DateTime Date { get; set; }
        public System.TimeSpan Hour { get; set; }
        public string Local { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Image { get; set; }
    
        public virtual User User { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wema_CAM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public long id { get; set; }
        public long CAMId { get; set; }
        public string Username { get; set; }
        public string Designation { get; set; }
        public string Comment1 { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
    }
}
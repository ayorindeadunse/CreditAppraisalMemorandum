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
    
    public partial class ApplicantsBrief
    {
        public long ApplicantID { get; set; }
        public Nullable<long> ApplicantType { get; set; }
        public string CustomerID { get; set; }
        public Nullable<int> AccountNumber { get; set; }
        public string LegalIdentityName { get; set; }
        public string YearOfIncorporation { get; set; }
        public string RCNumber { get; set; }
        public string BusinessAddress { get; set; }
        public string NameOfAuditors { get; set; }
        public string NatureOfBusiness { get; set; }
        public Nullable<long> Industry { get; set; }
        public string ProductRange { get; set; }
        public string MajorClients { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> DateLastUpdated { get; set; }
        public string UpdateUserId { get; set; }
    }
}
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
    
    public partial class CreditRequestSummary
    {
        public Nullable<long> ApplicantID { get; set; }
        public Nullable<long> CAMId { get; set; }
        public long FacilityID { get; set; }
        public Nullable<long> Facility { get; set; }
        public Nullable<decimal> FacilityAmount { get; set; }
        public Nullable<int> ApplicationType { get; set; }
        public string Purpose { get; set; }
        public string Tenor { get; set; }
        public string InterestRate { get; set; }
        public string ProcessingFee { get; set; }
        public string ManagementFee { get; set; }
        public string LCCommission { get; set; }
        public string OtherCharges { get; set; }
        public string COTRate { get; set; }
        public string SourceOfRepayment { get; set; }
        public string ModeOfRepayment { get; set; }
        public string Security { get; set; }
        public string Guarantee { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> DateLastUpdated { get; set; }
        public string UpdateUserId { get; set; }
    }
}
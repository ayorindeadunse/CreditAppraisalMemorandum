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
    using System.ComponentModel.DataAnnotations;

    public partial class CAM
    {
        public long CAMId { get; set; }
        public Nullable<long> ApplicantID { get; set; }
        public string BranchProcessedFrom { get; set; }
        public string ZoneProcessedFrom { get; set; }
        public string RegionProcessedFrom { get; set; }
        [Display(Name = "OWNERSHIP")]
        public string Ownership { get; set; }
        [Display(Name = "BOARD AND MANAGEMENT")]
        public string BoardAndManagement { get; set; }
        [Display(Name = "PROFILE OF BOARD AND MANAGEMENT")]
        public string ProfileOfBoardAndManagement { get; set; }
        [Display(Name = "HISTORY AND SCOPE OF OPERATION")]
        public string HistoryAndScopeOfOperation { get; set; }
        [Display(Name = "BORROWING POWER OF DIRECTORS (EXTRACT FROM MEMART)")]
        public string BorrowingPowerOfDirectors { get; set; }
        [Display(Name = "INDUSTRY ANALYSIS")]
        public string IndustryAnalysis { get; set; }
        [Display(Name = " BACKGROUND TO THE REQUEST")]
        public string BackgroundToTheRequest { get; set; }
        [Display(Name = "TRANSACTION DETAILS")]
        public string TransactionDetails { get; set; }
        [Display(Name = "ACCOUNT PERFORMANCE WITH WEMA BANK ")]
        public string AccountPerformanceWithWemaBank { get; set; }
        [Display(Name = "HIGHLIGHTS OF OTHER PERFORMANCE INDICATORS ")]
        public string HighlightsOfOtherPerformanceIndicators { get; set; }
        [Display(Name = "APPROVED CREDIT LIMIT")]
        public string Balance { get; set; }
        [Display(Name = "SWING ANALYSIS FOR THE LAST SIX MONTHS")]
        public string SwingAnalysisForLastSixMonths { get; set; }
        [Display(Name = "COMMENTS ON ACCOUNT PERFORMANCE")]
        public string CommentsOnAccountPerformance { get; set; }
        [Display(Name = "ACCOUNT PERFORMANCE WITH OTHER BANKS ")]
        public string AccountPerformanceWithOtherBanks { get; set; }
        [Display(Name = "PROPORTIONAL ANALYSIS OF COMPANY�S TURNOVER/BORROWINGS")]
        public string ProportionalAnalysisOfCompanyTurnover { get; set; }
        [Display(Name = "TOTAL BORROWING FROM WEMA AND OTHER BANKS")]
        public string TotalBorrowingFromWemaAndOtherBanks { get; set; }
        [Display(Name = "RELATED/GROUP FACILITIES(INDIRECT BORROWING FROM WEMA BANK PLC")]
        public string RelatedFacilities { get; set; }
        [Display(Name = "EXISTING BORROWING FROM OTHER BANKS")]
        public string ExistingBorrowingFromOtherBanks { get; set; }
        [Display(Name = "CREDIT CHECK ON CRMS, PRIVATE CREDIT BUREAUX AND BANK REFERENCE")]
        public string CreditCheckOnCRMS { get; set; }
        [Display(Name = "FINANCIAL REVIEW")]
        public string FinancialReview { get; set; }
        [Display(Name = "CAPITAL ADEQUACY")]
        public string CapitalAdequacy { get; set; }
        [Display(Name = "TRANSACTION DYNAMICS")]
        public string TransactionDynamics { get; set; }
        [Display(Name = "CONDITIONS PRECEDENT TO DRAWDOWN")]
        public string ConditionsPrecedentToDrawdown { get; set; }
        [Display(Name = "CONDITIONS AFTER DRAWDOWN")]
        public string ConditionsAfterDrawdown { get; set; }
        [Display(Name = "RISK CONTROL MEASURES")]
        public string RiskControlMeasures { get; set; }
        [Display(Name = "KEY RISKS AND MITIGATING MEASURES")]
        public string KeyRisksAndMitigatingMeasures { get; set; }
        [Display(Name = "JUSTIFICATION FOR THE CREDIT")]
        public string JustificationForTheCredit { get; set; }
        [Display(Name = "SECURITY")]
        public string Security { get; set; }
        [Display(Name = "GUARANTEE")]
        public string Guarantee { get; set; }
        [Display(Name = "CREDIT RISK RATING")]
        public string CreditRiskRating { get; set; }
        [Display(Name = "REGIONAL EXECUTIVE'S COMMENTS")]
        public string RegionalExecutiveComments { get; set; }
        [Display(Name = "CREDIT QUALITY ASSURANCE REVIEW")]
        public string CreditQualityAssuranceReview { get; set; }
        [Display(Name = "RECOMMENDATION")]
        public string Recommendation { get; set; }
        [Display(Name = "TOTAL FACILITY AMOUNT")]
        public Nullable<decimal> TotalFacilityAmount { get; set; }
        [Display(Name = "STATUS")]
        public string Status { get; set; }
        [Display(Name = "INITIATED BY")]
        public string InitiatedBy { get; set; }
        [Display(Name = "NEXT IN QUEUE")]
        public string DestinationUserId { get; set; }
        [Display(Name = "DATE CREATED")]
        public Nullable<System.DateTime> CreationDate { get; set; }
        [Display(Name = "DATE LAST UPDATED")]
        public Nullable<System.DateTime> DateLastUpdated { get; set; }
        [Display(Name = "USER ID")]
        public string UpdateUserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wema_CAM
{
    public class creditrequest
    {
        public long camid { get; set; }
        public string facilitytype { get; set; }
        public decimal? facilityamount { get; set; }
        public string applicanttype { get; set; }
        public string purpose { get; set; }
        public string tenor { get; set; }
        public string interestrate { get; set; }
        public string processingfee { get; set; }
        public string managementfee { get; set; }
        public string lccommission { get; set; }
        public string othercharges { get; set; }
        public string cotrate { get; set; }
        public string sourceofrepayment { get; set; }
        public string modeofrepayment { get; set; }
        public string security { get; set; }
        public string guarantee { get; set; }


    }
}
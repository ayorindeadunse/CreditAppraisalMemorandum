using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wema_CAM
{
    public class creditrequestobjects
    {
        public long applicantid { get; set; }
        public string camid { get; set; }
        public string status { get; set; }
        public string facilitytype { get; set; }
        public decimal? facilitylimit { get; set; }
        public string facilitytenor { get; set; }
        public string applicationtype {get;set;}
        public string collateral { get; set; }
        public string interestrate { get; set; }
        public string proposedfees { get; set; }
        public string industry { get; set; }
        public string customer { get; set; }
        public DateTime DateCreated { get; set; }
        
    }
}
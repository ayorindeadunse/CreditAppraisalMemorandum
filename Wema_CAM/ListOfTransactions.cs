using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wema_CAM
{
    public class ListOfTransactions
    {
        public long camid { get; set; }
        public string customername { get; set; }
        public string facilitytype { get; set; }
        public decimal? facilityamount { get; set; }
        public string branchprocessed { get; set; }
        public string zoneprocessed { get; set; }
        public string regionprocessed { get; set; }
        public string status { get; set; }
        public string initiatedby { get; set; }
        public string currentlocation { get; set; }  
       public string  datelastupdatedoutput { get; set; }
     
    }
}
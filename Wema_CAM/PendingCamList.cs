using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wema_CAM
{
    public class PendingCamList
    {
        public long camid { get; set; }
        public string initiatedby { get; set; }
        public string destinationuser { get; set; }
        public string customername { get; set; }
        public string facilitytype { get; set; }
        public string facilityamount { get; set; }
        public DateTime dateinitiated { get; set; }
        public DateTime datelastupdated { get; set; }
        public string status { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NgxAPI.Models
{
    public class WhiteList_Req
    {
        public string domain { get; set; }
        public List<WhiteList_Req_Data> ips { get; set; }
    }

    public class WhiteList_Req_Data
    {
        public string ip { get; set; }
        public string desc { get; set; }
    }
}

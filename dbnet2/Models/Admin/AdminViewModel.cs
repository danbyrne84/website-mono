using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dbnet2.Models.Admin
{
    public class AdminViewModel
    {
        public VpnInformation VpnConnection { get; set; }

        public AdminViewModel()
        {
            VpnConnection = new VpnInformation
            {
                Enabled = true,
                Configuration = "EU AirVPN 443",
                Received = 3242.1231,
                Sent = 126.3212
            };
        }
    }

    public class VpnInformation
    {
        public bool Enabled { get; set; }
        public string Configuration { get; set; }
        public double Sent { get; set; }
        public double Received { get; set; }
    }
}
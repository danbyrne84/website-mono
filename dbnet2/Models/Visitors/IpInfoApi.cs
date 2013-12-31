using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace dbnet2.Models.Visitors
{
    public class IpInfoApi
    {
        public IpInfoResponse GetIpaddressDetails(string ipAddress)
        {
            WebClient c = new WebClient();
            var json = c.DownloadString(String.Format("http://ipinfo.io/{0}/json", ipAddress));
            
            var response = JsonConvert.DeserializeObject<IpInfoResponse>(json);
            return response;
        }
    }

    public class IpInfoResponse
    {
        public string Country { get; set; }
        public string City { get; set; }
    }
}
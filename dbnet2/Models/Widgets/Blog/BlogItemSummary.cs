using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;

namespace dbnet2.Models
{
    public class BlogItemSummary
    {
        public string Subject { get; set; }
        public string Summary { get; set; }
        
        public DateTimeOffset PublishDate { get; set; }
        public Uri Link { get; set; }
    }
}
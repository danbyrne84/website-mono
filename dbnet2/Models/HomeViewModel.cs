using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dbnet2.Models
{
    public class HomeViewModel
    {
        public IList<BlogItemSummary> BlogItems;
        public IList<Visitor> LatestVisitors;
    }
}
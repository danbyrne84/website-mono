using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dbnet2.Models
{
    public class ViewModelBase
    {
        public IList<Visitor> LatestVisitors { get; set; }
    }
}
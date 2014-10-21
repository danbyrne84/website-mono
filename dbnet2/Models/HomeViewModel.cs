using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dbnet2.Models.Widgets.BuildMonitor;

namespace dbnet2.Models
{
    public class HomeViewModel : ViewModelBase
    {
        public BuildMonitorSummary BuildStatus { get; set; }
        public IList<BlogItemSummary> BlogItems { get; set; }
    }
}
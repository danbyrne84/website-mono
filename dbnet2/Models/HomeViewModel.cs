﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dbnet2.Models
{
    public class HomeViewModel : ViewModelBase
    {
        public IList<BlogItemSummary> BlogItems { get; set; }
    }
}
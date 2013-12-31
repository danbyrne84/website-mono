using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using dbnet2.Models;
using dbnet2.Models.Blog;
using dbnet2.Models.Visitors;

namespace dbnet2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var blogUrl = new Uri(ConfigurationManager.AppSettings["blogRssUrl"]);
            var accessLogPath = ConfigurationManager.AppSettings["accessLog"];

            var blogReader = new BlogReader(blogUrl);
            var accessLogReader = new AccessLogReader(accessLogPath);

            var viewModel = new HomeViewModel 
            { 
                BlogItems = blogReader.GetSummaryOfLatest(5), 
                LatestVisitors = accessLogReader.GetLastVisitors(5)
            };

            return View(viewModel);
        }
    }
}

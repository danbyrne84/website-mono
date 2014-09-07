using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dbnet2.Controllers
{
    [Authorize]
    public class CalenderController : Controller
    {
        //
        // GET: /Calender/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}

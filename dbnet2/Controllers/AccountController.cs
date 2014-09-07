using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using dbnet2.Models.Account;

namespace dbnet2.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {
            var htpasswdPath = ConfigurationManager.AppSettings["htpasswdPath"];
            var checker = new Htpasswd(htpasswdPath);

            if (checker.Validate(model.Username, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Username, true);
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect");
            }

            return View(model);
        }

    }
}

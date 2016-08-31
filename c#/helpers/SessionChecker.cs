using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fb_mvc.Classes
{
    public class SessionChecker : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["curr_user"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            HttpCookie idCookie = HttpContext.Current.Request.Cookies["_appid"];
            if (idCookie != null)
            {
                filterContext.Result = new RedirectResult("Default?uniqueid=" + idCookie.Value);
            }
            else
            {
                filterContext.Result = new RedirectResult("Default/SessionTimeout");
            }
        }
    }
}
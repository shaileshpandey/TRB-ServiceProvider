using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using TRB_ServiceProvider.Attributes;
using System.Linq;

namespace TRB_ServiceProvider.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewBag.Message = "Welcome to service provider exclusively developed for CEB.";
      var users = new List<SelectListItem>
        {
          new SelectListItem
            {
              Value = "Syudkovsky@executiveboard.com",
              Text = "syudkovsky@executiveboard.com",
              Selected = true
            },
          new SelectListItem
            {
              Value = "Shailesh.Pandey@globallogic.com",
              Text = "Shailesh.Pandey@globallogic.com",
              Selected = false
            },
          new SelectListItem
            {
              Value = "notpresent.notpresent@notpresent.com",
              Text = "notpresent.notpresent@notpresent.com",
              Selected = false
            }
        };

      var selectedUser = Request.RequestContext.HttpContext.User.Identity.Name;
      users = users.Select(c => new SelectListItem { Selected = c.Text.Equals(selectedUser), Text = c.Text, Value = c.Value }).ToList();
      ViewBag.Users = users;

      return View();
    }

    [HttpPost]
    public ActionResult Index(string user)
    {
      var isAuthenticatedUser = Request.LogonUserIdentity != null && (Request.IsAuthenticated && Request.RequestContext.HttpContext.User.Identity.Name.Equals(user));
      if (!isAuthenticatedUser)
      {
        FormsAuthentication.SignOut();
        var userIdp = Idp.GetIdp(user);
        HttpContext.User = new GenericPrincipal(new GenericIdentity(user), null);
        if (!string.Equals(userIdp, string.Empty, System.StringComparison.InvariantCultureIgnoreCase))
        {
          return Redirect(userIdp);
        }
      }

      return RedirectToAction("About");
    }

    [CebAuthorizeAttribute]
    public ActionResult About()
    {
      ViewBag.Message = "Your quintessential app description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your quintessential contact page.";

      return View();
    }
  }
}

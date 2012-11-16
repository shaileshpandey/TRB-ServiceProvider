using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using Gibraltar.Agent;

namespace TRB_ServiceProvider
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);

      BundleTable.Bundles.RegisterTemplateBundles();
      Log.MessageAlert += Log_MessageAlert;
    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
      // Skip request for .js .css .image files and Session related request
      if (!Request.Path.Contains(".") && !Request.Path.Contains("Session"))
      {
        if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
        {
          string cookiestr;
          cookiestr = Request.Cookies[FormsAuthentication.FormsCookieName].Value;
          FormsAuthenticationTicket tkt = FormsAuthentication.Decrypt(cookiestr);
          HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(tkt.Name), null);
        }
      }
    }

    private void Log_MessageAlert(object sender, LogMessageAlertEventArgs e)
    {
      if (e.TopSeverity <= LogMessageSeverity.Error)
      {
        e.SendSession = true;
        e.MinimumDelay = new TimeSpan(0, 0, 1);
      }
    }

  }
}
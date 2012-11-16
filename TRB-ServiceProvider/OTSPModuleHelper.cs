using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using opentoken;
using System.Web.Security;
using Com.PingIdentity.PingFederate.SampleApp.SampleAppUtility;
using System.Security.Principal;
using opentoken.util;
using System.Web.Routing;

namespace TRB_ServiceProvider
{
  public partial class OTSPHttpModule
  {
    /// <summary>
    /// Handles the SPSSO request.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    /// <param name="session">The session.</param>
    private void HandleSPSSORequest(HttpContext context, HttpRequest request, HttpResponse response, HttpSessionState session)
    {
      MultiStringDictionary userInfo = GetOpenToken(context);
      if (userInfo != null)
      {
        if (request.Cookies[FormsAuthentication.FormsCookieName] == null)
        {
          var username = userInfo[Constants.SUBJECT][0] as string;
          FormsAuthentication.SetAuthCookie(username, false);
          HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(username), null);
        }
      }
    }

    /// <summary>
    /// Gets the open token.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    private MultiStringDictionary GetOpenToken(HttpContext context)
    {
      HttpRequest request = context.Request;
      HttpResponse response = context.Response;
      HttpSessionState session = context.Session;
      MultiStringDictionary attributes = null;
      var propsPath = context.Request.PhysicalApplicationPath + "/" + Constants.PFAGENT_PROPERTIES;

      Agent agent = new Agent(propsPath);
      try
      {
        attributes = agent.ReadTokenMultiStringDictionary(request);
      }
      catch (Exception e)
      {
        if (e is TokenException || e is TokenExpiredException)
        {
          agent.DeleteToken(response);
          response.RedirectToRoutePermanent(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
        }
      }
      return attributes;
    }
  }
}
using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Com.PingIdentity.PingFederate.SampleApp.SampleAppUtility;
using opentoken;
using opentoken.util;

namespace com.pingidentity.adapters.sampleapp.idp
{
  /// <summary>
  /// Summary description for OTIdPHttpModule
  /// </summary>
  public class OTIdPHttpModule : IHttpModule
  {
    /// <summary>
    /// Inits the specified application.
    /// </summary>
    /// <param name="application">The application.</param>
    public void Init(HttpApplication application)
    {
      application.PostAcquireRequestState += Application_PostAcquireRequestState;
      application.BeginRequest += (Application_BeginRequest);
      application.EndRequest += (Application_EndRequest);
    }

    /// <summary>
    /// Handles the PostAcquireRequestState event of the Application control.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Application_PostAcquireRequestState(object source, EventArgs e)
    {
      var app = (HttpApplication)source;
      var context = app.Context;
      var request = context.Request;
      var response = context.Response;
      var session = HttpContext.Current.Session;
      if (!request.Path.Contains(".") && !request.Path.Contains("Session"))
      {
        HandleIdPSSORequest(context, request, response, session);
      }
    }



    /// <summary>
    /// Handles the BeginRequest event of the Application control.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Application_BeginRequest(Object source, EventArgs e)
    {
    }

    private void Application_EndRequest(Object source, EventArgs e)
    {
    }

    /// <summary>
    /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
    /// </summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// Handles the id PSSO request.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    /// <param name="session">The session.</param>
    private void HandleIdPSSORequest(HttpContext context, HttpRequest request, HttpResponse response, HttpSessionState session)
    {
      if (request.Cookies[FormsAuthentication.FormsCookieName] == null) return;
      var cookiestr = request.Cookies[FormsAuthentication.FormsCookieName].Value;
      var tkt = FormsAuthentication.Decrypt(cookiestr);
      if (tkt == null) return;
      var attributes = new MultiStringDictionary
        {
          {Constants.SUBJECT, tkt.Name},
          {"NickName", "defaultNickName"},
          {"Role", "Admin"}
        };
      if (request[Constants.RESUME_PATH] == null) return;
      var attributesToSend = new MultiStringDictionary();
      foreach (var pair in attributes)
      {
        var key = pair.Key;

        foreach (string value in pair.Value)
        {
          attributesToSend.Add(key, value);
        }
      }

      var strRedirect = "https://" + ConfigurationManager.AppSettings["PFHost"] + request[Constants.RESUME_PATH];
      strRedirect = SetOpenToken(context, strRedirect, attributesToSend);
      response.Redirect(strRedirect, true);
    }

    /// <summary>
    /// Sets the open token.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="url">The URL.</param>
    /// <param name="userInfo">The user info.</param>
    /// <returns></returns>
    private String SetOpenToken(HttpContext context, String url, MultiStringDictionary userInfo)
    {
      var response = context.Response;
      var propsPath = context.Request.PhysicalApplicationPath + @"\" + Constants.PFAGENT_PROPERTIES;
      var agent = new Agent(propsPath);
      var urlHelper = new UrlHelper(url);
      agent.WriteToken(userInfo, response, urlHelper, false);
      return urlHelper.ToString();
    }
  }
}
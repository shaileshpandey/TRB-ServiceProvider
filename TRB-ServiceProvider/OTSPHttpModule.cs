using System;
using System.Web;

namespace TRB_ServiceProvider
{

  /// <summary>
  /// Summary description for OTSPHttpModule
  /// </summary>
  public partial class OTSPHttpModule : IHttpModule
  {

    /// <summary>
    /// Inits the specified application.
    /// </summary>
    /// <param name="application">The application.</param>
    public void Init(HttpApplication application)
    {
      application.PostAcquireRequestState += Application_PostAcquireRequestState;
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
      if (request.IsAuthenticated) return;
      var response = context.Response;
      var session = HttpContext.Current.Session;
      if (!request.Path.Contains(".") && !request.Path.Contains("Session") && !request.Path.Contains("css") && !request.Path.Contains("js"))
      {
        HandleSPSSORequest(context, request, response, session);
      }
    }

    public void Dispose()
    {
    }
  }
}
namespace TRB_ServiceProvider.Attributes
{
  using System.Collections.Generic;
  using System.Web.Mvc;

  /// <summary>
  ///  Used to indicate the module, the permission on which is required for this action to execute
  /// </summary>
  public class CebAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
  {
    /// <summary>
    /// Called when [authorization].
    /// </summary>
    /// <param name="filterContext">The filter context.</param>
    public void OnAuthorization(AuthorizationContext filterContext)
    {

      if (filterContext.HttpContext.Request.IsAuthenticated)
      {
      }
      else
      {
        filterContext.Result = new RedirectResult(string.Concat("~/", "Account", "/", "Login"));
      }
    }
  }
}

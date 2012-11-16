using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace TRB_ServiceProvider
{
  [Serializable]
  public class Idp
  {
    /// <summary>
    /// Gets or sets the domain.
    /// </summary>
    /// <value>
    /// The domain.
    /// </value>
    [XmlElementAttribute]
    public string Domain { get; set; }

    /// <summary>
    /// Gets or sets the ping URL.
    /// </summary>
    /// <value>
    /// The ping URL.
    /// </value>
    [XmlElementAttribute]
    public string PingUrl { get; set; }

    /// <summary>
    /// Gets the idp.
    /// </summary>
    /// <param name="emailAddress">The email address.</param>
    /// <returns></returns>
    public static string GetIdp(string emailAddress)
    {
      var availableIdps = GetIdps();
      var emailParts = emailAddress.Split('@');
      return emailParts.Length > 1 ? availableIdps.Single(c => c.Text.Equals(emailParts[1], StringComparison.CurrentCultureIgnoreCase)).Value : "";
    }

    /// <summary>
    /// Gets the idps.
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<SelectListItem> GetIdps()
    {
      return DeserializeObject().Select(c => new SelectListItem { Text = c.Domain, Value = c.PingUrl }).ToList();
    }

    /// <summary>
    /// Deserializes the object.
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Idp> DeserializeObject()
    {
      var xmlPath = HttpContext.Current.Request.PhysicalApplicationPath + @"\" + "IDPList.xml";
      var xs = new XmlSerializer(typeof(List<Idp>));
      var reader = new StreamReader(xmlPath);
      return xs.Deserialize(reader) as List<Idp>;
    }
  }
}
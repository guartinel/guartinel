using System;
using System.Diagnostics;
using System.Web;

namespace Guartinel.Website.User.Misc {
   public class Utils {
      public static string GetGuartinelLicensePageAddress () {
      if(Debugger.IsAttached ) { return "https://localhost:8080/"; }
         string result = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)+ "/license"; //"https://" + HttpContext.Current.Request.Url.Host + "/license";
               
        return result;
       }

   }
}
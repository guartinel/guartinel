﻿using System.Web.Mvc ;

namespace Guartinel.Website.User {
   public class FilterConfig {
      public static void RegisterGlobalFilters (GlobalFilterCollection filters) {
         filters.Add (new HandleErrorAttribute()) ;
      }
   }
}

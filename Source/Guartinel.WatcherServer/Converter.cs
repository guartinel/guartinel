using System;
using System.Globalization ;
using System.Linq;
using System.Text;

namespace Guartinel.WatcherServer {
   public static class Converter {
      private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss" ;

      public static double? Normalize (this double? value) {
         if (value == null) return null ;

         double? result = value ;
         
         // Cut digits
         result = Math.Round (result.Value, ApplicationSettings.Use.Decimals) ;

         return result ;
      }

      public static string DateTimeToString (DateTime dateTime) {
         return dateTime.ToString() ;
      }

      public static DateTime StringToDateTime (string dateTime) {
         DateTime dateResult ;
         if (!DateTime.TryParseExact (dateTime, DATE_TIME_FORMAT,
                                      null, DateTimeStyles.AssumeLocal,
                                      out dateResult)) {
            dateResult = DateTime.MinValue ;
         }         
         return dateResult ;
      }
   }
}
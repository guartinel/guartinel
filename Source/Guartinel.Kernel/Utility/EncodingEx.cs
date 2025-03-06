using System;
using System.Collections.Generic ;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel.Utility {
   public static class EncodingEx {
      public static Encoding GetEncoding (string name) {
         if (string.IsNullOrEmpty (name)) throw new ArgumentNullException(nameof(name)) ;

         // if (name.ToLowerInvariant() == "utf-8") return new UTF8Encoding() ;

         // Truncate the quotes
         if (name.IsWrappedInDoubleQuotes ()) {
            name = name.Replace (@"""", "") ;
         }

         return Encoding.GetEncoding (name) ;
      }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp ;

namespace Guartinel.CLI.Utility {
   public static class CommandLineParserEx {
      public static FluentCommandLineParser Create() {
         var parser = new FluentCommandLineParser();
         parser.IsCaseSensitive = false ;
         return parser ;
      }
   }
}

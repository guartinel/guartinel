using System ;
using System.Text ;

namespace Guartinel.ManagementServer.Tests.Helpers {
   internal class Helper {
      public static string getTestName() {
         return "TEST" + new Random().Next (10000) ;
      }

      public static string getRandomString() {
         return Guid.NewGuid().ToString() ;
      }
   }
}

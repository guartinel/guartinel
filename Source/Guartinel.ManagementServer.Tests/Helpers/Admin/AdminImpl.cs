using System ;
using System.Text ;

namespace Guartinel.ManagementServer.Tests.Helpers.Admin {
   internal class AdminImpl {
      public string UserName {get ; set ;}
      public string Password {get ; set ;}

      public string Token {get ; set ;}

      public WatcherServerImpl WatcherServer {get ; set ;}
   }
}

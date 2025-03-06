using System ;
using System.Text ;
using Guartinel.ManagementServer.Tests.Helpers.Device ;
using Guartinel.ManagementServer.Tests.Helpers.Package ;

namespace Guartinel.ManagementServer.Tests.Helpers.Account {
   internal class AccountImpl {
      public string Id {get ; set ;}
      public string Email {get ; set ;}
      public string Token {get ; set ;}
      public string Password {get ; set ;}
      public string NewPassword {get ; set ;}
      public string FirstMame {get ; set ;}
      public string LastName {get ; set ;}
      public string ActivationCode {get ; set ;}

      public string IsActivated {get ; set ;}

      public DeviceImpl Agent {get ; set ;}
      public DeviceImpl AlertDevice {get ; set ;}

      public PackageImpl Package {get ; set ;}
   }
}

using System ;
using System.Runtime.Serialization ;
using Guartinel.Kernel.Utility ;
using Guartinel.Website.Common.Configuration ;
using Guartinel.Website.Common.Configuration.Data ;

namespace Guartinel.Website.User.Persistance {
   [DataContract]
   public class GuartinelUserWebSiteSettings : ISettings {
      public void ResetDefaultValues() {
         string doubleHashedPassword = Hashing.GenerateHash (Hashing.GenerateHash (Properties.Config.Default.DEFAULT_ADMIN_PASSWORD, Properties.Config.Default.DEFAULT_ADMIN_USERNAME), Properties.Config.Default.DEFAULT_ADMIN_USERNAME) ;

         AdminAccount = new AdminAccount() ;
         AdminAccount.Username = Properties.Config.Default.DEFAULT_ADMIN_USERNAME ;
         AdminAccount.PasswordHash = doubleHashedPassword ;
         AdminAccount.CreationTimeStamp = DateTime.UtcNow ;
      }

      [DataMember]
      public Guartinel.Website.Common.Configuration.Data.ManagementServer ManagementServer {get ; set ;}

      [DataMember]
      public Guartinel.Website.Common.Configuration.Data.AdminAccount AdminAccount {get ; set ;}
   }
}

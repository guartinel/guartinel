namespace Guartinel.Website.Common.Configuration {
   public interface ISettings {
      void ResetDefaultValues() ;
      Guartinel.Website.Common.Configuration.Data.ManagementServer ManagementServer {get ; set ;}
      Guartinel.Website.Common.Configuration.Data.AdminAccount AdminAccount {get ; set ;}
   }
}

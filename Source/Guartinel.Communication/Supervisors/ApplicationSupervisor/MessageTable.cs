using System.Collections.Generic ;

namespace Guartinel.Communication.Plugins.ApplicationSupervisor {
   public interface IMessages {
      string InstanceNotAvailableAlert {get ;}
      string ApplicationMeasurementAlert {get ;}
      string PackageNotAvailableAlert {get ;}
   }

   public class Messages : IMessages {
      public string InstanceNotAvailableAlert => nameof (InstanceNotAvailableAlert) ;
      public string ApplicationMeasurementAlert => nameof (ApplicationMeasurementAlert) ;
      public string PackageNotAvailableAlert => nameof (PackageNotAvailableAlert) ;
   }

   public class Parameters {
      public string InstanceName => nameof (InstanceName) ;
      public string Message => nameof (Message) ;
   }

   public class MessageTable : MessageTableBase {
      protected MessageTable() {
         Add (new Languages.English()) ;
      }

      public static MessageTable Use {get ;} = new MessageTable();
   }
}
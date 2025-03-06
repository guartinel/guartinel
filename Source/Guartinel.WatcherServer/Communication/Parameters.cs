using System;
using System.Collections.Generic ;
using System.Linq;
using System.Text;
using Guartinel.Communication ;
using Guartinel.Kernel.Configuration ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.WatcherServer.Communication {
   public class Parameters : ConfigurationData {
      public Parameters (string data) : base (data) {}
      public Parameters (JObject data) : base (data) {}
      public Parameters() {}

      public void Success() {
         Data.Remove (WatcherServerAPI.GeneralResponse.Names.ERROR) ;
         Data.Remove (WatcherServerAPI.GeneralResponse.Names.ERROR_PARAMETERS) ;
         Data.Remove (WatcherServerAPI.GeneralResponse.Names.ERROR_UUID) ;
         
         Data [WatcherServerAPI.GeneralResponse.Names.SUCCESS] = WatcherServerAPI.GeneralResponse.SuccessValues.SUCCESS ;
      }

      public void Error (string error,
                         IEnumerable<string> parameters) {
         Data.Remove (WatcherServerAPI.GeneralResponse.Names.SUCCESS) ;
         Data.Remove (WatcherServerAPI.GeneralResponse.Names.ERROR_PARAMETERS) ;

         Data [WatcherServerAPI.GeneralResponse.Names.ERROR] = error ;
         Data [WatcherServerAPI.GeneralResponse.Names.ERROR_UUID] = Guid.NewGuid().ToString() ;
         
         if (parameters != null) {
            Data.SetStringArray (WatcherServerAPI.GeneralResponse.Names.ERROR_PARAMETERS, parameters.ToArray()) ;
         }
      }

      public bool IsSuccess() {
         if (!Data.Exists (WatcherServerAPI.GeneralResponse.Names.SUCCESS)) return false ;
         return Data [WatcherServerAPI.GeneralResponse.Names.SUCCESS] == WatcherServerAPI.GeneralResponse.SuccessValues.SUCCESS ;
      }

      public bool IsError() {
         if (!Data.Exists(WatcherServerAPI.GeneralResponse.Names.SUCCESS)) return true ;
         return Data [WatcherServerAPI.GeneralResponse.Names.SUCCESS] != WatcherServerAPI.GeneralResponse.SuccessValues.SUCCESS ;
      }
   }
}

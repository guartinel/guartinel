using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel {
   public interface IApplicationSettingsReader {
      JObject ReadConfigurationObject() ;
      JObject GetConfigurationInfo ();

      void SubscribeForChange (int refreshIntervalSeconds,
                               Action notification) ;
   }
}
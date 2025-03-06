using System ;
using System.Linq ;
using System.Text ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.WatcherServer.Communication.ManagementServer {
   public interface IMeasuredDataStore {
      void StoreMeasuredData (string packageID,
                             string measurementType,
                             DateTime measurementDateTime,
                             JObject measurement) ;
   }
}
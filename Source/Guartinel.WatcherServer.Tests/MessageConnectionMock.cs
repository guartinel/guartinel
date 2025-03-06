using System ;
using System.Collections.Concurrent ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using Guartinel.Kernel ;
using Guartinel.Service.MessageQueues ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Instances ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests {
   public class MessageConnectionMock : IMessageConnection {
      public BroadcastSender CreateBroadcastSender (string name,
                                                    string[] tags) {
         throw new NotImplementedException() ;
      }

      public BroadcastConsumer CreateBroadcastConsumer (string name,
                                                        Action<JObject> messageHandler,
                                                        string[] tags) {
         throw new NotImplementedException() ;
      }

      public ServiceClient CreateServiceClient (string name,
                                                string[] tags) {
         throw new NotImplementedException() ;
      }

      public void CallServiceClient (string name,
                                     JObject message,
                                     Action<JObject> received,
                                     CancellationToken cancellation,
                                     string[] tags) {
         throw new NotImplementedException() ;
      }

      public ServiceServer CreateServiceServer (string name,
                                                Func<JObject, JObject> messageHandler,
                                                string[] tags) {
         throw new NotImplementedException() ;
      }
   }
}

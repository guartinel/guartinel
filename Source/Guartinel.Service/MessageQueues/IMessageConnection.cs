using System;
using System.Text ;
using System.Threading ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.MessageQueues {
   public interface IMessageConnection {
      BroadcastSender CreateBroadcastSender (string name,
                                             string[] tags) ;

      BroadcastConsumer CreateBroadcastConsumer (string name,
                                                 Action<JObject> messageHandler,
                                                 string[] tags) ;
      ServiceClient CreateServiceClient (string name,
                                         string[] tags) ;

      void CallServiceClient (string name,
                              JObject message,
                              Action<JObject> received,
                              CancellationToken cancellation,
                              string[] tags) ;

      ServiceServer CreateServiceServer (string name,
                                         Func<JObject, JObject> messageHandler,
                                         string[] tags) ;
   }
}
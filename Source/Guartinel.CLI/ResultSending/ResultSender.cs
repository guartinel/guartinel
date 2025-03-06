using System ;
using System.Reflection ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.ResultSending {
   public class ResultSender : IResultSender {

      private static readonly PostSender _postSender = new PostSender() ;

      public static class Constants {
         public static class Send {
            public const string HEADER_TYPE = "application/json" ;

            public const string APPLICATION_TOKEN = "application_token" ;
            public const string INSTANCE_ID = "instance_id" ;
            public const string INSTANCE_NAME = "instance_name" ;
            public const string IS_HEARTBEAT = "is_heartbeat" ;
            public const string CHECK_RESULT = "measurement";
            public const string VERSION = "version";

         }

         public static class Receive {
            public const string SUCCESS = "success" ;
            public const string SUCCESS_VALUE = "SUCCESS" ;
            public const string ERROR = "error" ;
         }
      }

      public void SendSuccess (string address,
                               string token,
                               string instanceID,
                               string instanceName,
                               bool isHeartbeat) {
         SendSuccess (address,
                      token,
                      instanceID,
                      instanceName,
                      isHeartbeat,
                      null) ;
      }

      public void SendSuccess (string address,
                               string token,
                               string instanceID,
                               string instanceName,
                               bool isHeartbeat,
                               JObject data) {
         SendResult (address,
                     token,
                     instanceID,
                     instanceName,
                     isHeartbeat,
                     new CheckResult (true, string.Empty, string.Empty, string.Empty, data)) ;
      }

      public void SendError (string address,
                             string token,
                             string instanceID,
                             string instanceName,
                             bool isHeartbeat,
                             string message,
                             string details,
                             string extract,
                             JObject data) {
         SendResult (address,
                     token,
                     instanceID,
                     instanceName,
                     isHeartbeat,
                     new CheckResult (false, message, details, extract, data)) ;
      }

      public void SendResult (string address,
                              string token,
                              string instanceID,
                              string instanceName,
                              bool isHeartbeat,
                              CheckResult checkResult) {
         try {
            JObject values = new JObject() ;
            values [Constants.Send.APPLICATION_TOKEN] = token ;
            values [Constants.Send.INSTANCE_ID] = instanceID ;
            values [Constants.Send.INSTANCE_NAME] = instanceName ;
            values [Constants.Send.IS_HEARTBEAT] = isHeartbeat ;
            values [Constants.Send.CHECK_RESULT] = checkResult.AsJObject() ;
            values [Constants.Send.VERSION] = Assembly.GetExecutingAssembly().GetName().Version.ToString() ;

            Logger.Log ($"Send post to Management Server on {address}; request: {values}") ;
               
            var result = _postSender.Post (address, values) ;

            Logger.Log ($"Post returned from Management Server: {result}") ;
         } catch (Exception e) {
            // Log exception before throw
            Logger.Log (LogLevel.Error, $"Error when posting request to Management Server. Message: {e.GetAllMessages()}") ;

            throw ;
         }
      }
   }
}
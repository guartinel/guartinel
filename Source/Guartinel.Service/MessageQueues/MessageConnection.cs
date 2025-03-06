using System;
using System.Text ;
using System.Threading ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;
using RabbitMQ.Client;

namespace Guartinel.Service.MessageQueues {
   public class MessageConnection : IMessageConnection, IDisposable {
      //private static readonly Lazy<MessageConnection> _useInstance = new Lazy<MessageConnection> (() => {
      //   var connection = new MessageConnection() ;
      //   return connection ;
      //}) ;


      // public MessageConnection() : this (String.Empty, String.Empty, String.Empty) { }

      public static int TestChannelCount = 0 ;
      public static object TestChannelCountLock = new object() ;

      public MessageConnection (string serviceUri = "",
                                string userName = "",
                                string password = "") {

         // const string DEFAULT_USER_NAME = "guest" ;
         // const string DEFAULT_PASSWORD = "guest" ;

         try {
            _serviceUri = string.IsNullOrEmpty (serviceUri) ? "queue.service.guartinel.com:5672/" : serviceUri.EnsureTrailingSlash() ;

            // _userName = string.IsNullOrEmpty (userName) ? DEFAULT_USER_NAME : userName ;
            // var password1 = string.IsNullOrEmpty (password) ? DEFAULT_PASSWORD : password ;

            Logger.Info ($"Creating message queue service connection on '{_serviceUri}' with user '{userName}'...") ;

            // Setup connection
            var factory = new ConnectionFactory() ;
            var uri = new Uri (_serviceUri) ;
            factory.AutomaticRecoveryEnabled = true ;
            factory.HostName = uri.Host ;
            factory.Port = uri.Port ;
            factory.UserName = userName ;
            factory.Password = password ;

            _connection = factory.CreateConnection() ;
         } catch (Exception e) {
            Logger.Error ($"Cannot connect to queue service on {_serviceUri}. Details: {e.GetAllMessages()}") ;
            throw new Exception ($"Cannot connect to queue service on {_serviceUri}. Message: {e.Message}") ;
         }
      }

      // public static MessageConnection Use => _useInstance.Value ;

      private readonly IConnection _connection = null ;

      private readonly string _serviceUri ;
      // private readonly string _userName ;

      public override string ToString() {
         return $"RabbitMQ on '{_serviceUri}'" ;
      }

      public void Dispose() {
         _connection?.Close() ;
         _connection?.Dispose() ;
      }

      public IModel CreateChannel (string[] tags) {
         var logger = new TagLogger (tags) ;
         try {
            var result = _connection.CreateModel() ;
            lock (TestChannelCountLock) {
               TestChannelCount++;
            }

            logger.Debug ($"Channel created. Channel count: {TestChannelCount}") ;
            return result ;
         } catch (Exception e) {
            logger.ErrorWithDetails ($"Error when creating mesage connection. Message: {e.Message}.", e.GetAllMessages()) ;
            throw ;
         }
      }

      public static void CloseChannel (IModel channel,
                                       string[] tags) {
         if (channel == null) return ;
         var logger = new TagLogger (tags) ;

         lock (TestChannelCountLock) {
            TestChannelCount-- ;
         }

         logger.Debug ($"Channel closed. Channel count: {TestChannelCount}") ;
         channel.Close() ;
      }

      public BroadcastSender CreateBroadcastSender (string name,
                                                    string[] tags) {
         var logger = new TagLogger(tags, TagLogger.CreateTag(nameof(BroadcastSender), name));
         logger.Debug ($"Creating broadcast sender {name}...") ;
         return new BroadcastSender (CreateChannel (tags), name,
                                     logger.Tags) ;
      }

      public BroadcastConsumer CreateBroadcastConsumer (string name,
                                                        Action<JObject> messageHandler,
                                                        string[] tags) {
         var logger = new TagLogger(tags, TagLogger.CreateTag(nameof(BroadcastConsumer), name));
         logger.Debug ($"Creating broadcast consumer {name}...") ;
         return new BroadcastConsumer (CreateChannel (tags), name, messageHandler,
                                       logger.Tags) ;
      }

      public ServiceClient CreateServiceClient (string name,
                                                string[] tags) {
         var logger = new TagLogger(tags, TagLogger.CreateTag(nameof(ServiceClient), name));
         logger.Info ($"Creating service client {name}...") ;
         return new ServiceClient (CreateChannel (tags),
                                   CreateChannel (tags),
                                   name,
                                   logger.Tags) ;
      }

      public void CallServiceClient (string name,
                                     JObject message,
                                     Action<JObject> received,
                                     CancellationToken cancellation,
                                     string[] tags) {

         var logger = new TagLogger (tags, TagLogger.CreateTag (nameof(ServiceClientOneCall), name)) ;
         logger.Info ($"Creating one-call service client {name}...") ;
         ServiceClientOneCall.Call (name,
                                    CreateChannel (tags),
                                    CreateChannel (tags),
                                    message,
                                    received,
                                    cancellation,
                                    logger.Tags) ;
      }

      public ServiceServer CreateServiceServer (string name,
                                                Func<JObject, JObject> messageHandler,
                                                string[] tags) {
         var logger = new TagLogger (tags, TagLogger.CreateTag (nameof (ServiceServer), name)) ;
         logger.Info ($"Creating service server {name}...") ;
         return new ServiceServer (name, 
                                   CreateChannel(tags),
                                   CreateChannel(tags),
                                   messageHandler,
                                   logger.Tags) ;
      }
   }
}
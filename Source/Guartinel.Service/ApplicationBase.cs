using System;
using System.Text;
using System.Threading ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Service.MessageQueues ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service {
   public abstract class ApplicationBase : IDisposable {
      protected ApplicationBase () {
         _messageConnection = new Lazy<MessageConnection> (() => new MessageConnection (QueueServiceAddress, QueueServiceUserName, QueueServicePassword),
                                                           LazyThreadSafetyMode.PublicationOnly) ;
      }
      protected readonly Lazy<MessageConnection> _messageConnection ;
      protected ServiceServer _server ;

      protected virtual string QueueServiceAddress => "" ;

      protected virtual string QueueServiceUserName => "";

      protected virtual string QueueServicePassword => "";

      protected abstract string Name {get ;}

      protected virtual string Version => "1.0" ;

      protected string ServiceName => $"{Name}.{Version}" ;

      protected virtual void RegisterLoggers() {
         Logger.RegisterLogger<SimpleConsoleLogger>() ;
      }

      protected abstract JObject ProcessRequest1 (JObject request) ;

      public void Run() {
         var logger = new TagLogger (TagLogger.CreateTag ("service", ServiceName)) ;

         RegisterLoggers() ;
         IoC.Use.Single.Register<IMessageConnection> (() => _messageConnection.Value) ;

         logger.Info ("Starting service...") ;
         bool initialized = false ;
         do {
            try {
               if (string.IsNullOrEmpty (QueueServiceAddress) ||
                   string.IsNullOrEmpty (QueueServiceUserName) ||
                   string.IsNullOrEmpty (QueueServicePassword)) {

                  new TimeoutSeconds (5).Wait() ;
                  continue ;
               }

               logger.Info ("Creating connection...") ;
               var connection = IoC.Use.Single.GetInstance<IMessageConnection>() ;
               logger.Info ("Connection created, creating server...") ;
               _server = connection.CreateServiceServer (ServiceName, ProcessRequest, new[] {ServiceName}) ;
               logger.Info("Service initialized.");
               initialized = true ;
            } catch (Exception e) {
               // Log and try again
               logger.Error ($"Cannot initialize service. Message: {e.Message.EnsurePeriod()} Details: {e.GetAllMessages()}") ;
               initialized = false ;
               new TimeoutSeconds (2).Wait() ;
            }
         } while (!initialized) ;

         logger.Info ("Service started.") ;
      }

      //protected readonly ObjectPool<object> _services = new ObjectPool<object>(() => new object(), 25);
      //public int PoolSize { get => _services.MaxObjectCount; set => _services.MaxObjectCount = value; }

      private JObject ProcessRequest (JObject request) {
         return ProcessRequest1 (request) ;
      }

      public void Dispose() {
         _server.Dispose();
      }
   }
}
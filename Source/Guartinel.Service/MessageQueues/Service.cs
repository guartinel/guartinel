using System;
using System.Collections.Concurrent ;
using System.Text ;
using System.Threading ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Guartinel.Service.MessageQueues {
   public abstract class ServiceBase : TwoWayMessagingBase {
      protected ServiceBase (string name,
                             IModel sendChannel,
                             IModel receiveChannel,
                             string[] tags) : base (name, sendChannel, receiveChannel, tags) { }

      public string QueueName => $"{_name}_rpc" ;

      protected void DeclareCallingQueue (IModel channel) {
         channel.QueueDeclare (queue: QueueName,
                               durable: true,
                               exclusive: false,
                               autoDelete: false) ;
         channel.BasicQos (prefetchSize: 0,
                           prefetchCount: 1,
                           global: false) ;
      }
   }

   public class ServiceServer : ServiceBase {
      public ServiceServer (string name,
                            IModel sendChannel,
                            IModel receiveChannel,
                            Func<JObject, JObject> processCall,
                            string[] tags) : base (name, sendChannel, receiveChannel, tags) {
         // Prepare calling queue
         DeclareCallingQueue (_sendChannel) ;
         DeclareCallingQueue (_receiveChannel) ;

         var consumer = new EventingBasicConsumer (_receiveChannel) ;

         consumer.Received += (sender,
                               delivery) => {
            var logger = new TagLogger (tags, delivery.DeliveryTag.ToString()) ;

            ServiceResult serviceResult = new ServiceResult() ;

            var callProperties = delivery.BasicProperties ;
            var replyProperties = _sendChannel.CreateBasicProperties() ;
            replyProperties.CorrelationId = callProperties.CorrelationId ;
            replyProperties.Expiration = Constants.MessageExpiration ;

            try {
               var callString = Encoding.UTF8.GetString (delivery.Body) ;
               var callData = JObject.Parse (callString) ;
               logger.Debug ($"Receiving service call for {_name}. Delivery tag: {delivery.DeliveryTag.ToString()}") ;
               serviceResult = ServiceResult.CreateSuccess (processCall (callData)) ;
            } catch (Exception e) {
               // @todo: add error to response
               logger.DebugWithDetails ($"Error in service call for {_name}. Message: {e.Message}", e.GetAllMessages()) ;
               serviceResult = ServiceResult.CreateError ($"Error in service call for {_name}.", e.Message) ;
            } finally {
               var responseBytes = Encoding.UTF8.GetBytes (serviceResult.AsJObject().ToString (Formatting.Indented)) ;
               logger.Debug ($"Sending back service call result for {_name} to {callProperties.ReplyTo}.") ;

               _sendChannel.BasicPublish (exchange: string.Empty,
                                          routingKey: callProperties.ReplyTo,
                                          basicProperties: replyProperties, body: responseBytes) ;

               // Ack only if finished!
               _receiveChannel.BasicAck (deliveryTag: delivery.DeliveryTag,
                                         multiple: false) ;
            }
         } ;

         _receiveChannel.BasicConsume (queue: QueueName,
                                       autoAck: false,
                                       consumer: consumer) ;
      }
   }

   public abstract class ServiceClientBase : ServiceBase {
      protected class ServiceCallback {
         public ServiceCallback (string name,
                                 string correlationID,
                                 Action<JObject> callback,
                                 string[] tags) {
            _logger = new TagLogger (tags) ;

            Name = name ;
            CorrelationID = correlationID ;
            Callback = callback ;
         }

         private readonly TagLogger _logger ;

         public string Name {get ;}
         public string CorrelationID {get ;}
         public Action<JObject> Callback {get ;}

         protected void ReplyReceived (BasicDeliverEventArgs delivery,
                                       Action<JObject> received) {
            var replyBody = delivery.Body ;
            var reply = Encoding.UTF8.GetString (replyBody) ;
            if (delivery.BasicProperties.CorrelationId == CorrelationID) {
               ServiceResult serviceResult = new ServiceResult (JObject.Parse (reply)) ;

               _logger.DebugWithDetails ($"Received service call result for {Name}.", serviceResult.ToString()) ;

               if (!serviceResult.Success) {
                  _logger.InfoWithDebug ($"Error in call. Message: {serviceResult.Message.EnsurePeriod()}",
                                         $"Details: {serviceResult.Details.EnsurePeriod()}") ;

                  throw new Exception ($"Error in call {Name}. Message: {serviceResult.Message.EnsurePeriod()}") ;
               }

               Callback?.Invoke (serviceResult.Result) ;
            } else {
               _logger.Debug ($"Received service call result for {Name}, but the correlation ID was mismatched.") ;
            }
         }

         public void Invoke (BasicDeliverEventArgs delivery,
                             Action<JObject> received) {
            ReplyReceived (delivery, received) ;
         }
      }

      internal ServiceClientBase (string name,
                                  IModel sendChannel,
                                  IModel receiveChannel,
                                  string[] tags) : base (name, sendChannel, receiveChannel, tags) { }

      protected IBasicProperties CreateMessageProperties (string replyQueueName,
                                                          string correlationID) {
         IBasicProperties result = _sendChannel.CreateBasicProperties() ;
         result.CorrelationId = correlationID ;
         // Expiration: 10 minutes
         result.Expiration = Constants.MessageExpiration ;
         result.ReplyTo = replyQueueName ;

         return result ;
      }

      protected void Call (JObject message,
                           Action<JObject> received,
                           CancellationToken cancellation,
                           string[] tags) {

         var logger = new TagLogger (_logger.Tags, tags) ;

         if (message == null) message = new JObject() ;

         // Prepare reply queue
         // var replyQueue = _channel.QueueDeclare (durable: false, exclusive: true, autoDelete: true) ;
         var replyQueue = _receiveChannel.QueueDeclare() ;
         var replyQueueName = replyQueue.QueueName ;
         var replyConsumer = new EventingBasicConsumer (_receiveChannel) ;
         var correlationID = Guid.NewGuid().ToString() ;

         var callback = new ServiceCallback (_name, correlationID, result => {
            received?.Invoke (result) ;
         }, logger.Tags) ;

         replyConsumer.Received += (sender,
                                    delivery) => {
            try {
               callback.Invoke (delivery, received) ;
            } finally {
               logger.Debug ($"Cancel consumer {replyConsumer.ConsumerTag} on queue {replyQueueName}.") ;
               // Cancel consumer            
               _receiveChannel.BasicCancel (replyConsumer.ConsumerTag) ;
            }
         } ;

         var bodyString = message.ToString (Formatting.Indented) ;
         var body = Encoding.UTF8.GetBytes (bodyString) ;

         _receiveChannel.BasicConsume (consumer: replyConsumer,
                                       queue: replyQueueName,
                                       autoAck: true) ;

         _sendChannel.BasicPublish (string.Empty,
                                    routingKey: QueueName,
                                    basicProperties: CreateMessageProperties (replyQueueName, correlationID),
                                    body: body) ;
         logger.DebugWithDetails ($"Sent service call for {_name}. Reply queue: {replyQueueName}", message.ConvertToLog()) ;
      }
   }

   public class ServiceClient : ServiceClientBase {
      public ServiceClient (IModel sendChannel,
                            IModel receiveChannel,
                            string name,
                            string[] tags) : base (name, sendChannel, receiveChannel, tags) { }

      protected BlockingCollection<JObject> _replyQueue = new BlockingCollection<JObject>() ;

      public JObject CallSync (JObject message,
                               CancellationToken cancellation,
                               string[] tags) {

         Call (message, receivedResult => {
            _replyQueue.Add (receivedResult, cancellation) ;
         }, CancellationToken.None, tags) ;

         var result = _replyQueue.Take (cancellation) ;
         return result ;
      }

      public void CallAsync (JObject message,
                             Action<JObject> received,
                             CancellationToken cancellation,
                             string[] tags = null) {

         Call (message, result => {
            received?.Invoke (result) ;
         }, cancellation, tags) ;
      }
   }

   public class ServiceClientOneCall : ServiceClientBase {
      protected ServiceClientOneCall (string name,
                                      IModel sendChannel,
                                      IModel receiveChannel,
                                      string[] tags) : base (name, sendChannel, receiveChannel, tags) { }

      protected void Call (JObject message,
                           Action<JObject> received,
                           CancellationToken cancellation
      ) {
         try {
            cancellation.Register (() => {
               _receiveChannel.Close() ;
            }) ;

            Call (message, result => {
               received?.Invoke (result) ;
            }, cancellation, _logger.Tags) ;
         } finally {
            _sendChannel.Close() ;
         }
      }

      public static void Call (string name,
                               IModel sendChannel,
                               IModel receiveChannel,
                               JObject message,
                               Action<JObject> received,
                               CancellationToken cancellation,
                               string[] tags) {
         new ServiceClientOneCall (name, sendChannel, receiveChannel, tags).Call (message, received, cancellation) ;
      }
   }
}
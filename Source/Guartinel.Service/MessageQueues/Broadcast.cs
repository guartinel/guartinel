using System;
using System.Text ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Guartinel.Service.MessageQueues {
   public class BroadcastSender : OneWayMessagingBase {
      internal BroadcastSender (IModel channel,
                                string name,
                                string[] tags) : base (channel, name, tags) {
         _channel.ExchangeDeclare (name, Constants.EXCHANGE_TYPE_FANOUT) ;
      }

      public void Send (JObject message) {
         message.CheckNull (nameof(message)) ;

         var bodyString = message.ToString (Formatting.Indented) ;
         var body = Encoding.UTF8.GetBytes (bodyString) ;
         _logger.DebugWithDetails ($"Sending broadcast message for {_name}.", bodyString) ;
         _channel.BasicPublish (_name, string.Empty, CreateMessageProperties (_channel), body) ;
      }
   }

   public class BroadcastConsumer : OneWayMessagingBase {
      public BroadcastConsumer (IModel channel,
                                string name,
                                Action<JObject> messageHandler,
                                string[] tags) : base (channel, name, tags) {

         messageHandler.CheckNull (nameof(messageHandler)) ;

         _channel.ExchangeDeclare (name, Constants.EXCHANGE_TYPE_FANOUT) ;
         // Create an auto-named, non-durable, exclusive, auto-deleted queue for this particular consumer
         var queueName = _channel.QueueDeclare().QueueName ;
         _channel.QueueBind (queue: queueName,
                             exchange: name,
                             routingKey: string.Empty) ;

         EventingBasicConsumer consumer = new EventingBasicConsumer (_channel) ;
         consumer.Received += (sender,
                               delivery) => {
            var body = delivery.Body ;
            if (body == null) return ;

            var bodyString = Encoding.UTF8.GetString (body) ;
            _logger.DebugWithDetails ($"Receiving broadcast message for {_name}.", bodyString) ;
            JObject message = JObject.Parse (bodyString) ;
            messageHandler.Invoke (message) ;

            // Ack the message
            _channel.BasicAck (delivery.DeliveryTag, false) ;
         } ;

         //      //consumer.ConsumerCancelled += Consumer_ConsumerCancelled;
         //      //consumer.Shutdown += Consumer_Shutdown;
         //      //consumer.Registered += Consumer_Registered;
         //      //consumer.Unregistered += Consumer_Unregistered;

         channel.BasicConsume (queueName, false, consumer) ;
      }
   }
}

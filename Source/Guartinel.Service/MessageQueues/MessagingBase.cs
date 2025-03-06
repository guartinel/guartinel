using System;
using System.Text ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using RabbitMQ.Client;

namespace Guartinel.Service.MessageQueues {
   public abstract class MessagingBase : IDisposable {
      public static class Constants {
         public static string MessageExpiration => (10 * 60 * 1000).ToString() ;
         public const string EXCHANGE_TYPE_FANOUT = "fanout" ;
      }

      protected MessagingBase (string name,
                               string[] tags) {
         _logger = new TagLogger(tags);
         _name = name ;
      }

      protected static IBasicProperties CreateMessageProperties (IModel channel,
                                                                 bool persistent = true) {
         var result = channel.CreateBasicProperties() ;
         result.Persistent = persistent ;
         result.ContentType = " application/json" ;

         return result ;
      }


      // protected readonly IBasicProperties _messageProperties ;
      protected string _name ;
      protected TagLogger _logger ;

      public void Dispose() {
         Close() ;
      }

      public virtual void Close() { }
   }
   //   protected readonly IModel _channel;
   //   private readonly object _channelLock = new object();
   //   protected string _name ;
   //   protected bool _queueDeclared ;
   //   protected bool _deleteQueueAtDispose = false ;

   //   protected MessagingBase (Func<IModel> createChannel,
   //                            string name,
   //                            bool deleteQueueAtDispose = false) {
   //      if (createChannel == null) throw new ArgumentNullException($"{nameof(MessagingBase)}: {nameof(createChannel)}");

   //      _channel = createChannel();
   //      if (_channel == null) throw new Exception("Invalid channel for queue messaging.") ;
   //      _name = name;
   //      _deleteQueueAtDispose = deleteQueueAtDispose;
   //   }

   //   protected void CreateQueue() {
   //      _channel. QueueDeclare(queue: _name, durable: true, exclusive: false, autoDelete: false);
   //      _queueDeclared = true ;
   //   }
   //}

   //internal abstract class MessageQueue : IDisposable {
   //   private readonly Func<IModel> _createChannel;
   //   private readonly object _channelLock = new object();

   //   private IModel _consumerChannel = null;
   //   private IModel _publisherChannel = null;

   //   private EventingBasicConsumer _consumer = null;
   //   private string _queueName = string.Empty;
   //   private bool _deleteQueueAtDispose = false;

   //   protected MessageQueue (Func<IModel> createChannel) {
   //      _createChannel = createChannel;
   //   }

   //   public void Configure (MessageConnection connection,
   //                          string queueName,
   //                          bool deleteQueueAtDispose = false) {
   //      _queueName = queueName;
   //      _deleteQueueAtDispose = deleteQueueAtDispose;
   //   }

   //   public void RegisterMessageHandler (Action<string> messageHandler) {
   //      lock (_channelLock) {
   //         if (_consumerChannel == null) {
   //            _consumerChannel = CreateQueue();
   //            _consumerChannel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
   //         }
   //      }

   //      // Setup consumer
   //      _consumer = new EventingBasicConsumer(_consumerChannel);
   //      _consumer.Received += (sender,
   //                             delivery) => {
   //                                var body = delivery.Body;
   //                                string message = Encoding.UTF8.GetString(body);
   //                                messageHandler.Invoke(message);

   //                                // Ack the message
   //                                _consumerChannel.BasicAck(deliveryTag: delivery.DeliveryTag, multiple: false);
   //                             };

   //      //consumer.ConsumerCancelled += Consumer_ConsumerCancelled;
   //      //consumer.Shutdown += Consumer_Shutdown;
   //      //consumer.Registered += Consumer_Registered;
   //      //consumer.Unregistered += Consumer_Unregistered;

   //      _consumerChannel.BasicConsume(_queueName, false, _consumer);
   //   }

   //   protected IModel CreateQueue () {
   //      var channel = _createChannel();
   //      channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

   //      return channel;
   //   }

   //   public void Send (string message) {
   //      lock (_channelLock) {
   //         if (_publisherChannel == null) {
   //            _publisherChannel = CreateQueue();
   //         }
   //      }

   //      var properties = _publisherChannel.CreateBasicProperties();
   //      properties.Persistent = true;
   //      var body = Encoding.UTF8.GetBytes(message);
   //      _publisherChannel.BasicPublish(exchange: "",
   //                             routingKey: _queueName,
   //                             basicProperties: properties,
   //                             body: body);
   //   }

   //   public void Dispose () {
   //      if (_deleteQueueAtDispose) {
   //         if ((_publisherChannel ?? _consumerChannel) == null) {
   //            _publisherChannel = CreateQueue();
   //         }

   //         (_publisherChannel ?? _consumerChannel).QueueDeleteNoWait(_queueName);
   //      }

   //      _consumerChannel?.Close();
   //      _consumerChannel?.Dispose();

   //      _publisherChannel?.Close();
   //      _publisherChannel?.Dispose();
   //   }
   //}

   public abstract class OneWayMessagingBase : MessagingBase {
      protected OneWayMessagingBase (IModel channel,
                                     string name,
                                     string[] tags) : base (name, tags) {
         channel.CheckNull (nameof(channel)) ;
         _channel = channel ;
      }

      public sealed override void Close() {
         _logger.Debug ("Closing message channel.") ;

         MessageConnection.CloseChannel (_channel, _logger.Tags) ;
      }

      protected readonly IModel _channel ;
   }

   public abstract class TwoWayMessagingBase : MessagingBase {
      protected TwoWayMessagingBase (string name, 
                                     IModel sendChannel,
                                     IModel receiveChannel,                                     
                                     string[] tags) : base (name, tags) {
         sendChannel.CheckNull (nameof(sendChannel)) ;
         _sendChannel = sendChannel ;

         receiveChannel.CheckNull (nameof(receiveChannel)) ;
         _receiveChannel = receiveChannel ;
      }

      public sealed override void Close() {
         _logger.Debug ("Closing send and receive message channels.") ;
         
         MessageConnection.CloseChannel (_sendChannel, _logger.Tags) ;
         MessageConnection.CloseChannel (_receiveChannel, _logger.Tags) ;
      }

      protected readonly IModel _sendChannel;
      protected readonly IModel _receiveChannel;
   }
}
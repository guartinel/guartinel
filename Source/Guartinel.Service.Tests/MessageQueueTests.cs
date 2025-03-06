using System ;
using System.Collections.Concurrent ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using System.Threading.Tasks ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Service.MessageQueues ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;
using RabbitMQ.Client ;
using Timeout = Guartinel.Kernel.Timeout ;

namespace Guartinel.Service.Tests {
   [TestFixture]
   public class MessageQueueTests {
      public class Constants {
         public const string TEST_QUEUE_NAME = "UNIT_TEST_1ABRPK" ;

         //// public const string SERVER_ADDRESS = "185.203.119.150" ;
         //public const string SERVER_ADDRESS = "localhost" ;

         //public const int SERVER_PORT = 5672 ;

         //// public const string USER_NAME = "rabbitmquser" ;
         //public const string USER_NAME = "guest" ;

         //// public const string PASSWORD = "j12bXXhJsIcPtTBBXQgl" ;
         //public const string PASSWORD = "guest" ;
      }

      [SetUp]
      public void Setup() {
         // Delete queue
         using (MessageConnection messageConnection = new MessageConnection()) {
            // @todo: SzTZ: delete queue
            //messageConnection.Connect() ;
            //using (MessageQueue messageQueue = new MessageQueue()) {
            //   messageQueue.Configure (messageConnection, Constants.TEST_QUEUE_NAME, true) ;
            //}
         }
      }

      [Test]
      public void CreateBroadcastPair_CheckIfArrives() {
         JObject messageSent = new JObject() ;
         messageSent ["id"] = Guid.NewGuid().ToString() ;
         BlockingCollection<JObject> messagesGot = new BlockingCollection<JObject>() ;

         using (MessageConnection messageConnection = new MessageConnection()) {
            BroadcastSender sender = messageConnection.CreateBroadcastSender (Constants.TEST_QUEUE_NAME, null) ;
            BroadcastConsumer consumer = messageConnection.CreateBroadcastConsumer (Constants.TEST_QUEUE_NAME,
                                                                                    message => {
                                                                                       messagesGot.Add (message) ;
                                                                                    }, null) ;
            new Timeout (TimeSpan.FromSeconds (2)).Wait() ;
            sender.Send (messageSent) ;

            new Timeout (TimeSpan.FromSeconds (5)).WaitFor (() => messagesGot.Count > 0) ;
            Assert.AreEqual (1, messagesGot.Count) ;
            var messageGot = messagesGot.Take() ;
            Assert.IsNotNull (messageGot) ;
            Assert.AreEqual (messageSent ["id"], messageGot ["id"]) ;
         }
      }

      [Test]
      public void CreateBroadcastConsumerAnd2Senders_CheckIfGoesThrough() {
         JObject messageSent1 = new JObject() ;
         messageSent1 ["id"] = Guid.NewGuid().ToString() ;
         JObject messageSent2 = new JObject() ;
         messageSent1 ["id"] = Guid.NewGuid().ToString() ;

         BlockingCollection<JObject> messagesGot = new BlockingCollection<JObject>() ;

         using (MessageConnection messageConnection = new MessageConnection()) {
            BroadcastSender sender1 = messageConnection.CreateBroadcastSender (Constants.TEST_QUEUE_NAME, null) ;
            BroadcastSender sender2 = messageConnection.CreateBroadcastSender (Constants.TEST_QUEUE_NAME, null) ;
            messageConnection.CreateBroadcastConsumer (Constants.TEST_QUEUE_NAME,
                                                       message => {
                                                          messagesGot.Add (message) ;
                                                       }, null) ;
            new Timeout (TimeSpan.FromSeconds (2)).Wait() ;
            sender1.Send (messageSent1) ;
            sender2.Send (messageSent2) ;

            new Timeout (TimeSpan.FromSeconds (5)).WaitFor (() => messagesGot.Count >= 2) ;
            Assert.AreEqual (2, messagesGot.Count) ;
            var messageGot1 = messagesGot.Take() ;
            var messageGot2 = messagesGot.Take() ;
            Assert.IsNotNull (messageGot1) ;
            Assert.IsNotNull (messageGot2) ;
            Assert.AreEqual (messageSent1 ["id"], messageGot1 ["id"]) ;
            Assert.AreEqual (messageSent2 ["id"], messageGot2 ["id"]) ;
         }
      }
   }

   // Test server
   public class TestServer {
      public void Start (MessageConnection messageConnection) {
         ServiceServer server = messageConnection.CreateServiceServer ("testSzTZ1", received => {
            var result = new JObject() ;
            Thread.Sleep (500) ;
            result ["success"] = true ;
            result ["data"] = received ["data"] ;
            return result ;
         }, null) ;
      }
   }

   public class TestClient {
      private readonly MessageConnection _messageConnection ;

      public TestClient (MessageConnection messageConnection) {
         _messageConnection = messageConnection ;
      }

      public void Send (CancellationToken cancellation,
                        Action<JObject> processResult) {
         var data = new JObject() ;
         data ["data"] = Guid.NewGuid().ToString() ;

         _messageConnection.CallServiceClient ("testSzTZ1", data, result => {processResult?.Invoke (result) ;}, cancellation, null) ;
      }
   }

   [TestFixture]
   public class TestTerminationOfQueueService {
      [Test]
      public void TestX() {
         Logger.Settings.LogLevel = LogLevel.DetailedDebug ;
         Logger.RegisterLogger<SimpleConsoleLogger>();

         using (MessageConnection messageConnection = new MessageConnection (@"http://10.0.75.1:5672/",
                                                                             "guest",
                                                                             "guest")) {
            var timeout = new TimeoutSeconds(20);
            var testServer = new TestServer();
            testServer.Start (messageConnection) ;

            var cancellation = new CancellationTokenSource() ;
            var client = new TestClient (messageConnection) ;
            var task = new Task (() => {
               while (!cancellation.IsCancellationRequested) {
                  try {
                     client.Send (cancellation.Token, null) ;
                  } catch (RabbitMQ.Client.Exceptions.OperationInterruptedException e) {
                     Logger.Error (e.Message) ;
                     // Continue
                  }

                  Thread.Sleep (1000) ;
               }
            }, cancellation.Token) ;
            task.Start() ;

            while (!cancellation.IsCancellationRequested) {
               if (timeout.RunnedOut) {
                  cancellation.Cancel();
                  new TimeoutSeconds (5).Wait();
               }

               Thread.Sleep (1000) ;
            }
         }
      }

      [Test]
      public void TestNoAnswerTimeout () {
         Logger.Settings.LogLevel = LogLevel.DetailedDebug ;
         Logger.RegisterLogger<SimpleConsoleLogger>();

         using (MessageConnection messageConnection = new MessageConnection(@"http://10.0.75.1:5672/",
                                                                            "guest",
                                                                            "guest")) {
            var cancellation = new CancellationTokenSource();
            cancellation.CancelAfter (TimeSpan.FromSeconds (25)) ;
            var client = new TestClient(messageConnection);
            var task = new Task(() => {
            client.Send(cancellation.Token, null) ;
            }, cancellation.Token);
            task.Start();

            while (!cancellation.IsCancellationRequested) {
               new TimeoutSeconds(2).Wait();
            }

            new TimeoutSeconds(5).Wait();
         }
      }
   }
}
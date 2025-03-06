using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel.Tests.Messages {
   //[TestFixture]
   //public class TestMessagesXml : TestXmls {
   //   protected override void SetUp() {
   //      base.SetUp() ;
       
   //      Factory.Use.RegisterCreator (new Creator (typeof (TestMessage), () => new TestMessage())) ;
   //   }

   //   public class TestMessage : Message {
   //      public void SetValue (string name,
   //                            IPersistent value) {
   //         Set (name, value) ;
   //      }

   //      public IPersistent GetValue (string name) {
   //         return Get (name) ;
   //      }

   //   }

   //   [Test]
   //   public void CreateMessageSaveAndLoad() {
   //      TestMessage message = new TestMessage() ;
   //      message.SetValue ("TestName1", new StringValue ("TestValue1")) ;
   //      message.SetValue ("TestName2", new IntegerValue (112)) ;

   //      var xml = Factory.Use.SaveInstanceToXml (message) ;
   //      TestMessage loadedMessage = (TestMessage) Factory.Use.CreateInstanceFromXml (xml) ;

   //      Assert.AreEqual (((StringValue) message.GetValue ("TestName1")).Value, ((StringValue) loadedMessage.GetValue ("TestName1")).Value) ;
   //      Assert.AreEqual (((IntegerValue) message.GetValue ("TestName2")).Value, ((IntegerValue) loadedMessage.GetValue ("TestName2")).Value) ;
   //   }
   //}
}
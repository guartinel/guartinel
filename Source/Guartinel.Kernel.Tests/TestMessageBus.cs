using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   [TestFixture]
   public class TestMessageBus {
      public class Test1 {
         public string Value = string.Empty ;

         public Test1() { }

         public Test1 (string value) {
            Value = value ;
         }
      }

      public class Test2 { }

      [SetUp]
      public void Setup() { }

      [TearDown]
      public void TearDown() {
         _events.Clear() ;
         MessageBus.Use.Reset() ;
      }

      private readonly List<string> _events = new List<string>() ;

      [Test]
      public void TestUsingType() {
         MessageBus.Use.Register<Test1> (x => {
            _events.Add (x.Value) ;
         }) ;

         MessageBus.Use.Post (new Test1 ("message1")) ;
         // Wait a bit
         new Timeout (2000).WaitFor (() => _events.Count (x => x == "message1") == 1) ;

         Assert.AreEqual (1, _events.Count (x => x == "message1")) ;
      }

      [Test]
      public void TestUsingtypeAndID() {
         MessageBus.Use.Register<Test1> ("id1", x => {
            _events.Add ($"id1: {x.Value}") ;
         }) ;

         MessageBus.Use.Register<Test1> ("id2", x => {
            _events.Add ($"id2: {x.Value}") ;
         }) ;

         MessageBus.Use.Post ("id1", new Test1 ("message1")) ;
         MessageBus.Use.Post ("id1", new Test1 ("message1a")) ;
         MessageBus.Use.Post ("id2", new Test1 ("message2")) ;

         // Wait a bit
         new Timeout (TimeSpan.FromSeconds (2)).WaitFor (() => _events.Count (x => x == "id1: message1") == 1) ;
         Assert.AreEqual(1, _events.Count(x => x == "id1: message1"));

         new Timeout (TimeSpan.FromSeconds (2)).WaitFor (() => _events.Count (x => x == "id1: message1a") == 1) ;
         Assert.AreEqual(1, _events.Count(x => x == "id1: message1a"));

         new Timeout (TimeSpan.FromSeconds (2)).WaitFor (() => _events.Count (x => x == "id2: message2") == 1) ;
         Assert.AreEqual(1, _events.Count(x => x == "id2: message2"));
      }
   }
}
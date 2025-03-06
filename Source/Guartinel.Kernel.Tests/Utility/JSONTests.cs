using System ;
using System.Diagnostics ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Logging ;
using NUnit.Framework ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel.Tests.Utility {
   [TestFixture]
   internal class JSONTests {
      [SetUp]
      public void SetUp() {
         Logger.Setup<SimpleConsoleLogger> ("Test", "Test") ;
      }

      [Test]
      public void TestGetValue() {
         JObject jobject1 = new JObject() ;
         jobject1 ["one"] = "test1" ;
         jobject1 ["two"] = null ;

         Assert.AreEqual ("test1", jobject1.GetStringValue ("one")) ;
         Assert.AreEqual ("", jobject1.GetStringValue("two")) ;
      }

      [Test]
      public void TestGetProperty() {
         JObject jobject1 = new JObject() ;
         jobject1 ["one"] = "test1" ;
         jobject1 ["two"] = null ;

         Assert.AreEqual ("test1", jobject1.GetProperty<string>("one")) ;
         Assert.IsNull (jobject1.GetProperty<string> ("two")) ;
         Assert.AreEqual ("test1", jobject1.GetProperty<string>("two", "one")) ;
      }

      [Test]
      public void TestEscapedString () {
         const string VALUE = @"\\nehune1";

         //JValue value1 = new JValue (VALUE) ;
         //object value2 = value1.Value ;
         //Assert.AreEqual ("x", value2) ;
         //Assert.AreEqual (VALUE, value1.ToString (Formatting.None)) ;

         JObject test1 = new JObject();
         test1["name1"] = VALUE;         
         Assert.AreEqual(VALUE, test1["name1"].Value<string>());
         Assert.AreEqual(VALUE, JSONEx.GetStringValue (test1, "name1"));
         Assert.AreEqual(@"{""name1"":""\\\\nehune1""}", test1.ToString(Formatting.None));

         string message = $@"Message: {JSONEx.GetStringValue(test1, "name1")}";
         Assert.AreEqual(@"Message: \\nehune1", message);         
         Logger.Log (message) ;
      }

      [Test]
      public void TestJsonToString () {
         JObject testObject = new JObject();
         testObject["test"] = @"http://www.sysment.hu";

         Debug.WriteLine(testObject.ToString(Formatting.None));

         Assert.AreEqual(@"{""test"":""http://www.sysment.hu""}", testObject.ToString(Formatting.None));
      }
   }
}
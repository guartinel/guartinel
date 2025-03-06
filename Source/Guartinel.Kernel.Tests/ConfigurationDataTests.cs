using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Network ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   [TestFixture]
   internal class ConfigurationDataTests {
      [Test]
      public void SetValue_Check() {
         ConfigurationData configurationData1 = new ConfigurationData() ;
         configurationData1 ["test1"] = "Test1Value" ;

         Assert.AreEqual ("Test1Value", configurationData1 ["test1"]) ;
      }

      protected string TrimJson (string value) {
         if (string.IsNullOrEmpty (value)) {
            return String.Empty ;}

         return value.Replace (@"""", "")
                     .Replace ("\n", "")
                     .Replace ("\r", "")
                     .Replace (" ", "") ;
      }

      [Test]
      public void SetChild_Check() {
         ConfigurationData configurationData1 = new ConfigurationData() ;

         var child1 = new ConfigurationData() ;
         child1 ["value1"] = "Value1" ;
         child1 ["value2"] = "Value2" ;
         configurationData1.SetChild ("child1", child1) ;

         Assert.AreEqual ("Value1", configurationData1.GetChild ("child1") ["value1"]) ;
         Assert.AreEqual ("Value2", configurationData1.GetChild ("child1") ["value2"]) ;

         Assert.AreEqual (@"{child1:{value1:Value1,value2:Value2}}", TrimJson (configurationData1.ToString())) ;
      }

      [Test]
      public void SetChildren_Check() {
         ConfigurationData configurationData1 = new ConfigurationData() ;
         
         var child1 = new ConfigurationData() ;
         child1 ["value1"] = "Value11" ;
         child1 ["value2"] = "Value12" ;

         var child2 = new ConfigurationData() ;
         child2 ["value1"] = "Value21" ;
         child2 ["value2"] = "Value22" ;

         configurationData1.SetChildren ("children", new List<ConfigurationData> {
            child1, child2
         }) ;

         List<ConfigurationData> children = configurationData1.GetChildren ("children") ;
         Assert.AreEqual (2, children.Count) ;
         Assert.AreEqual ("Value11", children [0]["value1"]) ;
         Assert.AreEqual ("Value12", children [0]["value2"]) ;
         Assert.AreEqual ("Value21", children [1]["value1"]) ;
         Assert.AreEqual ("Value22", children [1]["value2"]) ;
         
         Assert.AreEqual (@"{children:[{value1:Value11,value2:Value12},{value1:Value21,value2:Value22}]}", TrimJson (configurationData1.ToString())) ;
      }

      // Manual test
      [Test]
      public void Manual_TestNotificationForConfigurationChange () {

         bool notified = false;
         const string KEY = @"Tests/UnitTest";
         const string TOKEN = "1234";

         GlobalConfiguration.Use.SubscribeForChange(KEY, TOKEN, 10, () => {
            notified = true;
         });

         new TimeoutSeconds (60).WaitFor (() => notified, TimeSpan.FromSeconds (3)) ;

         Assert.IsTrue(notified);
      }
   }
}

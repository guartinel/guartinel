using System ;
using System.Collections.Generic;
using System.IO ;
using Guartinel.CLI.Tests.Files ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Network {
   [TestFixture]
   public class PingsTests : FileTestsBase {
      [SetUp]
      public new void SetUp() {
         base.SetUp();
      }

      [TearDown]
      public new void TearDown () {
         base.TearDown();
      }

      [Test]
      public void PingHosts_CheckResults() {
         var result = RunCommand ("x5gym.dyndns.org:8900", 
                                  "www.sysment.com:80",                                  
                                  "backend2.guartinel.com:2174",
                                  "x5gym.dyndns.org:8892") ;
         Assert.AreEqual (4, result.Count) ;

         Assert.IsTrue (result [0].Success) ;
         Assert.IsTrue (result [1].Success) ;
         Assert.IsFalse (result [2].Success, result [2].Message) ;
         Assert.IsTrue (result [3].Success) ;
      }

      private List<CheckResult> RunCommand (params string[] targets) {
         return RunCommand ("pings", () => CreateArguments (targets)) ;
      }

      private List<string> CreateArguments (params string[] targets) {
         List<string> arguments = new List<string>();
         
         var listFile = Path.Combine (_testFolder, "test_list.txt") ;
         File.WriteAllLines (listFile, targets) ;

         arguments.Add ("pings") ;            
         arguments.Add ($"--pingsFile={listFile}") ;

         return arguments ;
      }
   }
}
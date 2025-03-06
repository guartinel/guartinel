using System ;
using System.Collections.Generic;
using System.Diagnostics ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Network {
   [TestFixture]
   public class PingTests : TestsBase {
      [SetUp]
      public new void SetUp () {
         base.SetUp();
      }

      [TearDown]
      public new void TearDown () {
         base.TearDown();
      }

      [Test]
      public void PingHost_CheckResults() {
         var result = RunCommand ("www.index.hu") [0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand ("ehune.xc") [0] ;
         Assert.IsFalse (result.Success, result.ToString()) ;
         Debug.WriteLine (result.Message) ;
         Assert.IsTrue (result.Message.ToLowerInvariant().Contains ("ehune.xc")) ;
      }

      private List<CheckResult> RunCommand (string host) {
         return RunCommand ("ping", () => CreateArguments (host)) ;
      }

      private static List<string> CreateArguments (string host) {

         List<string> arguments = new List<string>() ;
         arguments.Add ("ping") ;
         arguments.Add ($"--target={host}") ;
                  
         return arguments ;
      }
   }
}
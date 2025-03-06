using System ;
using System.Collections.Generic;
using System.Diagnostics ;
using System.Linq;
using Fclp;
using Guartinel.CLI.Utility.Commands;
using Guartinel.Core ;
using NUnit.Framework;

namespace Guartinel.CLI.Utility.Tests.Network {
   [TestFixture]
   public class PingTests : TestsBase {
      [Test]
      public void PingHosts_CheckResults() {
         var result = RunCommand("sysment.hu") [0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand ("ehune.xc") [0] ;
         Assert.IsFalse (result.Success, result.ToString()) ;
         Debug.WriteLine (result.Message) ;
         Assert.IsTrue (result.Message.ToLowerInvariant().Contains ("ehune.xc")) ;
      }

      private List<CheckResult> RunCommand (string host) {
         ICommand pingCommand = IoC.Use.GetAllInstances<ICommand>().First (x => x.Command == "ping") ;
         FluentCommandLineParser parser = new FluentCommandLineParser() ;
         pingCommand.Setup (parser) ;

         var arguments = CreateArguments (host) ;
         parser.Parse (arguments.ToArray()) ;
         return pingCommand.Run() ;
      }

      private static List<string> CreateArguments (string host) {

         List<string> arguments = new List<string>() ;
         arguments.Add ("--command:ping") ;
         arguments.Add ($"--target:{host}") ;
                  
         return arguments ;
      }
   }
}
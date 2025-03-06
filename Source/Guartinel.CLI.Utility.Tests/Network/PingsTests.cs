using System ;
using System.Collections.Generic;
using System.IO ;
using System.Linq;
using Fclp;
using Guartinel.CLI.Utility.Commands;
using Guartinel.CLI.Utility.Tests.Files ;
using Guartinel.Core ;
using NUnit.Framework;

namespace Guartinel.CLI.Utility.Tests.Network {
   [TestFixture]
   public class PingsTests : FileTestsBase {

      [Test]
      public void PingHosts_CheckResults() {

         ICommand pingsCommand = IoC.Use.GetAllInstances<ICommand>().First (x => x.Command == "pings") ;
         FluentCommandLineParser parser = new FluentCommandLineParser() ;
         pingsCommand.Setup (parser) ;

         var arguments = CreateArguments ("x5gym.dyndns.org:8900",
                                          "www.sysment.com:80",
                                          "backend2.guartinel.com:2174",
                                          "x5gym.dyndns.org:8892") ;

         parser.Parse (arguments.ToArray()) ;

         var result = pingsCommand.Run() ;
         Assert.AreEqual (4, result.Count) ;

         Assert.IsTrue (result [0].Success) ;
         Assert.IsTrue (result [1].Success) ;
         Assert.IsFalse (result [2].Success, result [2].Message) ;
         Assert.IsTrue (result [3].Success) ;
      }

      private List<string> CreateArguments (params string[] targets) {
         List<string> arguments = new List<string>();
         
         var listFile = Path.Combine (_testFolder, "test_list.txt") ;
         File.WriteAllLines (listFile, targets) ;

         arguments.Add ("--command:pings") ;            
         arguments.Add ($"--list_file:{listFile}") ;

         return arguments ;
      }
   }
}
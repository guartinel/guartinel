using System ;
using System.Collections.Generic ;
using Fclp ;
using Guartinel.CLI.Utility.Commands ;
using Guartinel.Core.Logging ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility {
   public class TestCommand : BaseCommand {
      public override string Description => "Test command." ;
      public override string Command => "test" ;
      
      private bool _echo = false;

      protected override void Setup1 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<bool> ("echo").Callback (value => _echo = value) ;
      }

      protected override List<CheckResult> Run1 () {
         if (_echo) {
            string line ;
            while ((line = Console.ReadLine()) != null) {               
               Console.WriteLine (line) ;
               // Logger.Log (LogLevel.Info, $"Test echo command executed, line: {line}.");
            }            
         }

         return new List<CheckResult> { new CheckResult (true, $"Test command successful.", null)} ;
      }
   }
}
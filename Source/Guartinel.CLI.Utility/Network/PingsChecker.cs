using System;
using System.Collections.Generic;
using System.Linq;
using Fclp;
using Guartinel.Core;
using Guartinel.Core.Logging;
using Guartinel.Core.Utility;

namespace Guartinel.CLI.Utility.Network {
   public class PingsChecker : PingCheckerBase {
      public new static class Constants {
         public static class Parameters {
            public const string LIST_FILE = "list_file" ;
         }
      }

      private string _listFile ;

      public override string Description => $"Check ping of a list of targets." ;

      public override string Command => $"pings" ;

      protected override void Setup3 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> (Constants.Parameters.LIST_FILE).Required().Callback (value => _listFile = value) ;
      }

      protected List<string> ReadTargets() {
         try {
         return System.IO.File.ReadAllText (_listFile).Replace ("\r", string.Empty).Split ("\n", StringSplitOptions.RemoveEmptyEntries).ToList() ;
         } catch (Exception e) {
            Logger.Log (LogLevel.Info, $"Cannot read ping list check from file '{_listFile}'. Message: {e.GetAllMessages()}") ;
            return new List<string>() ;
         }
      }

      protected override List<CheckResult> Run2() {
         var targets = ReadTargets() ;
         var results = new List<CheckResult>() ;

         foreach (var target in targets) {
            results.Add (Ping (target)) ;
         }

         return results ;
      }
   }
}
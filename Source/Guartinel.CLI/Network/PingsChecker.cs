using System;
using System.Collections.Generic;
using System.Linq;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility;
using Microsoft.Extensions.CommandLineUtils ;

namespace Guartinel.CLI.Network {
   public class PingsChecker : PingCheckerBase {
      public new static class Constants {
         public static class Parameters {
            public const string PINGS_FILE = "pingsFile" ;
         }

         public const string COMMENT_LINE_PREFIX = "#" ;
         public const string NAME_AND_ADDRESS_SEPARATOR = "|" ;
      }

      public override string Description => $"Check ping of a list of targets." ;

      public override string Command => $"pings" ;

      public string ListFile => FilesEx.EnsureFileNameHasFullPath (Parameters.GetStringValue (Constants.Parameters.PINGS_FILE, string.Empty)) ;

      protected List<string> ReadLines() {
         try {
            if (string.IsNullOrEmpty (ListFile)) return new List<string>() ;
            if (!System.IO.File.Exists (ListFile)) return new List<string>() ;

            var result = System.IO.File.ReadAllText (ListFile).Replace ("\r", string.Empty).Split ("\n", StringSplitOptions.RemoveEmptyEntries).ToList() ;
            // Trim elements
            // Remove comment lines
            result = result.Select (x => x.Trim()).Where (x => !x.StartsWith (Constants.COMMENT_LINE_PREFIX)).ToList() ;

            return result ;
         } catch (Exception e) {
            Logger.Info ($"Cannot read ping list check from file '{ListFile}'. Message: {e.GetAllMessages()}") ;
            return new List<string>() ;
         }
      }

      protected override List<CheckResult> Run2() {
         var lines = ReadLines() ;
         var results = new List<CheckResult>() ;

         foreach (var line in lines) {
            var targetAndName = line.Split (Constants.NAME_AND_ADDRESS_SEPARATOR).ToList() ;
            var target = targetAndName.Count > 1 ? targetAndName [1] : targetAndName [0] ;
            var caption = targetAndName.Count > 1 ? targetAndName [0] : null ;

            var result = Ping (new Host (target, caption), Retries, WaitTimeSeconds, TimeoutSeconds) ;
            if (!string.IsNullOrEmpty (caption)) {
               result.Name = caption ;
            }

            results.Add (result) ;
         }

         return results ;
      }
   }

   public class PingsCheckerCl : PingCheckerBaseCl<PingsChecker> {
      protected override void Setup3 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser,
                      PingsChecker.Constants.Parameters.PINGS_FILE,
                      $"Full path to file where the ping targets are listed in the format of name" +
                      $"{PingsChecker.Constants.NAME_AND_ADDRESS_SEPARATOR}address{Pinger.Constants.PORT_SEPARATOR}port.") ;
      }
   }
}
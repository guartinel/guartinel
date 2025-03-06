using System;
using System.Collections.Generic ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Commands {
   public class VersionCommand : BaseCommand {
      public VersionCommand() { }

      public override string Description => "Display version." ;
      public override string Command => "version" ;

      protected override List<CheckResult> Run1() {
         var version = typeof(Application).Assembly.GetName().Version.ToString() ;
         Console.WriteLine (version) ;
         // Nothing to do, just return the version
         return new List<CheckResult>{new CheckResult (true, version, version, version, null)} ;
      }
   }

   public class VersionCommandCl : BaseCommandCl<VersionCommand> {
      public VersionCommandCl () { }

      protected override void Setup1 (CommandLineApplication commandLineParser) {
      }
   }
}

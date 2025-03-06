using System;
using System.Collections.Generic ;
using Fclp;

namespace Guartinel.CLI.Utility.Commands {
   public class NullCommand : ICommand {
      public string Description => string.Empty ;
      public string Command => string.Empty;
      public void Setup (FluentCommandLineParser commandLineParser) {
         // Nothing to do
      }

      public List<CheckResult> Run() {
         // Nothing to do
         return new List<CheckResult> {new CheckResult.InvalidParameters()} ;
      }

      public string Info => string.Empty ;
   }
}

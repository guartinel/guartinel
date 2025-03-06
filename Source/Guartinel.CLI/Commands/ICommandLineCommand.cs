using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Microsoft.Extensions.CommandLineUtils ;

namespace Guartinel.CLI.Commands {
   public interface ICommandLineCommand {
      /// <summary>
      /// Description of the check.
      /// </summary>
      string Description { get; }

      /// <summary>
      /// Command.
      /// </summary>
      string Command { get; }

      /// <summary>
      /// Run check according to the parameters.
      /// </summary>
      /// <returns></returns>
      List<CheckResult> Run ();

      /// <summary>
      /// Setup command parser.
      /// </summary>
      void Setup (CommandLineApplication commandLineParser,
                  Action<ICommandLineCommand> setCommandToRun) ;
   }
}
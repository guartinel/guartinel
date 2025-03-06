using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Fclp ;

namespace Guartinel.CLI.Utility.Commands {
   public interface ICommand {
      /// <summary>
      /// Description of the check.
      /// </summary>
      string Description {get ;}

      /// <summary>
      /// Command.
      /// </summary>
      string Command {get ;}

      /// <summary>
      /// Setup command line parser
      /// </summary>
      /// <param name="commandLineParser"></param>
      void Setup (FluentCommandLineParser commandLineParser) ;

      /// <summary>
      /// Run check according to the parameters.
      /// </summary>
      /// <returns></returns>
      List<CheckResult> Run() ;
   }
}
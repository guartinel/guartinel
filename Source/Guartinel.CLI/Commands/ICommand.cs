using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Commands {
   //public class CommandParserMicrosoft : CommandParser {
   //   public CommandParserMicrosoft() {
   //      _commandLine = new CommandLineApplication (false) ;
   //   }

   //   private readonly CommandLineApplication _commandLine ;

   //   public override Option AddOption (string name, string description) {         
   //      var option = _commandLine.Option ($"--{name} <{name}>", description, CommandOptionType.SingleValue) ;

   //      Option result = new Option() ;
   //      return result;
   //   }

   //}

   //public abstract class CommandParser {
   //   public class Option {
   //   }

   //   public abstract Option AddOption (string name, string description) ;
   //}

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
      /// Run check according to the parameters.
      /// </summary>
      /// <returns></returns>
      List<CheckResult> Run() ;

      /// <summary>
      /// Setup command from configuration.
      /// </summary>
      /// <param name="parameters">Parameters for command.</param>
      /// <param name="merge">Merge parameters into existing ones?</param>
      void Setup (JObject parameters,
                  bool merge = false) ;
   }
}
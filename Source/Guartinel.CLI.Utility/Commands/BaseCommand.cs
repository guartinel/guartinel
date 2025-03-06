using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using Fclp ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility.Commands {
   public abstract class BaseCommand : ICommand {
      public static class Constants {
         public static class Parameters {
            public const string CONFIGURATION_FILE = "configuration" ;
            public const string READ_FROM_CONSOLE = "read" ;
            public const string WRITE_TO_CONSOLE = "write" ;
         }
      }

      public abstract string Description {get ;}
      public abstract string Command {get ;}

      /// <summary>
      /// Parameters for run.
      /// </summary>
      protected JObject _parameters = new JObject() ;

      protected void Merge (JObject parameters,
                            JObject additionalParameters) {
         foreach (var parameter in additionalParameters) {
            // DO NOT overwrite values
            if (parameters [parameter.Key] == null) {
               parameters [parameter.Key] = parameter.Value ;
            }
         }
      }

      protected void SetupOption (FluentCommandLineParser commandLineParser,
                                  string name) {
         commandLineParser.Setup<string> (name).Callback (value => _parameters [name] = value) ;
      }

      protected void SetupOptionBool (FluentCommandLineParser commandLineParser,
                                      string name) {
         commandLineParser.Setup<bool> (name).Callback (value => _parameters [name] = value) ;
      }

      /// <summary>
      /// Read configuration from file.
      /// </summary>
      protected string _configurationFile = string.Empty ;

      /// <summary>
      /// Read configuration from standard input.
      /// </summary>
      // protected bool _read = false ;

      /// <summary>
      /// Write result to standard output.
      /// </summary>
      protected bool _write = false ;

      public void Setup (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> (Constants.Parameters.CONFIGURATION_FILE).Callback (value => _configurationFile = value) ;
         // commandLineParser.Setup<bool> (Constants.Parameters.READ_FROM_CONSOLE).Callback (value => _read = value) ;
         commandLineParser.Setup<bool> (Constants.Parameters.WRITE_TO_CONSOLE).Callback (value => _write = value) ;

         Setup1 (commandLineParser) ;
      }

      protected virtual void Setup1 (FluentCommandLineParser commandLineParser) {}

      public List<CheckResult> Run() {
         // Use configuration file first 
         if (!string.IsNullOrEmpty (_configurationFile)) {
            // Logger.Log(LogLevel.Info, $"Test write command executed, line: {_write}.");            
            string configurationFileFolder = Path.GetDirectoryName (_configurationFile) ;
            if (string.IsNullOrEmpty (configurationFileFolder)) {
               _configurationFile = Path.Combine (Directory.GetCurrentDirectory(), _configurationFile) ;
            }

            string configuration = File.ReadAllText (_configurationFile) ;
            Merge (_parameters, JObject.Parse (configuration)) ;
         }

         //// Read configuration from console if specified so
         //if (_read) {
         //   // Logger.Log(LogLevel.Info, $"Test write command executed, line: {_write}.");

         //   StringBuilder readParameters = new StringBuilder();

         //   string line;
         //   while ((line = Console.ReadLine()) != null && line != string.Empty) {
         //      readParameters.Append (line) ;
         //   }

         //   if (!string.IsNullOrEmpty (readParameters.ToString())) {
         //      try {
         //         var readObject = JObject.Parse (readParameters.ToString()) ;
         //         Merge(_parameters, readObject) ;
         //      } catch (JsonReaderException e) {
         //         // Ignore error
         //      }
         //   }
         //}

         List<CheckResult> results = Run1() ;

         // Write result to output
         if (_write) {
            // Logger.Log(LogLevel.Info, $"Test write command executed, line: {_write}.");
            foreach (var result in results) {
               Console.WriteLine (result.ToString()) ;
            }            
         }

         return results ;
      }

      protected virtual List<CheckResult> Run1() {
         return new List<CheckResult> {new CheckResult.InvalidParameters()} ;
      }
   }
}
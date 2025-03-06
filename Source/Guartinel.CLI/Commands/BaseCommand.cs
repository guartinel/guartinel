using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Commands {
   public abstract class BaseCommand : ICommand {
      public static class Constants {
         public static class Parameters {
            public const string LOG_FOLDER = "logFolder" ;

            // public const string READ_FROM_CONSOLE = "read" ;
            public const string WRITE_TO_CONSOLE = "write" ;

            // 2018/05/03
            [Obsolete]
            public const string CONFIGURATION_FILE_OBSOLETE = "configuration" ;

            public const string CONFIGURATION_FILE = "configurationFile" ;
         }
      }

      protected TagLogger _logger = new TagLogger() ;

      public abstract string Description {get ;}
      public abstract string Command {get ;}

      /// <summary>
      /// Parameters for run.
      /// </summary>
      private JObject _parameters = new JObject() ;

      public JObject Parameters => _parameters ;

      protected void Merge (JObject parameters,
                            JObject additionalParameters) {
         foreach (var parameter in additionalParameters) {
            // DO NOT overwrite values
            if (parameters [parameter.Key] == null) {
               parameters [parameter.Key] = parameter.Value ;
            }
         }
      }

      public void Setup (JObject parameters,
                         bool merge = false) {
         if (merge) {
            Merge (_parameters, parameters) ;
         } else {
            _parameters = new JObject (parameters) ;
         }

         _logger = new TagLogger (_logger.Tags, Command) ;
      }

      public void SetParameter (string parameterName,
                                JToken value) {
         _parameters [parameterName] = value ;
      }

      public List<CheckResult> Run() {
         // Use configuration file first
         var configurationFileName = GetConfigurationFileName() ;

         if (!string.IsNullOrEmpty (configurationFileName)) {
            string configuration = File.ReadAllText (configurationFileName) ;
            Merge (_parameters, JObject.Parse (configuration)) ;
         }         

         // Log folder
         if (!string.IsNullOrEmpty (_parameters [Constants.Parameters.LOG_FOLDER]?.ToString())) {
            Logger.SetSetting (FileLogger.Constants.SETTING_NAME_FOLDER, _parameters [Constants.Parameters.LOG_FOLDER].ToString()) ;
         } else {
            Logger.SetSetting (FileLogger.Constants.SETTING_NAME_FOLDER, AssemblyEx.AddToAssemblyPath<Program> ("Logs")) ;
         }

         _logger.InfoWithDebug ($"Command '{Command}' started.", $"Parameters: {_parameters.ConvertToLog ()}") ;

         var results = Run1() ;

         // Write result to output
         if (_parameters.GetBoolValue (Constants.Parameters.WRITE_TO_CONSOLE)) {
            // _logger.Info($"Test write command executed, line: {_write}.");
            foreach (CheckResult result in results) {
               Console.WriteLine ($"Check result: {result}") ;
            }
         }

         return results ;
      }

      protected virtual string GetConfigurationFileName() {
         var configurationFileName = _parameters.GetProperty<string> (Constants.Parameters.CONFIGURATION_FILE,
                                                                      Constants.Parameters.CONFIGURATION_FILE_OBSOLETE) ;

         return FilesEx.EnsureFileNameHasFullPath (configurationFileName) ;
      }


      protected virtual List<CheckResult> Run1() {
         // Default: no check
         return new List<CheckResult> {new CheckResult.InvalidParameters()} ;
      }
   }
}
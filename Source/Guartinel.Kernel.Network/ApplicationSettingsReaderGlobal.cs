using System;
using System.Linq;
using System.Text;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel.Network {
   public class ApplicationSettingsReaderGlobal : IApplicationSettingsReader {
      public ApplicationSettingsReaderGlobal (string componentName) {
         _componentName = componentName.CheckNullOrEmpty ("Configuration component name not specified.") ;
      }

      protected readonly string _componentName ;

      public JObject ReadConfigurationObject() {
         var result = GlobalConfiguration.Use.Read (GetSettingsKey().CheckNullOrEmpty ("Configuration key is empty."), GetSettingsToken()) ;
         return result ;
      }

      public JObject GetConfigurationInfo() {
         JObject result = new JObject() ;
         result ["key"] = GetSettingsKey() ;
         result ["token"] = $"{GetSettingsToken().Substring (0, 5)}xxxxx." ;

         return result ;
      }

      public void SubscribeForChange (int refreshIntervalSeconds,
                                      Action notification) {
         GlobalConfiguration.Use.SubscribeForChange (GetSettingsKey(), GetSettingsToken(), refreshIntervalSeconds, notification) ;
      }

      public const string KEY_VARIABLE_NAME = "GUARTINEL_CONFIGURATION_NAME" ;
      protected string DefaultConfigurationName => "Test" ;

      private string GetSettingsKey() {
         // Get configuration name from an environment variable
         var configurationName = Environment.GetEnvironmentVariable (KEY_VARIABLE_NAME) ;
         if (string.IsNullOrEmpty (configurationName)) {
            configurationName = DefaultConfigurationName ;
         }

         return $"Guartinel/{_componentName}/{configurationName}" ;
      }

      public const string TOKEN_VARIABLE_NAME = "GUARTINEL_CONFIGURATION_TOKEN" ;
      protected string DefaultConfigurationToken => "5cc78182-4353-4b6d-a479-55e663b97e4f" ;

      private string GetSettingsToken() {
         // Get configuration name from an environment variable
         var configurationToken = Environment.GetEnvironmentVariable (TOKEN_VARIABLE_NAME) ;
         if (string.IsNullOrEmpty (configurationToken)) {
            return DefaultConfigurationToken ;
         }

         return configurationToken ;
      }
   }
}
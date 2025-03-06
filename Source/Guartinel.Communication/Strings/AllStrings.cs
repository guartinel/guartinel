using System ;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.Communication.Languages ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Communication.Strings {
   public class AllStrings {
      private readonly List<StringsBase> _stringsList = new List<StringsBase>() ;

      public AllStrings() {
         _stringsList.Add(Strings.Use);
         _stringsList.Add (Supervisors.HostSupervisor.Strings.Use) ;
         _stringsList.Add (Supervisors.WebsiteSupervisor.Strings.Use) ;
         _stringsList.Add (Supervisors.ApplicationSupervisor.Strings.Use) ;
         _stringsList.Add(Supervisors.EmailSupervisor.Strings.Use);
         _stringsList.Add(Supervisors.HardwareSupervisor.Strings.Use);
      }

      public Dictionary<string, AllPackageTypeValues> GetAllPackageTypeValues() {
         Dictionary<string, AllPackageTypeValues> result = new Dictionary<string, AllPackageTypeValues>() ;
         result.Add ("ALL_PACKAGE_TYPE_VALUES", new AllPackageTypeValues()) ;
         return result ;
      }

      public string ConstantsToJSON() {
         JObject result = new JObject();

         foreach (StringsBase plugin in _stringsList) {
            foreach (KeyValuePair<string, string> keyValuePair in plugin.GetProperties()) {
               result [keyValuePair.Key] = keyValuePair.Value ;
            }
         }
         return result.ToString(Formatting.None) ;
      }

      public string MessagesToJSON() {
         JObject result = new JObject() ;

         foreach (StringsBase strings in _stringsList) {
            LanguageTable languages = strings.GetLanguages() ;

            foreach (KeyValuePair<string, MessageTable> language in languages.Languages) {
               JObject languageObject = (JObject) result [language.Key] ;
               if (languageObject == null) {
                  languageObject = new JObject();
                  result [language.Key] = languageObject ;
               }

               foreach (KeyValuePair<string, string> message in language.Value.Messages) {
                  languageObject [strings.Prefix + "." + message.Key] = message.Value ;
               }               
            }
         }

         return result.ToString (Formatting.None) ;
      }

      public AllPackageTypeValues ALL_PACKAGE_TYPE_VALUES {get {return new AllPackageTypeValues() ;}}

      public class AllPackageTypeValues {
         [JsonProperty]
         public readonly string EMAIL_SUPERVISOR = Supervisors.EmailSupervisor.Strings.Use.PackageType;

         [JsonProperty]
         public readonly string HOST_SUPERVISOR = Supervisors.HostSupervisor.Strings.Use.PackageType;

         [JsonProperty]
         public readonly string WEBSITE_SUPERVISOR = Supervisors.WebsiteSupervisor.Strings.Use.PackageType;

         [JsonProperty]
         public readonly string APPLICATION_SUPERVISOR = Supervisors.ApplicationSupervisor.Strings.Use.PackageType;
         [JsonProperty]
         public readonly string HARDWARE_SUPERVISOR = Supervisors.HardwareSupervisor.Strings.Use.PackageType;
      }
   }
}
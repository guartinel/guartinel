using System.Collections.Generic ;
using Guartinel.Communication.Plugins ;

namespace Guartinel.Communication.Messages {
   public class LanguageTable {
      // Key: language
      // Value: name and message pairs
      private readonly Dictionary<string, Dictionary<string, string>> _languages = new Dictionary<string, Dictionary<string, string>>() ;

      protected void Add (MessagesOnLanguage language) {
         _languages.Add (language.Language, language.Messages()) ;
      }

      public Dictionary<string, Dictionary<string, string>> Languages() {
         return _languages ;
      }

      public LanguageTable (Plugins.Plugins plugins) {
         Add (new EnglishMessages()) ;

         foreach (Plugin plugin in plugins.PLUGINS) {
            Dictionary<string, Dictionary<string, string>> pluginMessages = plugin.GetLanguages() ;

            foreach (KeyValuePair<string, Dictionary<string, string>> language in pluginMessages) {
               foreach (KeyValuePair<string, string> languageElement in language.Value) {
                   _languages [language.Key].Add (plugin.PackageType + "." + languageElement.Key,languageElement.Value) ;
               }
            }
         }
      }
   }
}

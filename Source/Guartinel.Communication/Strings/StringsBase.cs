using System ;
using System.Collections.Generic ;
using Guartinel.Communication.Languages ;

namespace Guartinel.Communication.Strings {
   public abstract class StringsBase {
      public abstract string Prefix {get ;}
      public string Get (string value) {
         return $"{Prefix}.{value}" ;
      }

      public abstract Dictionary<string, string> GetProperties() ;

      protected readonly LanguageTable _languages = new LanguageTable() ;

      public LanguageTable GetLanguages() {
         return _languages ;
      }
   }
}

using System.Collections.Generic ;
using Guartinel.Communication.Languages ;

namespace Guartinel.Communication.Languages {
   public abstract class LanguageListBase {

      protected readonly LanguageTable _languages = new LanguageTable() ;

      public LanguageTable GetAllLanguages () {
         return _languages ;
      }
   }
}
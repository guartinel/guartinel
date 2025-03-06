using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guartinel.Communication.Supervisors ;
using Newtonsoft.Json ;

namespace Guartinel.Communication.Languages {
   public class LanguageTable {
      private readonly Dictionary<string, MessageTable> _languages = new Dictionary<string, MessageTable>() ;

      public Dictionary<string, MessageTable> Languages => _languages ;

      public void Add (LanguageBase language) {
         _languages.Add (language.Name, language.GetEntries()) ;
      }
   }
}
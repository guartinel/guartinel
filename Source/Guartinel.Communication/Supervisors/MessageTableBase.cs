using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guartinel.Communication.Languages ;

namespace Guartinel.Communication.Plugins {
   public abstract class MessageTableBase {
      // Key: language name
      // Value: name and string pairs   
      protected readonly Dictionary<string, Dictionary<string, string>> _languages = new Dictionary<string, Dictionary<string, string>>() ;

      protected void Add (LanguageBase language) {
         _languages.Add (language.Language, language.GetMessages()) ;
      }

      public Dictionary<string, Dictionary<string, string>> GetAllMessages() {
         return _languages ;
      }
   }
}
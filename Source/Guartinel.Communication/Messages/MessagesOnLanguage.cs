using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reflection ;

namespace Guartinel.Communication.Messages {
   public abstract class MessagesOnLanguage {
      protected string Parameter (string name) {
         return $"#{name}#" ;
      }

      public abstract string Language {get ;}

      // Key: name
      // Value: message
      protected Dictionary<string, string> _messages = new Dictionary<string, string>() ;

      public Dictionary<string, string> Messages() {
         return _messages ;
      }

      protected MessagesOnLanguage() {
         // Use reflection to get values
         FieldInfo[] fields = GetType().GetFields() ;
         foreach (FieldInfo field in fields) {
            _messages.Add (field.Name, (string) field.GetValue (this)) ;
         }

         PropertyInfo[] properties = GetType().GetProperties() ;
         foreach (PropertyInfo property in properties) {
            _messages.Add (property.Name, (string) property.GetValue (this)) ;
         }
      }
   }

   public abstract class EnglishMessagesBase : MessagesOnLanguage {
      public override string Language => "ENGLISH" ;
   }
}

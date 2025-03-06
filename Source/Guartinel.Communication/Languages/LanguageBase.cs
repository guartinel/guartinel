using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reflection ;
using System.Text ;

namespace Guartinel.Communication.Languages {
   public abstract class LanguageBase {
      protected string Parameter (string name) {
         return $"#{name}#" ;
      }

      public abstract string Name {get ;}

      protected MessageTable _entries = new MessageTable() ;

      public MessageTable GetEntries() {
         return _entries ;
      }

      protected LanguageBase() {
         FieldInfo[] fields = GetType().GetFields() ;
         foreach (FieldInfo field in fields) {
            if (field.DeclaringType == GetType()) {
               _entries.Messages.Add (field.Name, (string) field.GetValue (this)) ;
            }
         }

         PropertyInfo[] properties = GetType().GetProperties() ;
            foreach (PropertyInfo property in properties) {
               if (property.DeclaringType == GetType()) {
                  _entries.Messages.Add (property.Name, (string) property.GetValue (this)) ;
               }
            }
         }
      }

      public abstract class EnglishBase : LanguageBase {
      public override string Name => "ENGLISH" ;
   }
}
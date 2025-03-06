using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reflection ;
using System.Text ;


namespace Guartinel.Communication {
   public static class Helper {
      public static Dictionary<string, string> ObjectToDictionary (object customObject) {
         Dictionary<string, string> result = new Dictionary<string, string>() ;
         FieldInfo[] fieldProperties = customObject.GetType().GetFields() ;
         foreach (FieldInfo field in fieldProperties) {
            result.Add (field.Name, (string) field.GetValue (customObject)) ;
         }
       
         return result ;
      }
   }
}

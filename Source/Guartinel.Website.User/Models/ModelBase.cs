using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Website.User.Models {
   public class ModelBase {
      public new string ToString() {
         var properties = this.GetType().GetProperties() ;
         StringBuilder stringBuilder = new StringBuilder() ;
         foreach (var property in properties) {
            stringBuilder.AppendLine (property.Name + ": " + property.GetValue (this, null)) ;
         }
         return stringBuilder.ToString() ;
      }
   }
}

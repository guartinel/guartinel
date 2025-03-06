using System;
using System.Collections.Generic ;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel.Utility {
   public static class EnumEx {
      /// <summary>
      /// Return with the string representation of the enum.
      /// </summary>
      /// <param name="enumType"> </param>
      /// <param name="value"></param>
      /// <returns></returns>
      public static string GetName (Type enumType,
                                    Enum value) {
         // Check if enum
         if (!enumType.IsEnum) throw new ArgumentException() ;

         return Enum.GetName (enumType, value) ;
      }

      /// <summary>
      /// Parse enum string value to the enum value itself.
      /// </summary>
      /// <param name="value"></param>
      /// <param name="defaultValue"></param>
      /// <returns></returns>
      public static T Parse<T> (string value,
                                T defaultValue) {
         // Check if enum
         if (!typeof (T).IsEnum) throw new ArgumentException() ;

         if (string.IsNullOrEmpty(value)) return defaultValue ;

         foreach (object item in Enum.GetValues (typeof (T))) {
            if (item.ToString().ToLower().Equals(value.Trim().ToLower())) return (T) item ;
         }

         return defaultValue ;
      }

      public static bool In<T> (this T value, params T[] values) where T : struct {
         return values.Contains (value) ;
      }

      public static IEnumerable<T> GetValues<T>() {
         return Enum.GetValues (typeof(T)).Cast<T>() ;
      }
   }
}
using System;
using System.Linq;
using System.Text;
using Guartinel.Kernel.Logging ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel.Utility {
   public static class JSONEx {
      public static string NameToJSONName (this string name) {
         return name.SeparateCamelCase("_")?.ToLowerInvariant() ;
      }

      public static string GetStringValue (this JObject jobject,
                                           string name,
                                           string defaultValue = "") {
         if (jobject == null) return defaultValue ;

         JToken value ;

         if (!jobject.TryGetValue (name, out value)) {
            return defaultValue ;
         }

         if (value == null) return string.Empty ;

         if (value is JValue) {
            var jValue = ((JValue) value).Value ;
            if (jValue == null) return string.Empty ;
            if (jValue is string) return value.Value<string>() ;

            return jValue.ToString() ;
         }

         return value.ToString (Formatting.None) ;
      }

      private static T GetValue<T> (JObject jobject,
                                    string name,
                                    Func<JToken, T> cast,
                                    T defaultValue) {
         cast.CheckNull(nameof (cast)) ;

         if (!jobject.TryGetValue(name, out var value)) {
            return defaultValue ;
         }

         try {
            return cast.Invoke (value) ;
         } catch {
            return defaultValue;
         }
      }

      public static int GetIntegerValue (this JObject jobject,
                                         string name,
                                         int defaultValue) {

         return GetValue (jobject, name, value => (int) value, defaultValue) ;
      }

      public static int? GetIntegerValueNull (this JObject jobject,
                                              string name) {

         return GetValue (jobject, name, value => (int) value, (int?) null) ;
      }

      public static double GetDoubleValue (this JObject jobject,
                                           string name,
                                           double defaultValue) {

         return GetValue(jobject, name, value => (double) value, defaultValue);
      }

      public static bool GetBooleanValue (this JObject jobject,
                                          string name,
                                          bool defaultValue) {
         return GetValue(jobject, name, value => (bool) value, defaultValue);
      }

      public static DateTime GetDateTimeValue (this JObject jobject,
                                               string name,
                                               DateTime defaultValue) {

         return GetValue(jobject, name, value => (DateTime) value, defaultValue);
      }

      public static T GetProperty<T> (this JObject jobject,
                                      params string[] propertyNames) {
         foreach (var propertyName in propertyNames) {
            JToken result = jobject [propertyName] ;
            if (result == null) return default(T) ;

            T x = result.ToObject<T>() ;
            if (x != null) return x ;
         }

         return default(T) ;
      }

      public static string[] AsStringArray (this JObject jobject,
                                            string name,
                                            string[] defaultValue = null) {
         string[] defaultResult = defaultValue ?? new string[] { } ;
         try {
            if (jobject == null) return defaultResult;
            if (jobject [name] == null) return defaultResult;
            if (!(jobject [name] is JArray)) return defaultResult;

            return ((JArray) jobject [name]).Select (value => value.ToString()).ToArray() ;
         } catch {
            return defaultResult ;
         }
      }

      public static void SetStringArray (this JObject jobject,
                                         string name,
                                         string[] values) {
         if (values == null) {
            jobject [name] = string.Empty ;
            return ;
         }

         JArray array = new JArray (values.Select (x => (object) x)) ;
         jobject [name] =  array ;
      }

      public static string ConvertToLog (this JObject jobject,
                                         int? propertyValueLimit = null) {
         return Logger.ConvertToLog (jobject, propertyValueLimit) ;
      }
   }
}
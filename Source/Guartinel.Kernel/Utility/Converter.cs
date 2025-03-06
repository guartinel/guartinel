using System;
using System.Collections.Generic ;
using System.Globalization ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel.Utility {
   public static class Converter {
      public static class Constants {
         public static readonly List<string> TRUE_VALUES = new List<string> {"true", "yes", "y"} ;
         public static readonly List<string> FALSE_VALUES = new List<string> {"false", "no", "n"} ;

         public const string INVARIANT_DATE_TIME_FORMAT = @"yyyy/MM/dd HH:mm:ss.fff" ;

         public const int TIME_ROUNDING = 2;
      }

      /// <summary>
      /// Converts string to boolean, if error occures, returns default value
      /// </summary>
      /// <param name="value">What to convert</param>
      /// <param name="defaultValue">Use this value if cannot convert</param>
      /// <returns>integer value</returns>
      public static bool StringToBool (string value,
                                       bool defaultValue) {
         try {
            if (string.IsNullOrEmpty (value)) {
               return defaultValue ;
            } else {
               if (Constants.TRUE_VALUES.Contains (value.ToLower())) {
                  return true ;
               }

               if (Constants.FALSE_VALUES.Contains (value.ToLower())) {
                  return false ;
               }

               return defaultValue ;
            }
         } catch (System.Exception) {
            return defaultValue ;
         }
      }

      /// <summary>
      /// Converts boolean to string by invariant culture.
      /// </summary>
      /// <param name="value">What to convert</param>
      /// <returns>string value</returns>
      public static string BoolToString (bool value) {
         return value ? Constants.TRUE_VALUES [0] : Constants.FALSE_VALUES [0] ;
      }

      /// <summary>
      /// Converts string to integer, if error occures, returns default value
      /// </summary>
      /// <param name="value">What to convert</param>
      /// <param name="defaultValue">Use this value if cannot convert</param>
      /// <returns>integer value</returns>
      public static int StringToInt (string value,
                                     int defaultValue) {
         int? result = StringToIntNull (value) ;
         if (result == null) return defaultValue ;

         return result.Value ;
      }

      /// <summary>
      /// Converts string to integer, if error occures, returns null
      /// </summary>
      /// <param name="value">What to convert</param>
      /// <returns>integer value</returns>
      public static int? StringToIntNull (string value) {
         try {
            if (string.IsNullOrEmpty (value)) {
               return null ;
            }

            return Int32.Parse (value) ;
         } catch (System.Exception) {
            return null ;
         }
      }

      /// <summary>
      /// Converts integer to string by invariant culture.
      /// </summary>
      /// <param name="value">What to convert</param>
      /// <param name="defaultValue">Default value if the conversion is not possible. Passing null measn that the function raises an exception.</param>
      /// <returns>string value</returns>
      public static string IntToString (int value,
                                        string defaultValue = null) {
         try {
            return value.ToString (CultureInfo.InvariantCulture) ;
         } catch {
            if (defaultValue == null) throw ;
            return defaultValue ;
         }
      }

      /// <summary>
      /// Converts string to double, if error occures, returns default value
      /// </summary>
      /// <param name="value">What to convert</param>
      /// <param name="defaultValue">Use this value if cannot convert</param>
      /// <returns>integer value</returns>
      public static double StringToDouble (string value,
                                           double defaultValue) {
         double? result = StringToDoubleNull (value) ;
         if (result == null) return defaultValue ;

         return result.Value ;
      }

      /// <summary>
      /// Converts string to double, if error occures, returns null
      /// </summary>
      /// <param name="value">What to convert</param>
      /// <returns>integer value</returns>
      public static double? StringToDoubleNull (string value) {
         try {
            if (string.IsNullOrEmpty (value)) {
               return null ;
            }

            return Double.Parse (value) ;
         } catch {
            return null ;
         }
      }

      /// <summary>
      /// Convert date and time to string using invaliant culture.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public static string DateTimeToString (DateTime value) {
         return value.ToString (Constants.INVARIANT_DATE_TIME_FORMAT, CultureInfo.InvariantCulture) ;
      }

      /// <summary>
      /// Convert string to date and time using invariant culture.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public static DateTime StringToDateTime (string value) {
         if (string.IsNullOrEmpty (value)) return new DateTime() ;

         return DateTime.ParseExact (value, Constants.INVARIANT_DATE_TIME_FORMAT, CultureInfo.InvariantCulture) ;
      }

      /// <summary>
      /// Convert string to date and time using invariant culture. Can use Zulu time!
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public static TimeSpan StringToTimeSpan (string value) {
         if (string.IsNullOrEmpty (value)) return new TimeSpan() ;

         // return TimeSpan.ParseExact (value, Constants.INVARIANT_DATE_TIME_FORMAT, CultureInfo.InvariantCulture) ;
         return DateTime.Parse (value).ToUniversalTime().TimeOfDay ;
      }

      /// <summary>
      /// Convert date and time to string using Json format.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public static string DateTimeToStringJson (DateTime value) {
         return DateTimeToJsonDateTime (value).ToString ("o") ;
      }

      public static DateTime DateTimeToJsonDateTime (DateTime value) {
         return DateTime.SpecifyKind (value.TruncateMilliSeconds(), DateTimeKind.Utc) ;
      }

      public class JsonDate {
         [JsonProperty]
         public DateTime DateTimeValue {get ; set ;}
      }

      public class JsonDateString {
         public JsonDateString (string dateTimeValue) {
            DateTimeValue = dateTimeValue ;
         }

         [JsonProperty]
         public string DateTimeValue {get ; set ;}
      }

      /// <summary>
      /// Convert string to date and time using Json format.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public static DateTime StringToDateTimeJson (string value) {
         if (string.IsNullOrEmpty (value)) return new DateTime() ;

         // string objectString = @"{""DateTimeValue"": " + value + @"}" ;
         // return JsonConvert.DeserializeObject<JsonDate> (objectString, new IsoDateTimeConverter()).DateTimeValue ;

         // return DateTimeToJsonDateTime (value).ToString ("o") ;         

         try {
            return DateTime.Parse (value, null, DateTimeStyles.RoundtripKind) ;
         } catch {
            return new DateTime() ;
         }
      }

      public static bool GetBoolValue (this JObject jobject,
                                       string propertyName) {
         if (jobject == null) return false ;

         var value = jobject [propertyName] ;
         if (value == null) return false ;

         try {
            return (bool) value ;
         } catch {
            return false ;
         }
      }

      public static JArray CreateJArray (ICollection<string> values) {
         var result = new JArray() ;

         foreach (var value in values) {
            result.Add (value) ;
         }

         return result ;
      }

      public static double NormalizeValue (this double value) {
         return Math.Round (value, 2) ;
      }

      public static int NormalizePercent (this double value) {
         return (int) Math.Round (value, 0) ;
      }

      public static int Percent (double value,
                                 double whole) {
         return (int) Math.Round (value / whole * 100, 0) ;
      }
   }
}
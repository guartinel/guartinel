// **********************************************************************
// **  <D3CE0057-C733-4511-9D41-6F1FD43AEB32>                          **
// **                                                                  **
// **  (C) Copyright by Sysment Ltd, Hungary. All rights reserved.     **
// **  No portion of this software may be reproduced, transmitted,     **
// **  transcribed, stored in a retrieval system, or translated into   **
// **  any computer language, in any form or by any means, electronic, **
// **  magnetic, optical, manual, or otherwise except as permitted     **
// **  in writing by Sysment Ltd, Hungary, www.sysment.com             **
// **                                                                  **
// **  </D3CE0057-C733-4511-9D41-6F1FD43AEB32>                         **
// **********************************************************************
// **  Project:          Guartinel Core
// **  Module Desc:      String-related utilities.
// **  Author:           SZTZ
// **********************************************************************

using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Globalization ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel.Utility {
   /// <summary>
   /// String extensions.
   /// </summary>
   public static class StringEx {
      public static class Constants {
         public const string SPACE = " " ;
      }

      /// <summary>
      /// SubString enhancement: do not throw an exception when the length of the string is less than the
      /// specified limit, just return the remaining characters.
      /// </summary>
      /// <param name="s"></param>
      /// <param name="startIndex"></param>
      /// <param name="length"></param>
      /// <returns></returns>
      public static string SubstringEx (this string s,
                                        int startIndex,
                                        int length) {
         if (string.IsNullOrEmpty (s)) {
            return String.Empty ;
         }

         return s.Substring (startIndex, Math.Min (s.Length - startIndex, length)) ;
      }

      /// <summary>
      /// Get plural for of a text.
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public static string Plural (this string text) {
         if (string.IsNullOrEmpty (text)) return String.Empty ;
         return $"{text}(s)" ;

#warning SzTZ: need a new algorithm: does not work well for e.g. "day"

         //string result = text ;

         //string lastCharacter = result.Substring (result.Length - 1).ToLower() ;

         //// y's become ies (such as Category to Categories)
         //if (string.Equals (lastCharacter, "y", StringComparison.InvariantCultureIgnoreCase)) {
         //   result = result.Remove (result.Length - 1) ;

         //   result += "ie" ;
         //}

         //// ch's become ches (such as Pirch to Pirches)
         //if (string.Equals (result.Substring (result.Length - 2), "ch", StringComparison.InvariantCultureIgnoreCase)) {
         //   result += "e" ;
         //}

         //switch (lastCharacter) {
         //   case "s":
         //      return result + "es" ;

         //   default:
         //      return result + "s" ;
         //}
      }

      /// <summary>
      /// Convert string to uppercase. Make a check for null.
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public static string ToUpperCheckNull (this string text) {
         if (text == null) {
            return null ;
         }
         return text.ToUpper() ;
      }

      /// <summary>
      /// Convert string to lowercase. Make a check for null.
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public static string ToLowerCheckNull (this string text) {
         if (text == null) {
            return null ;
         }
         return text.ToLower() ;
      }

      /// <summary>
      /// Convert string to uppercase and use underscores at the new words.
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public static string ToUpperUnderscore (this string text) {
         if (text == null) {
            return null ;
         }

         string result = "" ;
         bool lastWasLower = false ;

         foreach (var character in text) {
            if (char.IsUpper (character)) {
               if (lastWasLower) {
                  // Add an underscore
                  result += "_" ;
               }
               lastWasLower = false ;
            } else {
               lastWasLower = true ;
            }

            // Add character
            result += character ;
         }

         return result.ToUpper() ;
      }

      /// <summary>
      /// Converts the camel-case string to separate words.
      /// </summary>
      /// <param name="phrase">The phrase to convert.</param>
      /// <param name="separator">Separator for splitted result.</param>
      /// <returns>Returns the converted string.</returns>
      public static string SeparateCamelCase (this string phrase,
                                              string separator = Constants.SPACE) {
         if (phrase == null) return null ;
         if (phrase == string.Empty) return string.Empty ;

         StringBuilder outputString = new StringBuilder() ;
         char[] phraseChars = phrase.ToCharArray() ;
         if (phraseChars.Length == 0) return outputString.ToString() ;
         bool isFirstChar = true ;

         foreach (var phraseChar in phraseChars) {
            // Capital letter?
            if (!isFirstChar && phraseChar.ToString().ToUpperInvariant() == phraseChar.ToString()) {
               outputString.Append (separator) ;
            }

            outputString.Append (phraseChar) ;
            isFirstChar = false ;
         }

         return outputString.ToString() ;
      }

      /// <summary>
         /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
         /// </summary>
         /// <param name="characters">Unicode Byte Array to be converted to String</param>
         /// <returns>String converted from Unicode Byte Array</returns>
         public static string UTF8ByteArrayToString (Byte[] characters) {
         UTF8Encoding encoding = new UTF8Encoding (false) ;
         // UnicodeEncoding encoding = new UnicodeEncoding() ;

         String constructedString = encoding.GetString (characters) ;

         return (constructedString) ;
      }

      /// <summary>
      /// Converts the String to UTF8 Byte array and is used in De serialization
      /// </summary>
      /// <param name="s"></param>
      /// <returns></returns>
      public static Byte[] StringToUTF8ByteArray (string s) {
         UTF8Encoding encoding = new UTF8Encoding (false) ;

         Byte[] byteArray = encoding.GetBytes (s) ;

         return byteArray ;
      }

      /// <summary>
      /// Add strings to each other using a separator.
      /// </summary>
      /// <param name="items"></param>
      /// <param name="separator"></param>
      /// <returns></returns>
      public static string Concat (this IEnumerable<string> items,
                                   string separator) {
         StringBuilder result = new StringBuilder() ;
         foreach (string item in items) {
            if (result.Length > 0) {
               result.Append (separator) ;
            }
            result.Append (item) ;
         }
         return result.ToString() ;
      }

      /// <summary>
      /// Splits a string using a single string as separator.
      /// </summary>
      /// <param name="source">Source string to split.</param>
      /// <param name="separator">Separator string.</param>
      /// <param name="options">Slip options.</param>
      /// <returns></returns>
      public static string[] Split (this string source,
                                    string separator,
                                    StringSplitOptions options = StringSplitOptions.None) {
         return source.Split(new[] { separator }, options);
      }

      /// <summary>
      /// String comparer for ordering operations.
      /// </summary>
      public class CaseInsensitiveComparer : IComparer<string> {
         /// <summary>
         /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
         /// </summary>
         /// <returns>
         /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
         /// </returns>
         /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
         public int Compare (string x,
                             string y) {
            return string.Compare (x, y, StringComparison.OrdinalIgnoreCase) ;
         }
      }

      /// <summary>
      /// Check null or empty argument and raise exception if necessary.
      /// </summary>
      /// <param name="value">value to check</param>
      /// <param name="message">exception message if it is rather null</param>
      /// <returns></returns>
      public static string CheckNullOrEmpty (this string value,
                                             string message) {
         if (string.IsNullOrEmpty (value)) {
            throw new CoreException (message) ;
         }
         return value ;
      }

      /// <summary>
      /// Check null or empty argument and raise exception if necessary.
      /// </summary>
      /// <param name="value">value to check</param>
      /// <returns></returns>
      public static string CheckNullOrEmpty (this string value) {
         return CheckNullOrEmpty (value, "Value or parameter cannot be null.") ;
      }

      /// <summary>
      /// Converts string to the specified value.
      /// </summary>
      /// <typeparam name="T">Type of the returned value.</typeparam>
      /// <param name="value">The string itself.</param>
      /// <returns>Returns a type-specific value.</returns>
      public static T To<T> (this string value) where T : struct {
         return To<T> (value, CultureInfo.InvariantCulture) ;
      }

      /// <summary>
      /// Converts string to the specified value.
      /// </summary>
      /// <typeparam name="T">Type of the returned value.</typeparam>
      /// <param name="value">The string itself.</param>
      /// <param name="cultureInfo">The current culture info.</param>
      /// <returns>Returns a type-specific value.</returns>
      public static T To<T> (this string value,
                             CultureInfo cultureInfo) where T : struct {
         try {
            value = value.Trim() ;

            TypeConverter converter = TypeDescriptor.GetConverter (typeof (T)) ;

            // KOGE: This was the original solution.
            //return (T)converter.ConvertFrom(null, cultureInfo, value);

            // KOGE: This is another solution but is doesn't work properly.
            //return (T)Convert.ChangeType(value, typeof(T), cultureInfo);

            // KOGE: This solution comes from the CodeProject site:
            // http://www.codeproject.com/KB/string/stringconversion.aspx
            // Nasty but needed because the BCL is not turning on the right NumberStyle flags.
            if (converter is BaseNumberConverter) {
               return (T) HandleThousandsSeparatorIssue<T> (converter, value, cultureInfo) ;
            }
            if (converter is BooleanConverter) {
               return (T) HandleBooleanValues (converter, value, cultureInfo) ;
            } else {
               return (T) converter.ConvertFromString (null, cultureInfo, value) ;
            }
         } catch (System.Exception ex) {
            throw new CoreException (ex, "Error when converting value '{0}' to '{1}'.", value, typeof (T).FullName) ;
         }
      }

      /// <summary>
      /// The BooleanTypeConverter is only able to handle True and False strings. With this function we are able to
      /// handle other frequently used values as well eg. Yes, No, On, Off, 0 and 1.
      /// </summary>
      /// <param name="converter">If the provided string cannot be translated, we'll hand it back to the type converter.</param>
      /// <param name="value">The string value which represents a boolean value.</param>
      /// <param name="cultureInfo">The culture info.</param>
      private static object HandleBooleanValues (TypeConverter converter,
                                                 string value,
                                                 CultureInfo cultureInfo) {
         string[] trueValues = {"true", "yes", "y", "on", "1"} ;
         string[] falseValues = {"false", "no", "n", "off", "0"} ;

         if (!string.IsNullOrEmpty (value)) {
            if (trueValues.Contains (value.ToLower())) {
               return true ;
            }

            if (falseValues.Contains (value.ToLower())) {
               return false ;
            }
         }

         return converter.ConvertFromString (null, cultureInfo, value) ;
      }

      /// <remarks>
      /// The Parse methods on the individual number classes are much smarter then their type converter 
      /// counterparts. See: http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/c980b925-6df5-428d-bf87-7ff83db4504c/
      /// </remarks>
      private static object HandleThousandsSeparatorIssue<T> (TypeConverter converter,
                                                              string value,
                                                              CultureInfo cultureInfo) {
         NumberStyles styles = NumberStyles.Number ;
         IFormatProvider format = cultureInfo == null ? null : cultureInfo.NumberFormat ;

         #region Double
         if (typeof (T).Equals (typeof (double))) {
            if (HasValidThousandSeparator (value, cultureInfo)) {
               return double.Parse (value, styles, format) ;
            }
         }
         #endregion

         #region Decimal
         if (typeof (T).Equals (typeof (decimal))) {
            if (HasValidThousandSeparator (value, cultureInfo)) {
               return decimal.Parse (value, styles, format) ;
            }
         }
         #endregion

         #region Single
         if (typeof (T).Equals (typeof (Single))) {
            if (HasValidThousandSeparator (value, cultureInfo)) {
               return Single.Parse (value, styles, format) ;
            }
         }
         #endregion

         #region Int16 & UInt16
         if (typeof (T).Equals (typeof (Int16))) {
            return Int16.Parse (value, styles, format) ;
         }

         if (typeof (T).Equals (typeof (UInt16))) {
            return UInt16.Parse (value, styles, format) ;
         }
         #endregion

         #region Int32 & UInt32
         if (typeof (T).Equals (typeof (Int32))) {
            return Int32.Parse (value, styles ^ NumberStyles.AllowDecimalPoint, format) ;
         }

         if (typeof (T).Equals (typeof (UInt32))) {
            return UInt32.Parse (value, styles ^ NumberStyles.AllowDecimalPoint, format) ;
         }
         #endregion

         #region Int64 & UInt64
         if (typeof (T).Equals (typeof (Int64))) {
            return Int64.Parse (value, styles ^ NumberStyles.AllowDecimalPoint, format) ;
         }

         if (typeof (T).Equals (typeof (UInt64))) {
            return UInt64.Parse (value, styles ^ NumberStyles.AllowDecimalPoint, format) ;
         }
         #endregion

         // Last chance. Fallback on the TypeConverters if we haven't choosen to provide a different 
         // method to parse the string.
         return (T) converter.ConvertFromString (null, cultureInfo, value) ;
      }

      /// <summary>
      /// Checks whether the thousand separator is a right position.
      /// It is a workaround for the .Net bug: the string "12,34" converting to double 
      /// gives a valid number (1234) in English culture because "," works as the thousand separator 
      /// in this case.
      /// </summary>
      /// <param name="value">The string-format value.</param>
      /// <param name="cultureInfo">The current culture info.</param>
      /// <returns>Returns true if minimum 3 digits follow the thousand separator.</returns>
      private static bool HasValidThousandSeparator (string value,
                                                     CultureInfo cultureInfo) {
         if (string.IsNullOrEmpty (value) ||
             cultureInfo == null) {
            return true ;
         }

         if (value.Contains (cultureInfo.NumberFormat.NumberGroupSeparator)) {
            int separatorIndex = value.IndexOf (cultureInfo.NumberFormat.NumberGroupSeparator) ;
            if (value.Substring (separatorIndex, value.Length - separatorIndex - 1).Length >= 3) {
               return true ;
            } else {
               return false ;
            }
         }

         return true ;
      }

      /// <summary>
      /// Tries to parse the string to the specified value.
      /// </summary>
      /// <typeparam name="T">Type of the returned value.</typeparam>
      /// <param name="value">The string itself.</param>
      /// <param name="output">The type-specific value.</param>
      /// <returns>Returns true if the conversion succeeded, otherwise returns false.</returns>
      public static bool TryParse<T> (this string value,
                                      out T output) where T : struct {
         return TryParse<T> (value, CultureInfo.InvariantCulture, out output) ;
      }

      /// <summary>
      /// Tries to parse the string to the specified value.
      /// </summary>
      /// <typeparam name="T">Type of the returned value.</typeparam>
      /// <param name="value">The string itself.</param>
      /// <param name="cultureInfo">The current culture info.</param>
      /// <param name="output">The type-specific value.</param>
      /// <returns>Returns true if the conversion succeeded, otherwise returns false.</returns>
      public static bool TryParse<T> (this string value,
                                      CultureInfo cultureInfo,
                                      out T output) where T : struct {
         try {
            output = value.To<T> (cultureInfo) ;
            return true ;
         } catch {
            output = default(T) ;
            return false ;
         }
      }

      /// <summary>
      /// Converts string to the specified nullable value.
      /// </summary>
      /// <typeparam name="T">Type of the returned nullable value.</typeparam>
      /// <param name="value">The string itself.</param>
      /// <returns>Returns a type-specific value.</returns>
      public static T? ToNullable<T> (this string value) where T : struct {
         return ToNullable<T> (value, CultureInfo.InvariantCulture) ;
      }

      /// <summary>
      /// Converts string to the specified nullable value.
      /// </summary>
      /// <typeparam name="T">Type of the returned nullable value.</typeparam>
      /// <param name="value">The string itself.</param>
      /// <param name="cultureInfo">The current culture info.</param>
      /// <returns>Returns a type-specific value.</returns>
      public static T? ToNullable<T> (this string value,
                                      CultureInfo cultureInfo) where T : struct {
         if (string.IsNullOrEmpty (value)) {
            return null ;
         }

         return value.To<T> (cultureInfo) ;
      }

      /// <summary>
      /// Tries to parse the string to the specified nullable value.
      /// </summary>
      /// <typeparam name="T">Type of the returned nullable value.</typeparam>
      /// <param name="value">The string itself.</param>
      /// <param name="output">The type-specific value.</param>
      /// <returns>Returns true if the conversion succeeded, otherwise returns false.</returns>
      public static bool TryParseNullable<T> (this string value,
                                              out T? output) where T : struct {
         return TryParseNullable<T> (value, CultureInfo.InvariantCulture, out output) ;
      }

      /// <summary>
      /// Tries to parse the string to the specified nullable value.
      /// </summary>
      /// <typeparam name="T">Type of the returned nullable value.</typeparam>
      /// <param name="value">The string itself.</param>
      /// <param name="cultureInfo">The current culture info.</param>
      /// <param name="output">The type-specific value.</param>
      /// <returns>Returns true if the conversion succeeded, otherwise returns false.</returns>
      public static bool TryParseNullable<T> (this string value,
                                              CultureInfo cultureInfo,
                                              out T? output) where T : struct {
         try {
            output = value.ToNullable<T> (cultureInfo) ;
            return true ;
         } catch {
            output = null ;
            return false ;
         }
      }

      /// <summary>
      /// Formats string-format value from a culture to another.
      /// </summary>
      /// <typeparam name="T">Type of the underlying value, e.g. double.</typeparam>
      /// <param name="value">The string-format value.</param>
      /// <param name="cultureFrom">The original culture.</param>
      /// <param name="cultureTo">The new culture.</param>
      /// <returns>Returns the formatted value string.</returns>
      public static string FormatStringNullableValue<T> (string value,
                                                         CultureInfo cultureFrom,
                                                         CultureInfo cultureTo) where T : struct {
         return FormatStringNullableValue<T> (value, cultureFrom, cultureTo, false) ;
      }

      /// <summary>
      /// Formats string-format value from a culture to another.
      /// </summary>
      /// <typeparam name="T">Type of the underlying value, e.g. double.</typeparam>
      /// <param name="value">The string-format value.</param>
      /// <param name="cultureFrom">The original culture.</param>
      /// <param name="cultureTo">The new culture.</param>
      /// <param name="useGroupSeparator">Flag to indicate when the result has to contain goup (thousand) separator.</param>
      /// <returns>Returns the formatted value string.</returns>
      public static string FormatStringNullableValue<T> (string value,
                                                         CultureInfo cultureFrom,
                                                         CultureInfo cultureTo,
                                                         bool useGroupSeparator) where T : struct {
         if (string.IsNullOrEmpty (value)) {
            return value ;
         }

         if (cultureFrom.Equals (cultureTo)) {
            return value ;
         }

         Nullable<T> number = null ;
         if (!value.TryParseNullable (cultureFrom, out number)) {
            throw new CoreException ("Invalid value format. Not possible to convert string {0} to numeric format by the culture {1}.", value, cultureFrom) ;
         }

         int integerDigitCount = 0 ;
         int decimalDigitCount = 0 ;
         GetStringNullableValueDigitCounts (value, cultureFrom, out integerDigitCount, out decimalDigitCount) ;

         string decimalFormatter = string.Empty ;
         for (int i = 0; i < decimalDigitCount; i++) {
            decimalFormatter += "0" ;
         }

         string numberFormatter = string.Empty ;
         if (value.Contains (cultureFrom.NumberFormat.PositiveSign)) {
            numberFormatter = cultureTo.NumberFormat.PositiveSign ;
         }

         if (useGroupSeparator &&
             integerDigitCount > 3) {
            numberFormatter += "{0:0,0." + decimalFormatter + "}" ;
         } else {
            numberFormatter += "{0:0." + decimalFormatter + "}" ;
         }

         return string.Format (cultureTo.NumberFormat, numberFormatter, number) ;

         // ----------------------------------------------------------------------------------------------------
         // Another solution:

         /*if (!ValidateStringNullableValueFormat<T>(value, cultureFrom)) {
            throw new Sysment.Core.Exception(Resources.General.InvalidValueFormatException, value, cultureFrom);
         }

         value = value.Replace(cultureFrom.NumberFormat.NumberDecimalSeparator, "<d>");
         //value = value.Replace(cultureFrom.NumberFormat.NumberGroupSeparator, "<g>");
         value = value.Replace(cultureFrom.NumberFormat.NumberGroupSeparator, "");

         value = value.Replace("<d>", cultureTo.NumberFormat.NumberDecimalSeparator);
         //value = value.Replace("<g>", cultureTo.NumberFormat.NumberGroupSeparator);

         string infinitySymbol = "";
         if (value.Contains(cultureFrom.NumberFormat.NegativeSign)) {
            value = value.Replace(cultureFrom.NumberFormat.NegativeSign, "");
            infinitySymbol = cultureTo.NumberFormat.NegativeSign;
         }
         else if (value.Contains(cultureFrom.NumberFormat.PositiveSign)) {
            value = value.Replace(cultureFrom.NumberFormat.PositiveSign, "");
            infinitySymbol = cultureTo.NumberFormat.PositiveSign;
         }

         StringBuilder stringValue;

         if (useGroupSeparator) {
            stringValue = new StringBuilder();
            int startIndex = value.Length - 1;

            if (value.Contains(cultureTo.NumberFormat.NumberDecimalSeparator)) {
               startIndex = value.IndexOf(cultureTo.NumberFormat.NumberDecimalSeparator) - 1;
               stringValue.Append(value.Substring(startIndex + 1));
            }

            int counter = 0;
            for (int i = startIndex; i >= 0; i--) {
               stringValue.Insert(0, value[i]);
               counter++;
               if (counter == 3 &&
                  i != 0) {
                  stringValue.Insert(0, cultureTo.NumberFormat.NumberGroupSeparator);
                  counter = 0;
               }
            }
         }
         else {
            stringValue = new StringBuilder(value);
         }

         stringValue.Insert(0, infinitySymbol);

         //return value;
         return stringValue.ToString();*/
      }

      /// <summary>
      /// Separates a string-format numeric value to integer and decimal part and gives back the count of these digits.
      /// </summary>
      /// <param name="value"></param>
      /// <param name="cultureInfo"></param>
      /// <param name="integerDigitCount"></param>
      /// <param name="decimalDigitCount"></param>
      private static void GetStringNullableValueDigitCounts (string value,
                                                             CultureInfo cultureInfo,
                                                             out int integerDigitCount,
                                                             out int decimalDigitCount) {
         integerDigitCount = 0 ;
         decimalDigitCount = 0 ;

         if (string.IsNullOrEmpty (value)) {
            return ;
         }

         value = value.TrimStart (cultureInfo.NumberFormat.NegativeSign.ToCharArray()) ;
         value = value.TrimStart (cultureInfo.NumberFormat.PositiveSign.ToCharArray()) ;

         if (value.Contains (cultureInfo.NumberFormat.NumberDecimalSeparator)) {
            decimalDigitCount = value.Length - value.IndexOf (cultureInfo.NumberFormat.NumberDecimalSeparator) - 1 ;
            integerDigitCount = value.Length - decimalDigitCount - 1 ;
         } else {
            integerDigitCount = value.Length ;
         }
      }

      /// <summary>
      /// Validates whether a string-format value is a valid T-typed value.
      /// </summary>
      /// <typeparam name="T">Type of the underlying value, e.g. double.</typeparam>
      /// <param name="value">The string-format value.</param>
      /// <param name="cultureInfo">The culture which defined the string format.</param>
      /// <returns>Returns true if the string-format value is a valid T-typed value.</returns>
      public static bool ValidateStringNullableValueFormat<T> (string value,
                                                               CultureInfo cultureInfo) where T : struct {
         if (string.IsNullOrEmpty (value)) {
            return false ;
         }

         Nullable<T> numericValue ;
         return value.TryParseNullable<T> (cultureInfo, out numericValue) ;
      }

      /// <summary>
      /// Make sure that string contains Windows line feeds (\r\n).
      /// </summary>
      /// <param name="value">value to check</param>
      /// <returns></returns>
      public static string EnsureEnvironmentNewLines (this string value) {
         const string NEW_LINE = "\n" ;

         if (string.IsNullOrEmpty (value)) {
            return value ;
         }
         if (!value.Contains (NEW_LINE)) {
            return value ;
         }

         StringBuilder result = new StringBuilder (value) ;
         return result.Replace (Environment.NewLine, NEW_LINE).Replace (NEW_LINE, Environment.NewLine).ToString() ;
      }

      /// <summary>
      /// Convert the nullable double value to a string, which contains culture invariant decimal separators.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public static string ToCultureInvariantString (this double? value) {
         return value == null ? string.Empty : value.GetValueOrDefault().ToString (CultureInfo.InvariantCulture) ;
      }

      /// <summary>
      /// Convert the integer value to a string with invariant culture.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public static string ToCultureInvariantString (this int? value) {
         return value == null ? string.Empty : value.GetValueOrDefault().ToString (CultureInfo.InvariantCulture) ;
      }

      public static string EnsureNumberInvariantFormat (string value) {
         if (string.IsNullOrEmpty (value)) {
            return value ;
         }

         // No whitespace
         var valueToUse = value.Trim() ;

         char invariantDecimalSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator.ToCharArray() [0] ;
         valueToUse = valueToUse.Replace (',', invariantDecimalSeparator) ;
         valueToUse = valueToUse.Replace ('.', invariantDecimalSeparator) ;
         if (string.IsNullOrEmpty (valueToUse)) {
            return valueToUse ;
         }

         // Use only the last separator
         StringBuilder result = new StringBuilder() ;
         string[] valueParts = valueToUse.Split (new[] {invariantDecimalSeparator}) ;
         for (int valuePartIndex = 0; valuePartIndex < valueParts.Length; valuePartIndex++) {
            if ((valuePartIndex > 0) && (valuePartIndex == (valueParts.Length - 1))) {
               result.Append (invariantDecimalSeparator) ;
            }
            result.Append (valueParts [valuePartIndex]) ;
         }
         return result.ToString() ;
      }

      public static string EnsureNumberCurrentThreadFormat (this string value) {
         if (string.IsNullOrEmpty (value)) {
            return value ;
         }

         // No whitespace
         var valueToUse = value.Trim() ;

         char decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray() [0] ;
         valueToUse = valueToUse.Replace (',', decimalSeparator) ;
         valueToUse = valueToUse.Replace ('.', decimalSeparator) ;
         if (string.IsNullOrEmpty (valueToUse)) {
            return valueToUse ;
         }

         // Use only the last separator
         StringBuilder result = new StringBuilder() ;
         string[] valueParts = valueToUse.Split (new[] {decimalSeparator}) ;
         for (int valuePartIndex = 0; valuePartIndex < valueParts.Length; valuePartIndex++) {
            if ((valuePartIndex > 0) && (valuePartIndex == (valueParts.Length - 1))) {
               result.Append (decimalSeparator) ;
            }
            result.Append (valueParts [valuePartIndex]) ;
         }
         return result.ToString() ;
      }

      /// <summary>
      /// Make sure that a string ends with the specified end string.
      /// </summary>
      /// <param name="value"></param>
      /// <param name="end"></param>
      /// <returns></returns>
      public static string EnsureEnds (string value,
                                       string end) {
         if (string.IsNullOrEmpty (value)) {
            return null ;
         }
         if (string.IsNullOrEmpty (end)) {
            return value ;
         }

         if (value.EndsWith (end)) {
            return value ;
         }

         return EnsureEnds (value, end.SubstringEx (0, end.Length - 1)) + end.SubstringEx (end.Length - 1, 1) ;
      }

      /// <summary>
      /// Indicates whether the specified <see cref="System.String"/> contains <c>whitespace</c>.
      /// </summary>
      /// <param name="value">The <see cref="System.String"/> to examine.</param>
      /// <returns><c>true</c> if <paramref name="value"/> contains at least one whitespace char; otherwise <c>false</c>.</returns>
      public static bool ContainsWhitespace (this string value) {
         return string.IsNullOrEmpty(value) == false && value.Contains(" ");
      }

      /// <summary>
      /// Indicates whether the specified <see cref="System.String"/> is <c>null</c>, <c>empty</c> or contains only <c>whitespace</c>.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      /// <remarks>This method mimics the String.IsNullOrWhiteSpace method available in .Net 4 framework.</remarks>
      public static bool IsNullOrWhiteSpace (this string value) {
         return string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim());
      }

      public static bool IsWrappedInDoubleQuotes (this string value) {
         if (string.IsNullOrEmpty (value)) return false ;
         
         return value.IsNullOrWhiteSpace() == false && value.StartsWith("\"") && value.EndsWith("\"");
      }

      /// <summary>
      /// Wraps the specified <see cref="System.String"/> in double quotes.
      /// </summary>
      public static string WrapInDoubleQuotes (this string value) {
         return $@"""{value}""" ;
      }

      /// <summary>
      /// Wraps the specified <see cref="System.String"/> in double quotes.
      /// </summary>
      public static string UnwrapFromDoubleQuotes (this string value) {
         if (!value.IsWrappedInDoubleQuotes()) return value ;

         // Cut first and last characters
         return value.Substring (1, value.Length - 2) ;
      }

      /// <summary>
      /// Wraps the specified <see cref="System.String"/> in double quotes if it contains at least one whitespace character.
      /// </summary>
      /// <param name="str">The <see cref="System.String"/> to examine and wrap.</param>
      public static string WrapInDoubleQuotesIfContainsWhitespace (this string str) {
         return str.ContainsWhitespace() && str.IsWrappedInDoubleQuotes() == false
            ? str.WrapInDoubleQuotes()
            : str;
      }

      public static string EnsureTrailingSlash (this string uri) {
         const string TRAILING_SLASH = "/" ;

         if (String.IsNullOrEmpty (uri)) return TRAILING_SLASH ;

         if (uri.EndsWith (TRAILING_SLASH)) return uri ;

         return $"{uri}{TRAILING_SLASH}" ;
      }

      public static string EnsurePeriod (this string value) {
         if (String.IsNullOrEmpty (value)) return String.Empty ;

         const string VALID_TERMINATOR_CHARACTERS = ".!?,;" ;

         if (VALID_TERMINATOR_CHARACTERS.Contains (value[value.Length - 1].ToString())) return value ;

         return $"{value}." ;
      }      
   }

   /// <summary>
   /// StringBuilder extensions.
   /// </summary>
   public static class StringBuilderEx {
      /// <summary>
      /// Appends a formatted string, which contains zero or more format specifications, 
      /// to this instance. Each format specification is replaced by the string representation 
      /// of a corresponding object argument.
      /// </summary>
      /// <param name="stringBuilder">The StringBuilder object itself.</param>
      /// <param name="format">A composite format string.</param>
      /// <param name="args">An array of objects to format.</param>
      /// <returns>A reference to this instance with format appended. Any format specification 
      /// in format is replaced by the string representation of the corresponding object argument.</returns>
      public static StringBuilder AppendFormatLine (this StringBuilder stringBuilder,
                                                    string format,
                                                    params object[] args) {
         return stringBuilder.AppendFormat ($"{format}\r\n", args) ;
      }
   }
}

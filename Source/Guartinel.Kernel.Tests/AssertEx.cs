using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Text.RegularExpressions ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   /// <summary>
   /// Extended support for asserts for unit testing. It eases the testing of common unit testing use cases,
   /// for example when a statement needs to throw an exception
   /// </summary>
   public static class AssertEx {
      /// <summary> 
      /// Compare properties of two objects and fail if they are not the same.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="object1"></param>/// 
      /// <param name="object2"></param>
      /// <param name="compareValues"></param>
      public static void AreEqualByCompare<T> (T object1,
                                               T object2,
                                               Func<T, T, bool> compareValues) {
         // Compare with defaults
         if (!compareValues (object1, object2)) {
            Assert.Fail ("Properties in two objects are not the same.") ;
         }
      }

      /// <summary>
      /// Compare two lists and fail if they are not the same. The comparison is done by item and the specified comparer method is used.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="list1"></param>
      /// <param name="list2"></param>
      /// <param name="compareValues"></param>
      public static void ShouldContainSameItems<T> (IList<T> list1,
                                                    IList<T> list2,
                                                    Func<T, T, bool> compareValues) {
         // Compare with defaults
         if (!Guartinel.Kernel.Utility.General.CompareLists (list1, list2, compareValues)) {
            Assert.Fail ("Lists are not the same.") ;
         }
      }

      /// <summary> 
      /// Compare properties of two objects and fail if they are not the same.
      /// </summary>
      /// <param name="value"></param>
      /// <param name="expectedValue"></param>
      /// <param name="allowedDifferenceInPercents"></param>
      public static void AreEqual (double expectedValue,
                                   double value,
                                   double allowedDifferenceInPercents) {
         if (!((expectedValue * (1.0 - allowedDifferenceInPercents)) < value &&
               (expectedValue * (1.0 + allowedDifferenceInPercents) > value))) {
            Assert.Fail ("{0} expected, but {1} got.", expectedValue, value) ;
         }
      }

      public static void AreEqualNoWhitespaces (string expectedValue,
                                                string value) {
         const string REGEXP = @"\s+" ;
         Assert.AreEqual (Regex.Replace (expectedValue, REGEXP, ""),
                          Regex.Replace (value, REGEXP, "")) ;
      }

      /// <summary> 
      /// Compare to date time values, down to milliseconds (ignore ticks).
      /// </summary>
      /// <param name="value"></param>
      /// <param name="expectedValue"></param>
      public static void AreEqualToMilliSeconds (DateTime expectedValue,
                                                 DateTime value) {
         Assert.AreEqual (expectedValue.AddTicks (-expectedValue.Ticks), value.AddTicks (-value.Ticks)) ;
      }

      #region Exceptions
      /// <summary>
      /// Assertion to check if a method throws an exception containing a piece of text or not.
      /// </summary>
      /// <param name="method"></param>
      /// <param name="messageParts"></param>
      public static void ShouldThrowContaining (Action method,
                                                params string[] messageParts) {
         bool exceptionThrown = false ;
         string exceptionMessage = string.Empty ;

         try {
            method() ;
         } catch (System.Exception e) {
            exceptionThrown = true ;
            exceptionMessage = e.GetAllMessages() ;

            string message = e.Message.ToLowerInvariant() ;

            // Check all message parts
            foreach (string messagePart in messageParts) {
               if (!message.Contains (messagePart.ToLowerInvariant())) {
                  Assert.Fail ("Exception thrown as expected, but did not contain message '{0}'. Message: {1}", messagePart, e.Message) ;
               }
            }
         }

         if (!exceptionThrown) {
            Assert.Fail ($"Exception {string.Empty} not thrown, but expected. Thrown: {exceptionMessage}") ;
         }
      }

      /// <summary>
      /// Assertion to check if a method throws an exception or not.
      /// </summary>
      /// <param name="method"></param>
      public static void ShouldThrowException (Action method) {
         bool exceptionThrown = false ;
         try {
            method() ;
         } catch (System.Exception) {
            exceptionThrown = true ;
         }

         if (!exceptionThrown) {
            Assert.Fail (String.Format ("Exception not thrown, but expected.")) ;
         }
      }

      /// <summary>
      /// Method is called, and it should throw the exception specified.
      /// </summary>
      /// <param name="method"></param>
      /// <param name="exceptionType"></param>
      public static void ShouldThrow (Action method,
                                      Type exceptionType) {
         try {
            method() ;

            Assert.Fail (String.Format ("Exception {0} not thrown, but expected", exceptionType.FullName)) ;
         } catch (System.Exception e) {
            if (exceptionType != null) {
               if (!exceptionType.IsAssignableFrom (e.GetType())) {
                  Assert.Fail (String.Format ("An exception {0} thrown, but {1} expected.", e.GetType().FullName, exceptionType.FullName)) ;
               }
            }
         }
      }

      /// <summary>
      /// Method is called, and it should throw the exception specified in TException.
      /// </summary>
      /// <param name="method"></param>
      public static void ShouldThrow<TException> (Action method) where TException : System.Exception {
         ShouldThrow (method, typeof (TException)) ;
      }
      #endregion Exceptions

      /// <summary>
      /// Compare open and close brackets order 
      /// </summary>
      /// <param name="actualString"></param>
      // Created by NEKR
      public static void CompareOpenCloseBrackets (string actualString) {
         const string OPEN_BRACKETS = "{([" ;
         const string CLOSE_BRACKETS = "})]" ;

         string openBracketsString = string.Empty ;
         foreach (char actChar in actualString) {
            if (OPEN_BRACKETS.Contains (actChar.ToString())) {
               openBracketsString += actChar.ToString() ;
            } else if (CLOSE_BRACKETS.Contains (actChar.ToString())) {
               if (openBracketsString.EndsWith (OPEN_BRACKETS.Substring (CLOSE_BRACKETS.IndexOf (actChar), 1))) {
                  openBracketsString = openBracketsString.Substring (0, openBracketsString.Length - 1) ;
               } else {
                  Assert.Fail ("Brackets are not in pairs.") ;
               }
            }
         }
         if (openBracketsString != String.Empty) {
            Assert.Fail ("Missing close-brackets.") ;
         }
      }
   }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel {
   public static class Hierarchy {
      /// <summary>
      /// Create enumerator from hierarchy.
      /// </summary>
      /// <typeparam name="TSource"></typeparam>
      /// <param name="source"></param>
      /// <param name="nextItem"></param>
      /// <param name="canContinue"></param>
      /// <returns></returns>
      private static IEnumerable<TSource> FromHierarchy<TSource> (
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue) {
       
         for (var current = source; canContinue (current); current = nextItem (current)) {
            yield return current ;
         }
      }

      /// <summary>
      /// Create enumerator from linked list.
      /// </summary>
      /// <typeparam name="TSource"></typeparam>
      /// <param name="source"></param>
      /// <param name="nextItem"></param>
      /// <returns></returns>
      public static IEnumerable<TSource> FromHierarchy<TSource> (
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class {
         return FromHierarchy (source, nextItem, s => s != null) ;
      }
   }
}
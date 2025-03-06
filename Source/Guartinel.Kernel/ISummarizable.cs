using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel {

   public enum SummaryKind {
      Basic,
      Specific,
      Full
   } ;

   public interface ISummarizable {
      /// <summary>
      /// Get summary of the object.
      /// </summary>
      /// <param name="kind">Defines the kind of the summary.</param>
      /// <returns></returns>
      List<string> GetSummary (SummaryKind kind) ;
   }
}
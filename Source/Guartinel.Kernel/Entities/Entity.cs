using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel.Entities {
   public abstract class Entity { // }: IDuplicable, ISummarizable {
      public static class ParameterNames {
         public const string ID = "id" ;
      }

      #region Construction

      protected Entity() {
         ID = Guid.NewGuid().ToString() ;
      }

      #endregion
     
      #region Properties

      protected string _id ;
      public string ID {
         get => _id ;
         set => _id = value ;
      }

      protected DateTime _modificationTimestamp = DateTime.MinValue ;
      public DateTime ModificationTimestamp {
         get => _modificationTimestamp ;
         set => _modificationTimestamp = value ;
      }

      #endregion

      #region Duplicate

      //public IDuplicable Duplicate() {
      //   Entity target = Create() ;
      //   Duplicate (target) ;
      //   return target ;
      //}

      //protected void Duplicate (IDuplicable target) {
      //   if (target == null) return ;
      //   if (! (target is Entity)) return ;

      //   (target as Entity).ID = _id ;

      //   Duplicate1 (target as Entity) ;
      //}

      //protected abstract void Duplicate1 (Entity target) ;
      //protected abstract Entity Create() ;

      #endregion
 
      #region Summary

      ///// <summary>
      ///// Get summary of the object.
      ///// </summary>
      ///// <param name="kind">Defines the kind of the summary.</param>
      ///// <returns></returns>
      //public List<string> GetSummary (SummaryKind kind) {
      //   var result = new List<string>() ;

      //   switch (kind) {
      //      case SummaryKind.Basic:
      //         AddBasicSummary (result) ;

      //         break ;
            
      //      case SummaryKind.Specific:
      //         AddSpecificSummary (result) ;

      //         break ;

      //      case SummaryKind.Full:
      //         AddBasicSummary (result) ;
      //         AddSpecificSummary (result) ;

      //         break ;
      //   }

      //   return result ;         
      //}

      //protected virtual void AddBasicSummary (List<string> result) {
      //}

      //protected virtual void AddSpecificSummary (List<string> result) {
      //}
      #endregion
   }
}
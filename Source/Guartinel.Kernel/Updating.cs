using System;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel {
   /// <summary>
   /// Class to store updating information.
   /// </summary>
   public class Updating {
      /// <summary>
      /// Constructor.
      /// </summary>
      public Updating() {}

      /// <summary>
      /// Updating counter.
      /// </summary>
      protected int UpdatingCount {get ; set ;}

      /// <summary>
      /// Is updating under progress now?
      /// </summary>
      public bool IsUpdating {
         get {return UpdatingCount > 0 ;}
      }

      /// <summary>
      /// Begin update: increase counter.
      /// </summary>
      public void BeginUpdate() {
         UpdatingCount++ ;
      }

      /// <summary>
      /// End update: derease counter.
      /// </summary>
      public void EndUpdate() {
         // Check and decrease counter
         if (UpdatingCount > 0) {
            UpdatingCount-- ;
         }
      }

      /// <summary>
      /// Instance to support 'using'.
      /// </summary>
      public UpdatingCounterAgent DoUpdate {
         get {return new UpdatingCounterAgent (this) ;}
      }

      // Support 'using' statement
      public class UpdatingCounterAgent : IDisposable {
         private Updating _updating ;

         /// <summary>
         /// Constructor accepting reference to counter.
         /// </summary>
         /// <param name="updating"></param>
         public UpdatingCounterAgent (Updating updating) {
            _updating = updating ;

            _updating.BeginUpdate() ;
         }

         /// <summary>
         /// Destructor: end update.
         /// </summary>
         ~UpdatingCounterAgent() {
            EndUpdate() ;
         }

         /// <summary>
         /// End update.
         /// </summary>
         private void EndUpdate() {
            if (_updating == null) return ;

            _updating.EndUpdate() ;
            _updating = null ;
         }

         /// <summary>
         /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
         /// </summary>
         /// <filterpriority>2</filterpriority>
         public void Dispose() {
            EndUpdate() ;
         }
      }
   }
}

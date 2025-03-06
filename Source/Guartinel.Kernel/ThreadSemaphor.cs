using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel {
   /// <summary>
   /// Thread-safe semaphor object to allow parent thread to communicate with child thread.
   /// When parent thread wants to stop child thread, then disables the semaphor. The child needs
   /// to check the state of the semaphor during its run and stop if the semaphor is disabled.
   /// Pass the Flag
   /// </summary>
   public class ThreadSemaphor {
      public class Flag {
         private readonly object _lock = new object() ;
         private bool _enabled = true ;

         public Flag() {}

         public void RunLocked (Action action) {
            if (!_enabled) return ;

            lock (_lock) {
               action() ;
            }
         }

         public void Disable() {
            lock (_lock) {
               if (!_enabled) return ;

               _enabled = false ;
            }
         }

         public bool IsEnabled {
            get {
               lock (_lock) {
                  return _enabled ;
               }
            }
         }
      }

      private Flag _flag = new Flag() ;
      public Flag GetFlag {
         get {
            Flag flag = null ;
            _flag.RunLocked (() => {
               flag = _flag ;
            }) ;
            
            return flag ;
         }
      }

      public bool IsEnabled => _flag.IsEnabled ;

      public void DisableAndAbandonFlag() {
         _flag.Disable() ;

         _flag = new Flag() ;
      }
   }
}
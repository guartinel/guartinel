using System ;
using System.Collections.Concurrent ;
using System.Diagnostics ;
using System.Linq ;
using System.Text;
using Guartinel.Kernel.Utility ;
using System.Threading ;
using System.Threading.Tasks ;

namespace Guartinel.Kernel {
   /// <summary>
   /// Behavior class: execute an action. Base just executes action in sync.
   /// </summary>
   public abstract class ExecuteBehavior {
      public virtual void Execute (string actionName,
                                   Action action,
                                   Action<string> log = null) {
         // System.Diagnostics.Debug.WriteLine ("Action '" + actionName + "' started.") ;

         try {
            action() ;
         } catch (Exception e) {
            log?.Invoke (e.GetAllMessages()) ;
         }

         // System.Diagnostics.Debug.WriteLine ("Action '" + actionName + "' executed.") ;
      }

      public virtual void Cancel() {         
      }

      public class Synchronized : ExecuteBehavior {
      }

      /// <summary>
      /// Execute the action in separate background thread.
      /// </summary>
      public class InThread : ExecuteBehavior {
         private volatile bool _cancelled = false ;

         protected virtual ThreadPriority Priority => ThreadPriority.Normal ;

         public override void Execute (string actionName,
                                       Action action,
                                       Action<string> log = null) {

            // System.Diagnostics.Debug.WriteLine ("Action '" + actionName + "' queued (thread).") ;

            var thread = new Thread (x => {
               try {
                  if (_cancelled) return ;
                  
                  action() ;
               } catch (Exception e) {
                  log?.Invoke (e.GetAllMessages()) ;
               }
            }) ;

            // No background
            // thread.IsBackground = true ;
            thread.Priority = Priority ;
            try {
               thread.Start() ;
            } catch (Exception e) {
               log?.Invoke (e.GetAllMessages()) ;
               Debug.WriteLine ($"Error when starting thread: {e.Message}") ;
               throw ;
            }
         }

         public override void Cancel() {
            _cancelled = true ;
         }
      }

      /// <summary>
      /// Execute the action in separate background thread.
      /// </summary>
      public class InThreadLow : InThread {
         protected override ThreadPriority Priority => ThreadPriority.Lowest ;
      }

      /// <summary>
      /// Execute the action using tasks.
      /// </summary>
      public class InTask : ExecuteBehavior {
         private static volatile int _taskCount ;
         private volatile bool _cancelled = false;

         public override void Execute (string actionName,
                                       Action action,
                                       Action<string> log = null) {

            _taskCount++ ;            
            // Debug.WriteLine ($"Task created: {_taskCount}.") ;
            var task = new Task (() => {
               try {
                  if (_cancelled) return ;

                  // Debug.WriteLine ($"Task {actionName} started.") ;

                  action() ;

                  _taskCount-- ;
                  // Debug.WriteLine ($"Task {actionName} ended. Remaining tasks: {_taskCount}.") ;
               }
               catch (Exception e) {
                  // Debug.WriteLine ($"Task error: {e.GetAllMessages()}.") ;
                  log?.Invoke (e.GetAllMessages()) ;
               }
            }) ;

            try {
               task.Start() ;
            } catch (Exception e) {
               log?.Invoke (e.GetAllMessages()) ;
               Debug.WriteLine ($"Error when starting task: {e.Message}") ;
               throw;
            }
         }

         public override void Cancel() {
            // Debug.WriteLine($"InTask cancelled.");
            _cancelled = true ;
         }
      }

      /// <summary>
      /// Execute the action using tasks.
      /// </summary>
      public class InTaskQueue : ExecuteBehavior {
         private readonly string _name ;
         private readonly ConcurrentQueue<Task> _tasks = new ConcurrentQueue<Task>();
         private readonly CancellationTokenSource _taskCancellation = new CancellationTokenSource();

         // private static ConcurrentDictionary<string, Task> _queues = new ConcurrentDictionary<string, Task>() ;

         public InTaskQueue (string name) {
            // Debug.WriteLine($"Queue {name} created.");
            _name = name ;

            var taskRunner = new Task(() => {
               while (!_taskCancellation.IsCancellationRequested) {
                  // Debug.WriteLine ($"Check tasks in queue {_name}.");
                  Task nextTask;
                  _tasks.TryDequeue(out nextTask);
                  
                  if (nextTask != null) {
                     // Debug.WriteLine ($"Task in queue {_name} started, remaining: {_tasks.Count}.") ;
                     nextTask.Start();
                  }
                  Thread.Sleep(200);
               }
            }, _taskCancellation.Token, TaskCreationOptions.LongRunning) ;
            taskRunner.Start();
         }

         public override void Execute (string actionName,
                                       Action action,
                                       Action<string> log = null) {

            // Create new task
            var task = new Task (action, _taskCancellation.Token) ;
            _tasks.Enqueue (task) ;
            // Debug.WriteLine ($"Task in queue {_name} created, number of tasks: {_tasks.Count}.") ;
         }

         public override void Cancel() {
            // Debug.WriteLine($"Queue {_name} cancelled.");
            _taskCancellation.Cancel();
         }
      }
   }
}
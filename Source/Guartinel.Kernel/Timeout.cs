using System;
using System.Linq;
using System.Text;
using System.Threading ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.Kernel {
   public class Timeout : IDisposable {
      private readonly string _name ;

      public Timeout (int timeoutInMilliSeconds,
                      bool start,
                      string name) {
         _timeoutInMilliSeconds = timeoutInMilliSeconds ;
         _name = name ;

         // if (!string.IsNullOrEmpty (_name)) Logger.Log ($"Timeout {_name} created.", nameof (Timeout)) ;

         if (start) {
            _startTime = DateTime.UtcNow ;

            // if (!string.IsNullOrEmpty (_name)) Logger.Log ($"Timeout {_name} started.", nameof (Timeout)) ;
         }
      }

      public Timeout (int timeoutInMilliSeconds,
                      bool start) : this (timeoutInMilliSeconds, start, string.Empty) { }

      public Timeout (int timeoutInMilliSeconds) : this (timeoutInMilliSeconds, true) { }

      public Timeout (int timeoutInMilliSeconds,
                      string name) : this (timeoutInMilliSeconds, true, name) { }

      public Timeout (TimeSpan timeSpan) : this ((int) timeSpan.TotalMilliseconds, true) { }

      public Timeout (TimeSpan timeSpan,
                      string name) : this ((int) timeSpan.TotalMilliseconds, true, name) { }

      public Timeout() : this (0, false) { }

      public Timeout (Timeout source) : this() {
         _startTime = source._startTime ;
         _timeoutInMilliSeconds = source._timeoutInMilliSeconds ;
         _name = source._name ;

         // if (!string.IsNullOrEmpty (_name)) Logger.Log ($"Timeout {_name} cloned.", nameof (Timeout)) ;
      }

      protected readonly object _lock = new object() ;
      protected DateTime? _startTime = null ;
      public DateTime? StartTime => _startTime ;

      public bool IsStarted {
         get {
            lock (_lock) {
               return _startTime.HasValue ;
            }
         }
      }

      public void Reset (DateTime? startTime) {
         lock (_lock) {
            _startTime = startTime ;

            // if (!string.IsNullOrEmpty (_name)) Logger.Log ($"Timeout {_name} reseted to {startTime}.", nameof (Timeout)) ;
         }
      }

      public void Reset() {
         if (_timeoutInMilliSeconds <= 0) return ;

         lock (_lock) {
            _startTime = DateTime.UtcNow ;
            // if (!string.IsNullOrEmpty (_name)) Logger.Log ($"Timeout {_name} reseted to {DateTime.UtcNow}.", nameof (Timeout)) ;
         }
      }

      public void Start () {
         Reset() ;
      }

      protected int? _timeoutInMilliSeconds ;

      public int? TimeoutInMilliSeconds {
         get => _timeoutInMilliSeconds ;
         set => _timeoutInMilliSeconds = value ;
      }

      public bool RunnedOut {
         get {
            lock (_lock) {
               if (!_startTime.HasValue) return false ;
               if (!_timeoutInMilliSeconds.HasValue) return false ;

               return _startTime.Value.AddMilliseconds (_timeoutInMilliSeconds.Value) < DateTime.UtcNow ;
            }
         }
      }

      public bool StillOK {
         get {
            var result = !RunnedOut ;

            // if (!string.IsNullOrEmpty (_name)) Logger.Log ($"Timeout {_name} is still OK? {result}", nameof (Timeout)) ;

            return result ;
         }
      }

      /// <summary>
      /// Wait for for the condition to be true or timeout.
      /// </summary>
      /// <param name="condition"></param>
      /// <param name="sleepInMilliSeconds">Sleep this time between two checks during the wait.</param>
      /// <returns>Return true if no timeout but the condition became true.</returns>
      public bool WaitFor (Func<bool> condition,
                           int sleepInMilliSeconds = 200) {
         return WaitFor (condition, TimeSpan.FromMilliseconds (sleepInMilliSeconds)) ;
      }

      public bool WaitFor (Func<bool> condition,
                           TimeSpan sleep) {
         if (condition == null) return false ;

         while (StillOK && !condition()) {
            Thread.Sleep (sleep) ;
         }

         return !RunnedOut ;
      }

      /// <summary>
      /// Wait for for the condition to be true or timeout.
      /// </summary>
      /// <returns>Return true if no timeout but the condition became true.</returns>
      public void Wait() {
         while (StillOK) {
            Thread.Sleep (200) ;
         }
      }

      public void Dispose() {
         // if (!string.IsNullOrEmpty (_name)) Logger.Log ($"Timeout {_name} is disposed.", nameof (Timeout)) ;
      }
   }

   public class TimeoutSeconds : Timeout {
      public TimeoutSeconds (int timeoutInSeconds,
                             bool start,
                             string name) : base (timeoutInSeconds * 1000, start, name) { }

      public TimeoutSeconds (int timeoutInSeconds,
                             bool start) : base (timeoutInSeconds * 1000, start) { }

      public TimeoutSeconds (int timeoutInSeconds) : base (timeoutInSeconds * 1000) { }

      public TimeoutSeconds (int timeoutInSeconds,
                             string name) : base (timeoutInSeconds * 1000, name) { }
   }
}
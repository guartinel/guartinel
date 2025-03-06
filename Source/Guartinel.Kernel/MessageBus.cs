using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Linq ;
using System.Threading ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.Kernel {
   public class MessageBus {
      // Ancestor for message objects
      public class Message {
      }

      public class Subscription {
         public Subscription (Type type,
                       string id) {
            Type = type ;
            ID = id ;
         }

         public Subscription (Type type) {
            Type = type;
            ID = null ;
         }

         public readonly Type Type;
         public readonly string ID ;

         protected bool Equals (Subscription other) {
            return Equals (Type, other.Type) && string.Equals (ID, other.ID) ;
         }

         public override bool Equals (object other) {
            if (ReferenceEquals (null, other)) return false ;
            if (ReferenceEquals (this, other)) return true ;
            if (other.GetType() != this.GetType()) return false ;
            return Equals ((Subscription) other) ;
         }

         public override int GetHashCode() {
            unchecked {
               return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (ID != null ? ID.GetHashCode() : 0) ;
            }
         }
      }

      public static MessageBus Use {get ;} = new MessageBus() ;

      // List of subscriptions, key: subscription instance
      private readonly Dictionary<Subscription, object> _consumers = new Dictionary<Subscription, object>() ;
      private readonly object _consumersLock = new object() ;

      private readonly SynchronizationContext _synchronizationContext ;

      public MessageBus() {
         _synchronizationContext = AsyncOperationManager.SynchronizationContext ;
      }

      private void Register (Subscription subscription,
                             Action<object> action) {
         Logger.Debug ($"MessageBus: New action registered. Type '{subscription.Type.Name}'. ID: {subscription.ID}") ;

         lock (_consumersLock) {
            List<Action<object>> list ;
            if (!_consumers.ContainsKey (subscription)) {
               list = new List<Action<object>>() ;
               _consumers.Add (subscription, list) ;
            } else {
               list = _consumers [subscription] as List<Action<object>> ;
            }
            list?.Add (action) ;
         }
      }

      public void Register<T> (string id,
                               Action<T> action) {
         Register (new Subscription (typeof(T), id), x => {
            if (!(x is T)) return ;

            action ((T) x) ;
         }) ;
      }

      public void Register<T> (Action<T> action) {
         Register (null, action) ;
      }

      //public void Register<T>(Action action) {
      //   // Logger.Debug ("New action registered for type " + typeof (T)) ;

      //   Register<T>(message => action());
      //}

      private void Unregister (Subscription subscription) {
         lock (_consumersLock) {
            if (!_consumers.ContainsKey (subscription)) return ;
            
            Logger.Debug ($"MessageBus: Action unregistered. Type '{subscription.Type.Name}'. ID: {subscription.ID}") ;

            _consumers.Remove (subscription) ;
         }
      }

      public void Unregister<T> (string id) {
         Unregister (new Subscription(typeof(T), id)) ;
      }

      public void Unregister<T> () {
         Unregister<T>(null);
      }

      private void Post (Subscription subscription,
                         object message) {
         List<Action<object>> actions ;

         // Collect subscribers
         lock (_consumersLock) {
            if (!_consumers.ContainsKey (subscription)) return ;

            actions = _consumers [subscription] as List<Action<object>> ;
         }

         Logger.Debug ($"MessageBus: Executing actions ({actions?.Count}). Type '{subscription.Type.Name}'. ID: {subscription.ID}") ;

         // Call subscribers
         actions?.ForEach (action => {_synchronizationContext.Post (message1 => action (message1), message) ;}) ;

         Logger.Debug ($"MessageBus: Actions executed. Type '{subscription.Type.Name}'. ID: {subscription.ID}") ;
      }

      public void Post<T> (string id,
                           T message) {
         Post (new Subscription (typeof(T), id), message) ;
      }

      public void Post<T> (T message) {
         Post (null, message) ;
      }

      public void Reset() {
         Logger.Debug ("MessageBus: Cleared.") ;

         lock (_consumersLock) {
            _consumers.Clear() ;
         }
      }
   }
}

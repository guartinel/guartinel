using System;
using System.Collections.Concurrent ;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading ;
using System.Threading.Tasks;
using Guartinel.Kernel.Logging ;

namespace Guartinel.Kernel {
   public class ObjectPool<T> {
      public ObjectPool (Func<T> objectCreator,
                         int maxObjectCount) {
         _objectCreator = objectCreator ?? throw new ArgumentNullException (nameof(objectCreator)) ;
         _maxObjectCount = maxObjectCount ;
      }

      private readonly ConcurrentBag<T> _objects = new ConcurrentBag<T>();
      private readonly Func<T> _objectCreator;

      private int _maxObjectCount;

      public int MaxObjectCount {
         get {
            lock (_objectCreationLock) {
               return _maxObjectCount ;
            }
         }

         set {
            lock (_objectCreationLock) {
               _maxObjectCount = value ;
            }
         }
      }

      private int _objectCount = 0;
      public int ObjectCount {
         get {
            lock (_objectCreationLock) {
               return _objectCount;
            }
         }

         set {
            lock (_objectCreationLock) {
               _objectCount = value;
            }
         }
      }
      
      private readonly object _objectCreationLock = new object();

      public void Use(Action<T> action) {
         if (action == null) return ;

         Logger.Debug($"Object pool {typeof(T).Name} request arrived.") ;

         while (true) {
            // If we have free, then we take it, yum-yum
            if (_objects.TryTake (out var item)) {
               try {
                  Logger.Debug ($"Object pool {typeof(T).Name} request fulfilled.") ;
                  // We got a shot
                  action (item) ;
                  Logger.Debug ($"Object pool {typeof(T).Name} executed action.") ;
               } finally {
                  lock (_objectCreationLock) {
                     if (_objectCount <= _maxObjectCount) {
                        // Put item back
                        _objects.Add (item) ;
                        Logger.Debug ($"Object pool {typeof(T).Name} reused object. Contains {_objectCount} instances.");
                     } else {
                        // DO NOT put item back
                        Interlocked.Decrement (ref _objectCount) ;
                        Logger.Debug ($"Object pool {typeof(T).Name} released an item, contains {_objectCount} instances, maximum {_maxObjectCount}.") ;
                     }
                  }
               }

               return ;
            }

            // Check if we can create objects
            lock (_objectCreationLock) {
               if (_objectCount < _maxObjectCount) {
                  Interlocked.Increment (ref _objectCount) ;
                  _objects.Add (_objectCreator()) ;
                  Logger.Debug($"Object pool {typeof(T).Name} created a new item, contains {_objectCount} instances, maximum {_maxObjectCount}.") ;
                  continue;
               }
            }
            // No free objects, cannot create, so cry!
            Thread.Sleep (10) ;
         }
      }
   }
}
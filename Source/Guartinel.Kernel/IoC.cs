using System;
using System.Collections.Generic ;
using System.Linq;
using System.Reflection ;
using System.Text;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;

// using SimpleInjector ;
// using SimpleInjector.Advanced ;

namespace Guartinel.Kernel {   
   public class IoC {
      public static IoC Use = new IoC() ;

      public class NoImplementationFound<TService> : Exception where TService : class {
         public NoImplementationFound() : base ($"No implementation found for '{typeof(TService).Name}'.") {}
      }

      public class TypeAlreadyRegisteredException : Exception {
         public TypeAlreadyRegisteredException (Type service) : base($"{service.Name} is already registered.") {}
      }

      // private Container _container = new Container() ;

      public IoC () {
         Single = new SingleRegistrations() ; // () => _container) ;
         Multi = new MultiRegistrations() ; // () => _container) ;
      }

      public SingleRegistrations Single {get ;}      

      public class SingleRegistrations {
         private readonly Dictionary<Type, Func<object>> _registeredTypes = new Dictionary<Type, Func<object>>() ;

         public SingleRegistrations () {
         }

         private void CheckType (Type service) {
            if (service == null) {
               throw new Exception($"Invalid service type specified.");
            }

            lock (_registeredTypes) {
               // Check if a collection is registered already, if not, then register
               if (_registeredTypes.ContainsKey (service)) {
                  throw new TypeAlreadyRegisteredException (service) ;
               }
            }
         }

         private void CheckType<TService> () where TService : class {
            CheckType (typeof (TService)) ;
         }

         public void Register<TService> (Func<TService> creator) where TService : class {
            CheckType<TService>() ;

            lock (_registeredTypes) {
               _registeredTypes.Add (typeof(TService), creator) ;
            }
         }

         public void RegisterLazy<TService> (Func<TService> lazyCreator) where TService : class {
            CheckType<TService>();
            lazyCreator.CheckNull() ;
            var lazy = new Lazy<TService>(lazyCreator) ;

            lock (_registeredTypes) {
               _registeredTypes.Add (typeof(TService), () => lazy.Value) ;
            }
         }

         public void Register<TService> (TService instance) where TService : class {
            Register(() => instance) ;
         }

         public void Register<TService, TImplementation> () where TService : class
                                                            where TImplementation : class, TService, new() {
            Register<TService> (new TImplementation()) ;
         }

         public TService GetInstance<TService> () where TService : class {
            lock (_registeredTypes) {
               if (_registeredTypes.ContainsKey (typeof(TService))) {
                  TService result = _registeredTypes [typeof(TService)]?.Invoke().CastTo<TService>() ;

                  if (result != null) return result ;
               }
            }
            
            Logger.Error ($"Implementation not found for '{typeof(TService).FullName}'.") ;
            
            throw new NoImplementationFound<TService>() ;
         }

         public bool ImplementationExists<TService> () where TService : class {
            lock (_registeredTypes) {
               return _registeredTypes.ContainsKey (typeof(TService)) ;
            }
         }

         internal void Clear () {
            lock (_registeredTypes) {
               _registeredTypes.Clear() ;
            }
         }
      }

      public MultiRegistrations Multi { get; }

      public class MultiRegistrations {
         public MultiRegistrations () {
         }

         private class Registration {
            public Registration (Func<object> creator,
                                 IEnumerable<string> names = null) {
               Creator = creator ;
               Names = names?.ToList() ?? new List<string>() ;
            }

            public Func<object> Creator {get ;}

            public List<string> Names { get; }
         }

         private readonly Dictionary<Type, List<Registration>> _registeredTypes = new Dictionary<Type, List<Registration>>() ;

         private void Ensure<TService>() where TService : class {
            lock (_registeredTypes) {
               // Check if a collection is registered already, if not, then register
               if (_registeredTypes.ContainsKey (typeof(TService))) return ;

               
               _registeredTypes.Add (typeof(TService), new List<Registration>()) ;
            }
         }

         public void Register<TService> (Func<TService> creator,
                                         IEnumerable<string> names = null) where TService : class {
            Ensure<TService>();

            lock (_registeredTypes) {
               _registeredTypes[typeof(TService)].Add(new Registration (creator, names));
            }
         }

         public void Register<TService> (Func<TService> creator,
                                         string name) where TService : class {

            Register<TService>(creator, new[] {name}) ;
         }

         public void Register<TService> (TService instance,
                                         IEnumerable<string> names = null) where TService : class {
            Register (() => instance, names) ;
         }

         public void Register<TService, TImplementation>(IEnumerable<string> names = null)
            where TService : class
            where TImplementation : class, TService, new() {
            
            Register<TService> (() => new TImplementation(), names) ;
         }

         public void Register<TService, TImplementation> (params string[] names)
            where TService : class
            where TImplementation : class, TService, new() {

            Register<TService>(() => new TImplementation(), names);
         }

         public void Register<TService, TImplementation1, TImplementation2>()
            where TService : class
            where TImplementation1 : class, TService, new()
            where TImplementation2 : class, TService, new() {

            Ensure<TService>();

            lock (_registeredTypes) {
               _registeredTypes [typeof(TService)].Add (new Registration (() => new TImplementation1())) ;
               _registeredTypes [typeof(TService)].Add (new Registration (() => new TImplementation2())) ;
            }
         }

         public void Register<TService> (Assembly assembly) where TService : class {
            Ensure<TService>();

            var types = from type in assembly.GetExportedTypes()
                           where typeof(TService).IsAssignableFrom (type)
                           select type ;

            foreach (Type type in types) {
               var constructor = type.GetConstructors().FirstOrDefault (c => !c.GetParameters().Any()) ;
               if (constructor != null) {
                  Register (() => (TService) Activator.CreateInstance (type)) ;
               }               
            }
         }

         public bool ImplementationExists<TService>() where TService : class {
            lock (_registeredTypes) {
               if (!_registeredTypes.ContainsKey (typeof(TService))) return false ;

               return _registeredTypes [typeof(TService)].Any() ;
            }
         }

         public bool ImplementationExists<TService> (string name) where TService : class {
            lock (_registeredTypes) {
               if (!_registeredTypes.ContainsKey (typeof(TService))) return false ;

               return _registeredTypes [typeof(TService)].Any (x => x.Names.Contains (name)) ;
            }
         }

         public IList<TService> GetInstances<TService>() where TService : class {
            Ensure<TService>();

            List<Registration> registrations ;
            List<TService> result = new List<TService>();

            lock (_registeredTypes) {
               registrations = _registeredTypes [typeof(TService)] ;
            }

            foreach (var registration in registrations) {
               result.Add(registration.Creator?.Invoke().CastTo<TService>());
            }

            return result;
         }

         public TService GetInstance<TService> (string name) where TService : class {
            Ensure<TService>() ;

            Registration registration ;

            lock (_registeredTypes) {
               registration = _registeredTypes [typeof(TService)].FirstOrDefault (x => x.Names.Contains (name)) ;
            }

            if (registration == null) throw new Exception($"Instance not found for type '{typeof(TService).Name}' for name '{name}'.");

            return registration.Creator?.Invoke().CastTo<TService>();
         }

         internal void Clear() {
            lock (_registeredTypes) {
               _registeredTypes.Clear();
            }
         }
      }

      //public static void RegisterToCollection<TService, TImplementation>() where TImplementation : class, TService {
      //   // Use.AppendToCollection (typeof(TService), typeof (TImplementation)) ;   
      //   Use.AppendToCollection (typeof (TService), Lifestyle.Singleton.CreateRegistration<TImplementation> (Use)) ;
      //}

      //public static void RegisterToCollection<TService>(Func<TService> creator) where TService : class {
      //   Use.AppendToCollection (typeof (TService), Lifestyle.Singleton.CreateRegistration (creator, Use)) ;
      //}

      //public static void RegisterImplementations<TService>(Assembly assembly) where TService : class {
      //   // Debug.WriteLine ($"Assembly: {assembly.FullName}") ;
      //   Use.RegisterCollection<TService> (new[] {assembly}) ;
      //}      

      //public static bool ImplementationExists<TService>() where TService : class {
      //   return Use.GetRegistration (typeof(TService)) != null ;
      //}

      //public static List<TService> GetAllInstances<TService>() where TService : class {
      //   if (!ImplementationExists<TService>()) {
      //      return new List<TService>() ;
      //   }

      //   return Use.GetAllInstances<TService>().ToList() ;
      //}

      //public static TService GetImplementation<TService>() where TService : class {
      //   return Use.GetRegistration (typeof(TService)).GetInstance() as TService ;
      //}

      public void Verify() {
         // _container.Verify();
      }

      public void Clear() {
         Single.Clear() ;
         Multi.Clear();
      }
   }
}

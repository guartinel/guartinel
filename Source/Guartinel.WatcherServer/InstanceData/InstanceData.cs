using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Configuration ;
using Guartinel.WatcherServer.Supervisors.ApplicationSupervisor ;

namespace Guartinel.WatcherServer.InstanceData {
   public class InstanceData {
      public string ID ;
      public string Name ;
      public ConfigurationData MeasuredData ;

      public InstanceData (string id,
                             string name,
                             ConfigurationData result) {
         ID = id ;
         Name = name ;
         MeasuredData = result.Duplicate() ;
      }

      public InstanceData Duplicate() {
         return new InstanceData (ID, Name, MeasuredData.Duplicate()) ;
      }
   }

   public class InstanceDataLists {
      protected Dictionary<string, List<InstanceData>> _lists = new Dictionary<string, List<InstanceData>>();
      protected object _listsLock = new object() ;


      public InstanceDataLists() { }

      public InstanceDataLists (string id,
                                string name,
                                ConfigurationData result) {
         lock (_listsLock) {
            var list = GetList (id) ;
            list.Add (new InstanceData (id, name, result)) ;
         }         
      }

      public IList<string> Keys {
         get {
            lock (_listsLock) {
               return _lists.Keys.ToList() ;
            }
         }
      }

      public bool IsEmpty {
         get {
            lock (_listsLock) {
               return !_lists.Keys.Any();
            }
         }
      }

      //public bool HeartbeatExists {
      //   get {
      //      lock (_listsLock) {
      //         return _lists.Any (x => ApplicationInstanceData.IsHeartbeat(x.Value.LastOrDefault())) ;
      //      }
      //   }
      //}

      protected List<InstanceData> GetList (string id) {
         if (!_lists.ContainsKey (id)) {
            _lists.Add (id, new List<InstanceData>()) ;
         }

         return _lists [id] ;
      }

      public List<InstanceData> Get (string id) {
         lock (_listsLock) {
            var list = GetList(id);
            return list;
         }
      }

      public void Add (string id,
                       InstanceData instanceData) {
         lock (_listsLock) {
            var list = GetList (id) ;
            list.Add (instanceData) ;
         }
      }

      public void Remove (string id) {
         lock (_listsLock) {
            if (!_lists.ContainsKey (id)) return ;
            _lists.Remove (id) ;
         }
      }

      public InstanceDataLists Duplicate() {
         var result = new InstanceDataLists();

         lock (_listsLock) {
            foreach (var key in _lists.Keys) {
               var resultList = new List<InstanceData>() ;
               foreach (var instanceData in _lists [key]) {
                  resultList.Add (instanceData.Duplicate()) ;
               }

               result._lists.Add (key, resultList) ;
            }
         }

         return result ;
      }

      public void Clear() {
         lock (_listsLock) {
            _lists.Clear();
         }
      }

      public bool Contains (string id) {
         lock (_listsLock) {
            return _lists.ContainsKey (id) ;
         }
      }
   }
}
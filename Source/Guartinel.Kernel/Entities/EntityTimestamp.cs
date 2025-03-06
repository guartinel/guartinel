using System ;
using System.Linq ;
using System.Text ;
using Newtonsoft.Json ;

namespace Guartinel.Kernel.Entities {
   public class EntityTimestamp {
      public EntityTimestamp() {}

      public EntityTimestamp (Entity entity) {
         ID = entity.ID ;
         ModificationTimestamp = entity.ModificationTimestamp ;
      }

      public EntityTimestamp (string id,
                              DateTime modificationTimestamp) {
         ID = id ;
         ModificationTimestamp = modificationTimestamp ;
      }

      [JsonProperty]
      public string ID {get ; set ;}

      [JsonProperty]
      public DateTime ModificationTimestamp {get ; set ;}
   }
}
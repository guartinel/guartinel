using System ;
using System.Linq ;
using System.Runtime.Serialization ;
using System.Text ;

namespace Guartinel.Website.Common.Configuration.Data {
   [DataContract]
   public class UserWebServer : IConnectable {
      public UserWebServer (string address,
            string name) {
         Address = address ;
         Name = name ;
        
      }

      public string GetAddress() {
         return Address;
      }

      [DataMember]
      public string Address {get ; set ;}

      [DataMember]
      public string Name {get ; set ;}
    
   }
}

using System ;
using System.Runtime.Serialization ;

namespace Guartinel.Website.Common.Configuration.Data {
   [DataContract]
   public class ManagementServer  : IConnectable{
      public ManagementServer (string name,
            string address,
            string description,
            string token
            ) {
         Name = name ;
         Address = address ;
         Description = description ;
         Token = token ;
      }
      public string GetAddress() {
         return Address ;
      }
      [DataMember]
      public int Id {get ; set ;}

      [DataMember]
      public string Name {get ; set ;}

      [DataMember]
      public string Address {get ; set ;}

      [DataMember]
      public string Description {get ; set ;}

      [DataMember]
      public string Token {get ; set ;}
    
   }
}

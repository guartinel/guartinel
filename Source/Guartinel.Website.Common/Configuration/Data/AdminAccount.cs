using System ;
using System.Linq ;
using System.Runtime.Serialization ;

namespace Guartinel.Website.Common.Configuration.Data {
    [DataContract]
    public class AdminAccount {
   
       [DataMember]
        public string PasswordHash { get; set; }

        [DataMember]
        public System.DateTime CreationTimeStamp { get; set; }

        [DataMember]
        public Nullable<System.DateTime> LastLogin { get; set; }

        [DataMember]
        public string Username { get; set; }
       
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public bool IsFirstConfigurationDone { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }
        }
    }
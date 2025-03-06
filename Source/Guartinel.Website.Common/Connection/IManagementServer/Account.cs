using Guartinel.Communication ;
using Guartinel.Website.Common.Configuration.Data ;

namespace Guartinel.Website.Common.Connection.IManagementServer {
   public class Account {
      public class ValidateToken : Request {
         public ValidateToken (WebRequester requester,IConnectable destination, string token) : base (requester,destination,Communication.ManagementServerAPI.Account.ValidateToken.FULL_URL) {
            _requestModel.Add (ManagementServerAPI.Account.ValidateToken.Request.TOKEN, token) ;
            Execute() ;
         }

         protected override void ParseResponse() {}
      }
   }
}

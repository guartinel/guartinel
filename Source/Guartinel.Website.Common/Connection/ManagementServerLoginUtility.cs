using Guartinel.Communication;
using Guartinel.Kernel.Logging;
using Guartinel.Website.Common.Configuration;
using Guartinel.Website.Common.Error;
using Guartinel.Website.Common.Tools;
using Newtonsoft.Json.Linq;

namespace Guartinel.Website.Common.Connection {
   public class ManagementServerLoginUtility {
      private WebRequester _requester ;

      public ManagementServerLoginUtility (WebRequester webRequester) {
         _requester = webRequester ;
      }

      public void LoginAndSetToken (ISettings settings) {
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.Login.Request.PASSWORD, settings.AdminAccount.PasswordHash) ;
         requestModel.Add (ManagementServerAPI.Admin.Login.Request.USER_NAME, settings.AdminAccount.Username) ;

         JObject managementResponse = _requester.SendRequestTo (settings.ManagementServer, ManagementServerAPI.Admin.Login.FULL_URL,
               requestModel) ;

         if (!MessageTool.IsSuccess (managementResponse)) {
            string error = MessageTool.GetError (managementResponse) ;
            Logger.Error($"Error when trying to login in MS: {error}");

            throw new CustomException {
               ErrorMessage = error
            } ;
         }
         settings.ManagementServer.Token = MessageTool.GetToken (managementResponse) ;
      }
   }
}

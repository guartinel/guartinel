using Guartinel.Communication ;
using Guartinel.Website.Common.Configuration.Data ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Website.Common.Connection.IManagementServer {
   public class Admin {
      public class SetWebSiteAddress : Request {
         public SetWebSiteAddress (WebRequester requester, IConnectable destination, string token,
                                   string address) : base (requester, destination, Communication.ManagementServerAPI.Admin.SetWebSiteAddress.FULL_URL) {
            _requestModel.Add (ManagementServerAPI.Admin.SetWebSiteAddress.Request.TOKEN, token) ;
            _requestModel.Add (ManagementServerAPI.Admin.SetWebSiteAddress.Request.WEBSITE_ADDRESS, address) ;

            Execute() ;
         }

         protected override void ParseResponse() {}
      }

      public class Login : Request {
         public Login (WebRequester requester, IConnectable dest, string userName,
                       string password) : base (requester, dest, Communication.ManagementServerAPI.Admin.Login.FULL_URL) {
            _requestModel.Add (ManagementServerAPI.Admin.Login.Request.USER_NAME, userName) ;
            _requestModel.Add (ManagementServerAPI.Admin.Login.Request.PASSWORD, password) ;

            Execute() ;
         }

         protected override void ParseResponse() {
            try {
               Token = _response.GetValue (ManagementServerAPI.Admin.Login.Response.TOKEN).Value<string>() ;
            } catch (System.Exception) {
               _isSuccess = false ;
            }
         }

         public string Token {get ; set ;}
      }

      public class Update : Request {
         public Update (WebRequester requester, IConnectable destination,
                        string token,
                        string userName,
                        string password,
                        string webPageURL,
                        string emailPassword,
                        string emailProvider,
                        string emailUserName) : base (requester, destination, Communication.ManagementServerAPI.Admin.Update.FULL_URL) {
            _requestModel.Add (ManagementServerAPI.Admin.Update.Request.TOKEN, token) ;
            _requestModel.Add (ManagementServerAPI.Admin.Update.Request.USER_NAME, userName) ;
            _requestModel.Add (ManagementServerAPI.Admin.Update.Request.PASSWORD, password) ;
            _requestModel.Add (ManagementServerAPI.Admin.Update.Request.WEB_PAGE_URL, webPageURL) ;
            _requestModel.Add (ManagementServerAPI.Admin.Update.Request.EMAIL_PASSWORD, emailPassword) ;
            _requestModel.Add (ManagementServerAPI.Admin.Update.Request.EMAIL_PROVIDER, emailProvider) ;
            _requestModel.Add (ManagementServerAPI.Admin.Update.Request.EMAIL_USER_NAME, emailUserName) ;

            Execute() ;
         }

         protected override void ParseResponse() {}
      }
   }
}
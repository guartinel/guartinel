using System;
using System.IO;
using System.Net;
using Guartinel.Website.Common.Configuration;
using Guartinel.Website.Common.Configuration.Data;
using Guartinel.Website.Common.Error;
using Guartinel.Website.Common.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.Website.Common.Connection {
   public class WebRequester {
      private static ISettings _settings ;

      public WebRequester (ISettings settings) {
         _settings = settings ;
         ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications ;
      }

      ~WebRequester() {
         ServicePointManager.ServerCertificateValidationCallback -= AcceptAllCertifications ;
      }

      public static bool AcceptAllCertifications (object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certification,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors) {
          Logger.Log("AcceptAllCertifications is called. This method should be used only in dev.");
         return true ;
      }

      public JObject SendRequestTo (IConnectable connectable,
            string path,
            object data,
            bool isRetry = false,bool isGetRequest = false) {
         string address = StringTool.EnsureStringEndsToBackSlash (connectable.GetAddress()) ;
         if (path.StartsWith ("/")) path = path.Remove (0, 1) ;

         string requestUrl = address + path ;
         Logger.Log("Sending request to: " + requestUrl);
         try {
            WebRequest webRequest = WebRequest.Create (requestUrl) ;
            webRequest.ContentType = "application/json" ;
            if(isGetRequest) {
               webRequest.Method = "GET";
            }else {
               webRequest.Method = "POST";
            }

            using (StreamWriter streamWriter = new StreamWriter (webRequest.GetRequestStream())) {
               string json = JsonConvert.SerializeObject (data) ;
              // LogWrapper.Debug ("With data: " + json) ;
                streamWriter.Write (json) ;
            }

            HttpWebResponse httpResponse = (HttpWebResponse) webRequest.GetResponse() ;

            using (StreamReader streamReader = new StreamReader (httpResponse.GetResponseStream())) {
               string result = streamReader.ReadToEnd() ;
               Logger.Debug("Response from MS:\n" + result);
               JObject response = JObject.Parse (result) ;
               if (MessageTool.IsSuccess (response)) return response ;

               if (MessageTool.GetError (response) != null && MessageTool.GetError (response).Equals (AllErrorValues.INVALID_ADMINISTRATION_TOKEN)) {
                  Logger.Log ("Administration token is invalid trying to relogin.") ;
                  var loginRequest = new Common.Connection.IManagementServer.Admin.Login (this, _settings.ManagementServer, _settings.AdminAccount.Username, _settings.AdminAccount.PasswordHash) ;
                  string token = loginRequest.Token ;
                  _settings.ManagementServer.Token = token ;

                  if (!isRetry) {
                     Logger.Log ("This was the first attempt of a request so lets try it again with the new token.") ;
                     SendRequestTo (connectable, requestUrl, data, true) ;
                  }
               }
               return response ;
            }
         } catch (Exception e) {
             Logger.Error($"Error while making request to server: { e.GetAllMessages()}");
            JObject jObject = new JObject() ;
            jObject.Add (AllParameters.ERROR, ErrorMessages.CANNOT_CONNECT_TO_REMOTE_HOST) ;
            return jObject ;
         }
      }
   }
}

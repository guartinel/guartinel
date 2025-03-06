using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Net ;
using System.Text ;
using System.Threading ;
using System.Web ;
using System.Windows.Forms ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Connection.Server.Routes ;
using Guartinel.WatcherServer.Connection.Server.Routes.Package ;
using Guartinel.WatcherServer.Logic ;
using Sysment.Watcher.WatcherServer ;

namespace Guartinel.WatcherServer.Connection.Server {
   internal class HttpServer {

      private readonly string[] _httpUrlPrefixes = {
         RunningConfig.LocalHost + Urls.WatcherServer.CHECK_RESULT,
         RunningConfig.LocalHost + Urls.WatcherServer.CREATE_ITSTATUS_PACKAGE,
         RunningConfig.LocalHost + Urls.WatcherServer.UPDATE_ITSTATUS_PACKAGE,
         RunningConfig.LocalHost + Urls.WatcherServer.DELETE_PACKAGE
      } ;

      private readonly HttpListener _httpListener = new HttpListener() ;
      private readonly IWatcherRunnerX _runnerX = null ;

      public HttpServer (IWatcherRunnerX runnerX) {
         _runnerX = runnerX ;
         foreach (string s in _httpUrlPrefixes) {
            _httpListener.Prefixes.Add (s) ;
            MainForm.View.AddMsgToList ("Prefix added: " + s) ;
         }
      }

      public void Run() {
         try {
            _httpListener.Start() ;
         } catch (HttpListenerException e) {
            if (e.Message.Equals ("Access is denied")) {
               MainForm.View.AddMsgToList ("Watcher server must be run as Administrator.") ;
               MessageBox.Show ("Watcher server must be run as Administrator.") ;
            } else {
               MainForm.View.AddMsgToList ("Error while starting watcher server : " + e.Message) ;
            }
            throw ;
         }
         ThreadPool.QueueUserWorkItem ((o) => {
            MainForm.View.AddMsgToList ("Webserver running...") ;
            try {
               while (_httpListener.IsListening) {
                  ThreadPool.QueueUserWorkItem ((callback) => {
                     HttpListenerContext context = callback as HttpListenerContext ;
                     HandleRequest (context.Request, context) ;
                  }, _httpListener.GetContext()) ;
               }
            } catch (Exception e) {
               MainForm.View.AddMsgToList ("Error inside HttpServer:" + e) ;
            }
         }) ;
      }

      public void Stop() {
         _httpListener.Abort() ;
         _httpListener.Close() ;
      }

      private void HandleRequest (HttpListenerRequest request,
                                  HttpListenerContext context) {
         if (!request.HasEntityBody) {
            SendResponse (ConnectionVars.Content.INTERNAL_SYSTEM_ERROR, context) ;
            return ;
         }
         string requestString = GetRequestString (request) ;
         Dictionary<string, string> parsedResponse = ParseRequestStringToDictionary (requestString) ;
         if (parsedResponse == null) {
            SendResponse (ConnectionVars.Content.INTERNAL_SYSTEM_ERROR, context) ;
            return ;
         }


         if (Urls.WatcherServer.CHECK_RESULT.Contains (request.RawUrl)) {
            string result = CheckResult.Route (parsedResponse, _runnerX) ;
            if (result.Equals (ConnectionVars.Content.INTERNAL_SYSTEM_ERROR)) {
               MainForm.View.AddMsgToList ("Result of incoming /checkresult request: " + result) ;
               MainForm.View.AddMsgToList ("Request from MS: " + requestString) ;
            }
            SendResponse (result, context) ;
            return ;
         }

         if (Urls.WatcherServer.CREATE_ITSTATUS_PACKAGE.Contains (request.RawUrl)) {
            string result = Create.Route (parsedResponse, _runnerX) ;
            if (result.Equals (ConnectionVars.Content.INTERNAL_SYSTEM_ERROR)) {
               MainForm.View.AddMsgToList ("Result of incoming /package/create request: " + result) ;
               MainForm.View.AddMsgToList ("Request from MS: " + requestString) ;
            }
            SendResponse (result, context) ;
            return ;
         }
         if (Urls.WatcherServer.UPDATE_ITSTATUS_PACKAGE.Contains (request.RawUrl)) {
            string result = Update.Route (parsedResponse, _runnerX) ;
            if (result.Equals (ConnectionVars.Content.INTERNAL_SYSTEM_ERROR)) {
               MainForm.View.AddMsgToList ("Result of incoming /package/update request: " + result) ;
               MainForm.View.AddMsgToList ("Request from MS: " + requestString) ;
            }
            SendResponse (result, context) ;
         }
         if (Urls.WatcherServer.DELETE_PACKAGE.Contains (request.RawUrl)) {
            string result = Delete.Route (parsedResponse, _runnerX) ;
            if (result.Equals (ConnectionVars.Content.INTERNAL_SYSTEM_ERROR)) {
               MainForm.View.AddMsgToList ("Result of incoming /package/delete request: " + result) ;
               MainForm.View.AddMsgToList ("Request from MS: " + requestString) ;
            }
            SendResponse (result, context) ;
         }
      }

      private string GetRequestString (HttpListenerRequest request) {
         string result ;

         using (System.IO.Stream body = request.InputStream) // here we have data
         {
            using (System.IO.StreamReader reader = new System.IO.StreamReader (body, request.ContentEncoding)) {
               result = reader.ReadToEnd() ;
            }
         }
         return result ;
      }

      private Dictionary<string, string> ParseRequestStringToDictionary (string request) {
         // convert url encoded request to key value pair in a dictionary
         if (request == null) {
            return null ;
         }
         Dictionary<string, string> postParams = new Dictionary<string, string>() ;

         string[] rawParams = request.Split ('&') ;
         foreach (string param in rawParams) {
            string[] keyValuePair = param.Split ('=') ;
            string key = keyValuePair [0] ;
            string value = HttpUtility.UrlDecode (keyValuePair [1]) ;
            postParams.Add (key, value) ;
         }
         return postParams ;
      }

      private void SendResponse (string response,
                                 HttpListenerContext context) {
         try {
            byte[] buf = Encoding.UTF8.GetBytes (response) ;
            context.Response.ContentLength64 = buf.Length ;
            context.Response.OutputStream.Write (buf, 0, buf.Length) ;
         } catch (Exception e) {
            MainForm.View.AddMsgToList ("Error while sending http response:" + e) ;
         } finally {
            context.Response.OutputStream.Close() ;
         }
      }
   }
}
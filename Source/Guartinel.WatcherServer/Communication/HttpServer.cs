using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Net ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Communication.Routes ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues ;

namespace Guartinel.WatcherServer.Communication {
   /// <summary>
   /// HTTP server instance.
   /// </summary>
   public class HttpServer {
      public static class ParameterNames {
         // public const string CONTENT = "success" ;
      }

      public static class ResultNames {
         // public const string CONTENT = "success" ;
         // public const string ALERT_MESSAGE = "message" ;
      }

      public static class Contents {
         //public const string INVALID_TOKEN = "INVALID_TOKEN" ;
         //public const string CONFIG_VALID = "CONFIG_VALID" ;
         //public const string CONFIG_EXPIRED = "CONFIG_EXPIRED" ;
         //public const string DEVICE_NAME_TAKEN = "DEVICE_NAME_TAKEN" ;
         //public const string CANNOT_FIND_EMAIL = "CANNOT_FIND_EMAIL" ;
         //public const string INVALID_PASSWORD = "INVALID_PASSWORD" ;
         //public const string EMAIL_ALREADY_REGISTERED = "EMAIL_ALREADY_REGISTERED" ;
         // public const string SUCCESS = "SUCCESS" ;
         // public const string INVALID_REQUEST_PARAMETERS = "INVALID_REQUEST_PARAMETERS" ;
         //public const string BAD_REQUEST = "BAD_REQUEST" ;
         // public const string INTERNAL_SYSTEM_ERROR = "INTERNAL_SYSTEM_ERROR" ;
         //public const string INVALID_ACCOUNT_ID = "INVALID_ACCOUNT_ID" ;
         //public const string INVALID_DEVICE_UUID = "INVALID_DEVICE_UUID" ;
      }

      public static class Constants {
         public const string SLASH = "/" ;
      }

      public HttpServer() {}

      // Routes available in server
      private readonly Dictionary<string, Route> _routes = new Dictionary<string, Route>() ;

      /// <summary>
      /// Normalize route: remove starting and ending slashes
      /// </summary>
      /// <param name="route"></param>
      /// <returns></returns>
      protected string NormalizeRoutePath (string route) {
         string result = route ;
         if (string.IsNullOrEmpty (route)) { return route ; }

         if (route.StartsWith (Constants.SLASH, StringComparison.Ordinal)) {
            result = result.Remove (0, 1) ;
         }

         if (route.EndsWith (Constants.SLASH, StringComparison.Ordinal)) {
            result = result.Remove (result.Length - 1, 1) ;
         }

         return result ;
      }

      public void RegisterRoute (Route route) {
         // Store route
         _routes.Add (NormalizeRoutePath (route.Path), route) ;

         // Add route to HTTP server
         _prefixes.Add (ApplicationSettings.Use.GetFullRoutePath (route.Path)) ;

         Logger.Info ($"Route '{ApplicationSettings.Use.GetFullRoutePath (route.Path)}' added.") ;
      }

      private HttpRequestHandler _requestHandler ;
      private readonly List<string> _prefixes = new List<string>() ;

      public void Start() {
         try {
            //_httpListener.Prefixes.Add (_prefixes) ;
            //_httpListener.Prefixes.Add (Configuration.GetFullRoutePath (string.Empty)) ;
            //_httpListener.Start() ;

            _requestHandler = new HttpRequestHandler() ;
            _requestHandler.ListenAsynchronously (_prefixes, HandleRequest) ;
         } catch (Exception e) {
            string message ;
            if (e.Message.Equals ("Access is denied")) {
               message = "Guartinel Watcher Server must be run as Administrator." ;
            } else {
               message = $"Error while starting Guartinel Watcher Server. Message: {e.Message}" ;
            }

            Logger.Log (message) ;

            throw new ServerException (e, AllErrorValues.GENERAL_ERROR, message) ;
         }

         //ThreadPool.QueueUserWorkItem (dummy => {
         //   Logger.Log ("Webserver running...") ;
         //   try {
         //      while (_httpListener.IsListening) {
         //         ThreadPool.QueueUserWorkItem (context => {
         //            if (_httpListener.IsListening) {
         //               HttpListenerContext httpContext = context as HttpListenerContext ;
         //               if (httpContext != null) {
         //                  HandleRequest (httpContext.Request, httpContext) ;
         //               }
         //            }
         //         }, _httpListener.GetContext()) ;
         //      }
         //   } catch (Exception e) {
         //      Logger.Log ("Error inside HttpServer: " + e.Message) ;
         //   }
         //}) ;

         // _httpListener.BeginGetContext ()
      }

      //private async void Listen()
      // {
      //     while (true)
      //     {
      //         var context = await _httpListener.GetContextAsync();
      //         Console.WriteLine("Client connected");
      //         Task.Factory.StartNew(() => ProcessRequest(context));
      //     }

      //     _httpListener.Close();
      // }

      // private void ProcessRequest(HttpListenerContext context)
      // {
      //     System.Threading.Thread.Sleep(10*1000);
      //     Console.WriteLine("Response");
      // }

      public void Stop() {
         // _httpListener.Abort() ;

         //if (_httpListener != null) {
         //   _httpListener.Stop() ;
         //   _httpListener.Close() ;
         //}

         _requestHandler?.StopListening() ;
      }

      private void HandleRequest (Route route,
                                  Parameters request,
                                  Parameters results,
                                  string[] tags) {
         route.ProcessRequest (request, results, tags) ;
      }

      protected void HandleRequest (HttpListenerContext context) {         
         var logger = new TagLogger() ;
         HttpListenerRequest request = context.Request ;

         Parameters response = new Parameters() ;

         if (!request.HasEntityBody) {
            logger.Error ($"HTTP request arrived. URL: {request.RawUrl}. No body in HTTP request.") ;

            SendErrorResponse (response, context, AllErrorValues.NO_BODY_IN_REQUEST, logger.Tags) ;
            return ;
         }

         // Get request parameters
         string requestString = GetRequestString (request) ;
         logger.Info ($"HTTP request arrived. URL: {request.RawUrl}.") ;

         Parameters parsedRequest = ParseRequestString (requestString) ;

         if (parsedRequest == null) {
            logger.Error ($"Cannot parse HTTP request. Request string: {requestString}") ;

            SendErrorResponse (response, context, AllErrorValues.CANNOT_PARSE_REQUEST, logger.Tags) ;
            return ;
         }

         logger.Debug ($"Request string: {parsedRequest.AsJObject.ConvertToLog()}") ;

         // logger.Log ("Request parameters: {0}", parsedRequest.ToJson().ToString()) ;

         string routeURL = NormalizeRoutePath (request.RawUrl) ;

         // Check if the request is registered         
         if (!_routes.ContainsKey (routeURL)) {
            logger.Error ($"Invalid route '{request.RawUrl}'.") ;

            SendErrorResponse (response, context, AllErrorValues.INVALID_ROUTE, logger.Tags, new[] {request.RawUrl}) ;
            return ;
         }

         Route route = _routes [routeURL] ;

         if (route == null) {
            logger.Error ($"Route '{request.RawUrl}' in request not found.") ;

            SendResponse (response, context, logger.Tags) ;
            return ;
         }

         // Route found, pass request
         try {
            HandleRequest (route, parsedRequest, response, logger.Tags) ;
         } catch (ExpiredTokenException e) {
            // DO NOT log this exception
            SendErrorResponse (response, context, e.ErrorCode, logger.Tags, e.ErrorParameters) ;
            return ;
         } catch (ServerException e) {
            var errorParametersString = e.ErrorParameters.Concat (";") ;
            if (!string.IsNullOrEmpty (errorParametersString)) {
               errorParametersString = $" Error parameters: {errorParametersString}" ;
            }

            if (e is InvalidTokenException) {
               // No error in this case
               logger.Log (LogLevel.Info, $"Invalid token in request.") ;
            }
            if (e is ExpiredTokenException) {
               // No error in this case
               logger.Log (LogLevel.Info, $"Token expired in request.") ;
            } else {
               logger.Error ($"Server error when handling request. " +
                                $"Error code: {e.ErrorCode}{errorParametersString}") ;
            }

            SendErrorResponse (response, context, e.ErrorCode, logger.Tags, e.ErrorParameters) ;
            return ;
         } catch (Exception e) {
            logger.Error ($"Server error when handling request '{logger.Tags}'. " +
                             $"Error message: {e.GetAllMessages()}") ;
            SendErrorResponse (response, context, AllErrorValues.GENERAL_ERROR, logger.Tags, new[] {e.Message, e.GetAllMessages (false)}) ;
            return ;
         }

         SendResponse (response, context, logger.Tags) ;
      }

      /// <summary>
      /// Convert request to a simple string.
      /// </summary>
      /// <param name="request"></param>
      /// <returns></returns>
      private static string GetRequestString (HttpListenerRequest request) {
         string result = null ;

         using (System.IO.Stream body = request.InputStream) {
            using (System.IO.StreamReader reader = new System.IO.StreamReader (body, request.ContentEncoding)) {
               try {
                  result = reader.ReadToEnd() ;
               } catch (HttpListenerException) {
                  Logger.Error ($"HttpListenerException occurred during stream reading. Url: {request.RawUrl}.") ;
               }
            }
         }
         return result ;
      }

      /// <summary>
      /// Parse request string and extract named parameters.
      /// </summary>
      /// <param name="request"></param>
      /// <returns></returns>
      private static Parameters ParseRequestString (string request) {
         //if (string.IsNullOrEmpty (request)) return new Parameters() ; DTAP: commented out to prevent empty parameters result when the request is null

         JObject requestObject ;
         // One value: as a JSON object
         try {
            requestObject = JsonConvert.DeserializeObject<JObject> (request) ;
         } catch (Exception e) {
            Logger.Log ($"Error while deserializing HTTP request. Message: '{e.Message}'") ;
            return null ;
         }
         return new Parameters (requestObject) ;

         //string[] requestParemeters = request.Split (Constants.REQUEST_PARAMETER_SEPARATOR) ;
         //if (requestParemeters.Length == 0) return result ;

         //foreach (string parameter in requestParemeters) {
         //   string[] namedParameter = parameter.Split (Constants.REQUEST_NAME_VALUE_SEPARATOR) ;
         //   if (namedParameter.Length != 2) continue ;

         //   if (string.IsNullOrEmpty (namedParameter [0])) continue ;

         //   if (namedParameter [0].Equals (ParameterNames.CONTENT)) {
         //      string value = HttpUtility.UrlDecode (namedParameter [1]) ;
         //      result = new Parameters (value) ;
         //      return result ;
         //   }
         //}
         //return result ;
      }

      /// <summary>
      /// Send back response on HTTP.
      /// </summary>
      private static void SendResponse (Parameters results,
                                        HttpListenerContext context,
                                        string[] tags) {
         var logger = new TagLogger(tags);

         try {            
            // Logger.Log ($"Send response for request '{requestID}'. Result: {results.AsJObject}") ;
            logger.Debug ($"Send response for request. Result: {results.AsJObject.ConvertToLog (200)}") ;

            byte[] encodedResponse = Encoding.UTF8.GetBytes (results.ToString()) ;
            context.Response.ContentLength64 = encodedResponse.Length ;
            context.Response.OutputStream.Write (encodedResponse, 0, encodedResponse.Length) ;
            // context.Response.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json").ToString() ;
         } catch (Exception e) {
            logger.Error ($"Error sending response for request. Message: {e.Message}") ;
         } finally {
            context.Response.OutputStream.Close() ;
         }
      }

      /// <summary>
      /// Send error back response on HTTP.
      /// </summary>
      /// <param name="results"></param>
      /// <param name="context"></param>
      /// <param name="error">Internal error code (e.g. INVALID_TOKEN)</param>
      /// <param name="tags">Logger tags for tracking.</param>
      /// <param name="errorParameters">Parameters for error message.</param>
      private static void SendErrorResponse (Parameters results,
                                             HttpListenerContext context,
                                             string error,
                                             string[] tags,
                                             IEnumerable<string> errorParameters = null) {
         results.Error (error, errorParameters) ;

         SendResponse (results, context, tags) ;
      }
   }
}
using System;
using System.Linq;
using System.Net.Http ;
using System.Text;
using System.Threading ;
using System.Threading.Tasks ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel.Network {
   public class GlobalConfiguration : IDisposable {
      private GlobalConfiguration() {
         _httpClient = new HttpClient() ;
         _httpClient.Timeout = TimeSpan.FromSeconds (10) ;
      }

      private static readonly Lazy<GlobalConfiguration> _instance = new Lazy<GlobalConfiguration> (() => new GlobalConfiguration()) ;
      public static GlobalConfiguration Use => _instance.Value ;

      protected static class Constants {
         public static readonly string[] CONFIGURATION_SERVERS = {
                  "config8215.guartinel.com:5558",
                  "config7248.guartinel.com:5558",
                  "config1903.guartinel.com:5558"
         } ;
      }

      public static string GetConfigurationUrl (string server) => $"https://{server}/get/value" ;
      public static string GetHashUrl (string server) => $"https://{server}/get/hash";

      private readonly HttpClient _httpClient ;

      protected JObject CallForAvailableServer (Func<string, string> getURL,
                                                JObject requestData) {
         getURL.CheckNull();

         foreach (var server in Constants.CONFIGURATION_SERVERS) {
            try {
               var resultObject = CallConfigurationService (server, getURL, requestData) ;

               if (!resultObject.GetBooleanValue ("success", false)) {
                  continue ;
               }

               if (resultObject != null) {
                  return resultObject ;
               }
            } catch (Exception e) {
               // Ignore error
            }
         }

         Logger.Error ($"Error in call {getURL(String.Empty)} in Configuration Service.") ;
         throw new Exception ($"Error in call {getURL(String.Empty)} in Configuration Service.") ;
      }

      protected JObject CallConfigurationService (string server,
                                                  Func<string, string> getURL,
                                                  JObject requestData) {
         getURL.CheckNull() ;

         var content = new StringContent (requestData.ToString (Formatting.Indented), Encoding.UTF8, "application/json") ;
         HttpResponseMessage response = _httpClient.PostAsync (getURL(server), content).Result ;

         if (!response.IsSuccessStatusCode) {
            Logger.Error ($"Error calling configuration service. Code: {response.StatusCode}.") ;
            throw new Exception ($"Error calling configuration service.") ;
         }

         var resultObject = JObject.Parse (response.Content.ReadAsStringAsync().Result) ;
         return resultObject ;
      }

      public JObject Read (string key,
                           string token) {
         try {
            JObject requestData = new JObject();
            requestData[nameof(key).NameToJSONName()] = key;
            requestData[nameof(token).NameToJSONName()] = token;

            var callResult = CallForAvailableServer (GetConfigurationUrl, requestData) ;

            JObject result ;
            result = (JObject) callResult [nameof(result).NameToJSONName()] ;
            return result ;
         } catch {
            return new JObject();
         }
      }

      public void SubscribeForChange (string key,
                                      string token,
                                      int refreshIntervalSeconds,
                                      Action notification) {
         if (notification == null) return ;
         string lastHash = String.Empty ;

         try {
            // Use a timer to run a check for the change in the configuration
            Timer timer = new Timer (dummy => {
                                        JObject requestData = new JObject() ;
                                        requestData [nameof(key).NameToJSONName()] = key ;
                                        requestData [nameof(token).NameToJSONName()] = token ;

                                        JObject hashResult = CallForAvailableServer (GetHashUrl, requestData) ;
                                        var hash = hashResult.GetStringValue ("message") ;
                                        // If no hash, ignore!
                                        if (string.IsNullOrEmpty (hash)) return ;

                                        try {
                                           if (string.IsNullOrEmpty (lastHash)) return ;
                                           if (hash.Equals (lastHash)) return ;

                                           notification() ;
                                        } finally {
                                           lastHash = hash ;
                                        }
                                     },
                                     null,
                                     TimeSpan.Zero, TimeSpan.FromSeconds (refreshIntervalSeconds)) ;
         } catch {
            // Ignore error
            Logger.Error ("Cannot register change notification on Configuration Service.") ;
         }
      }

      //protected JObject ReadInt (string name,
      //                           string token) {

      //   HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create (Constants.CONFIGURATION_SERVER_URL) ;
      //   request.Method = Constants.REQUEST_METHOD ;
      //   request.ContentType = "application/json";
      //   using (var streamWriter = new StreamWriter (request.GetRequestStream())) {
      //      JObject requestData = new JObject() ;
      //      requestData [nameof(name).NameToJSONName()] = name ;
      //      requestData [nameof(token).NameToJSONName()] = token ;

      //      streamWriter.Write (requestData.ToString (Formatting.Indented)) ;
      //      streamWriter.Flush() ;
      //      streamWriter.Close() ;
      //   }

      //   string resultString;
      //   using (HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
      //      using (Stream dataStream = response.GetResponseStream()) {
      //         if (dataStream == null) return new JObject() ;
      //         using (StreamReader reader = new StreamReader (dataStream)) {
      //            resultString = reader.ReadToEnd() ;
      //            reader.Close() ;
      //            dataStream.Close() ;
      //         }
      //      }
      //   }

      //   var result = JObject.Parse (resultString) ;
      //   JObject value ;
      //   value = (JObject) result [nameof(value).NameToJSONName()] ;
      //   return value ;
      //}
      public void Dispose() {
         _httpClient.Dispose() ;
      }
   }
}
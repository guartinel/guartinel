using System;
using System.Net ;
using System.Net.Http ;
using System.Net.Security ;
using System.Security.Cryptography.X509Certificates ;
using System.Text ;
using System.Threading.Tasks;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel.Network {
   public class PostSender : IDisposable {
      public PostSender() {
         // Switch off certification check
         ServicePointManager.ServerCertificateValidationCallback += NoCertificationCheck ;
      }

      ~PostSender() {         
      }

      public void Dispose() {
         ServicePointManager.ServerCertificateValidationCallback -= NoCertificationCheck;

         if (_httpClient != null) {
            _httpClient.Dispose() ;
            _httpClient = null ;
         }
      }

      public static class Constants {
         public static class Send {
            public const string CONTENT_TYPE = "application/json" ;
         }
      }

      private bool NoCertificationCheck (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
         return true ;
      }

      private HttpClient _httpClient ;
      private readonly object _httpClientLock = new object() ;

      public JObject Post (string address,
                           JObject values,
                           int timeoutSeconds = 5,
                           int retryCount = 3,
                           int waitTimeSeconds = 5) {
         lock (_httpClientLock) {
            if (_httpClient == null) {
               _httpClient = new HttpClient() ;
            }
         }

         string stringContent = new ConfigurationData (values).ToString() ;

         var exceptionMessage = string.Empty ;

         retryCount = Math.Max (retryCount, 1) ;
         for (var retries = 0; retries < retryCount; retries++) {
            try {
               using (var httpContent = new StringContent (stringContent, Encoding.UTF8, Constants.Send.CONTENT_TYPE) ) {
                  // httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue (Constants.Send.CONTENT_TYPE) ;
                  using (var postResponse = _httpClient.PostAsync ($@"{address}", httpContent)) {
                     postResponse.Wait (TimeSpan.FromSeconds (timeoutSeconds)) ;                     

                     //if (timeoutSeconds <= 0) {
                     //   // var cancellationTokenSource = new CancellationTokenSource() ;
                     //   // cancellationTokenSource.Cancel(false);
                     //   postResponse.Wait() (1) ;
                     //} else {
                     //   postResponse.Wait (waitTimeSeconds * 1000) ;
                     //}

                     //// Check if wrong route
                     //if (postResponse.Result.StatusCode == HttpStatusCode.NotFound) {
                     //   Logger.Error ($"Invalid route '{address}'.") ;

                     //   throw new Exception (Common.AllErrorValues.INVALID_ROUTE, new[] { request.RawUrl }) ;
                     //}

                     if (!postResponse.Result.IsSuccessStatusCode) {                        
                        throw new Exception (postResponse.Result.ReasonPhrase) ;
                     }

                     using (var response = postResponse.Result.Content.ReadAsStringAsync()) {
                        response.Wait (TimeSpan.FromSeconds (timeoutSeconds)) ;

                        // Logger.Log($"Post returned from Management Server. {response.Result}") ;

                        if (string.IsNullOrEmpty (response.Result)) {
                           return new JObject() ;
                        }

                        if (response.Result.StartsWith (@"<!DOCTYPE html>", StringComparison.InvariantCultureIgnoreCase)) {
                           return new JObject();
                        }

                        return JObject.Parse (response.Result) ;
                     }
                  }
               }
            } catch (Exception e) {
               Logger.Error ($@"Error when posting request to {address}. Values: {values}. Error: {e.GetAllMessages()}") ;

               exceptionMessage = e.GetAllMessages() ;
               Task.Delay (TimeSpan.FromSeconds (waitTimeSeconds)).Wait() ;
            }            
         }

         throw new Exception (exceptionMessage) ;
      }
   }
}
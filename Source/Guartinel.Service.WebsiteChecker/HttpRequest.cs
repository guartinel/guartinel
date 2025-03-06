using System ;
using System.Diagnostics ;
using System.IO ;
using System.Linq ;
using System.Net ;
using System.Net.Security ;
using System.Security.Cryptography.X509Certificates ;
using System.Text ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Service.WebsiteChecker {
   public class HttpWebResponseResult : IDisposable {
      public HttpWebResponseResult (HttpWebResponse httpWebResponse) {
         if (httpWebResponse == null) return ;

         StatusCode = httpWebResponse.StatusCode ;
         HttpWebResponse = httpWebResponse ;
      }

      public HttpWebResponseResult (HttpStatusCode statusCode) {
         StatusCode = statusCode ;
      }

      public HttpStatusCode StatusCode {get ;} = HttpStatusCode.NotFound ;
      public HttpWebResponse HttpWebResponse {get ;}

      public void Dispose() {
         HttpWebResponse?.Dispose() ;
      }

      public void Close() {
         HttpWebResponse?.Close() ;
      }
   }

   public class HttpRequest : SiteDownloader {
      public HttpRequest() {
         ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications ;
      }

      ~HttpRequest() {
         ServicePointManager.ServerCertificateValidationCallback -= AcceptAllCertifications ;
      }

      private bool AcceptAllCertifications (object sender,
                                            X509Certificate certificate,
                                            X509Chain chain,
                                            SslPolicyErrors sslpolicyerrors) {
         return true ;
      }

      private static HttpWebResponseResult GetResponse (HttpWebRequest request) {
         // Need to handle exception for 302 - issue in .net Core implementation
         // https://github.com/dotnet/corefx/issues/19183
         try {
            return new HttpWebResponseResult ((HttpWebResponse) request.GetResponse()) ;
         } catch (WebException e) {
            Logger.Debug ($"Web exception when getting response. Type: {e.GetType()}. Messages: {e.GetAllMessages()}") ;

            var response = e.Response ;
            if (response == null) throw ;
            if (!(response is HttpWebResponse)) throw ;

            return new HttpWebResponseResult (response as HttpWebResponse) ;
         } catch (Exception e) {
            Logger.Debug ($"Exception when getting response. Type: {e.GetType()}. Messages: {e.GetAllMessages()}") ;
            throw ;
         }
      }

      public SiteProperties GetSiteProperties (Website website,
                                               int? timeoutSeconds,
                                               string userAgent) {
         userAgent = string.IsNullOrEmpty (userAgent) ? Constants.DEFAULT_USER_AGENT : userAgent ;
         HttpWebRequest request = GetSiteRequest (website, true, Constants.MAX_TIMEOUT_SECONDS, userAgent) ;

         // Just touch the website, do not try to get its contents
         var maxRedirections = Constants.MAX_REDIRECTIONS ;
         while (true) {
            using (HttpWebResponseResult response = GetResponse (request)) {
               if (response.StatusCode == HttpStatusCode.OK) {
                  return new SiteProperties (GetCertificateExpiryDate (request)) ;
               }

               if (response.StatusCode == HttpStatusCode.Found ||
                   response.StatusCode == HttpStatusCode.Moved ||
                   response.StatusCode == HttpStatusCode.MovedPermanently) {
                  if (maxRedirections == 0) {
                     Logger.Error ($"No more redirects possible for '{request.Address}'.") ;

                     throw new Exception ($"Too many redirects ({Constants.MAX_REDIRECTIONS}) for '{request.Address}'.") ;
                  }

                  maxRedirections-- ;

                  string location = request.Address.ToString() ;
                  var webResponse = response.HttpWebResponse ;
                  if (webResponse == null) {
                     Logger.Debug ($"HttpWebResponse is null for {request.Address}.") ;
                  } else {
                     location = response.HttpWebResponse.Headers ["Location"] ;
                     Logger.Debug ($"HttpWebResponse is NOT null for {request.Address}, header Location: {location}.") ;
                  }

                  Logger.Info ($"Manual redirecting from '{request.Address}' to '{location}'. WebResponse is null: {response.HttpWebResponse == null}.") ;

                  response.Close() ;

                  request = GetSiteRequest (new Website (location, website.Caption), false,
                                            Constants.MAX_TIMEOUT_SECONDS, userAgent) ;

                  continue ;
               }

               // return new SiteDownloadResult.Fail (website, response.StatusDescription) ;
               throw new Exception (response.HttpWebResponse.StatusDescription) ;
            }
         }
      }

      public DateTime? GetCertificateExpiryDate (HttpWebRequest request) {
         if (request.Address.Scheme != Constants.HTTPS_PREFIX) return null ;

         try {
            // Retrieve the ssl cert and assign it to an X509Certificate object
            X509Certificate certificate = request.ServicePoint.Certificate ;
            if (certificate == null) return null ;

            // Convert the X509Certificate to an X509Certificate2 object
            X509Certificate2 certificate2 = new X509Certificate2 (certificate) ;
            try {
               return DateTime.Parse (certificate2.GetExpirationDateString()) ;
            } catch {
               Logger.Log ($"Cannot parse SSL certificate expiry date. Site: {request.Address}. Date reported: {certificate2.GetExpirationDateString()}.") ;

               // Ignore error
               return null ;
            }
         } catch (Exception e) {
            Logger.Log ($"Error when getting certificate expiry date. Site: {request.Address}. Message: {e.GetAllMessages()}.") ;

            return null ;
         }
      }

      public static HttpWebRequest GetSiteRequest (Website website,
                                                   bool allowAutoRedirect,
                                                   int timeoutSeconds,
                                                   string userAgent) {
         HttpWebRequest request = WebRequest.CreateHttp (website.Address) ;

         request.AllowAutoRedirect = allowAutoRedirect ;
         request.CookieContainer = new CookieContainer() ;
         request.UserAgent = userAgent ;
         // request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.30; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
         request.Timeout = timeoutSeconds * 1000 ;

         // SZTZ: test!
         request.ProtocolVersion = HttpVersion.Version10 ;
         // request.KeepAlive = false;

         return request ;
      }

      protected override SiteDownloadResult DownloadSite1 (Website website,
                                                           int? timeoutSeconds,
                                                           string userAgent,
                                                           string searchInPage) {
         try {
            Stopwatch stopwatch = new Stopwatch() ;
            stopwatch.Start() ;
            using (HttpWebResponse response = (HttpWebResponse) GetSiteRequest (website, true, timeoutSeconds ?? Constants.MAX_TIMEOUT_SECONDS, userAgent).GetResponse()) {
               Logger.Debug ($"Site checker request created for '{website.Address}'.") ;

               // Now look to see if it's a redirect
               //if ((int) response.StatusCode >= 300 && (int) webResponse.StatusCode <= 399) {
               //   return response.Headers ["Location"] ;
               //}

               Logger.Debug ($"Web response arrived for '{website.Address}'. Code: {response.StatusCode}. Description: {response.StatusDescription}") ;

               if (response.StatusCode != HttpStatusCode.OK) {
                  // return new SiteDownloadResult.Fail (website, response.StatusDescription) ;
                  return new SiteDownloadResult (website, "Website reported an error.", response.StatusDescription) ;
               }

               using (Stream receiveStream = response.GetResponseStream()) {
                  if (receiveStream == null) {
                     // return new SiteDownloadResult.Fail (website, response.StatusDescription) ;
                     return new SiteDownloadResult (website, "Website reported an error.", response.StatusDescription) ;
                  }

                  using (StreamReader readStream = string.IsNullOrEmpty (response.CharacterSet) ? new StreamReader (receiveStream) :
                           new StreamReader (receiveStream, EncodingEx.GetEncoding (response.CharacterSet))) {
                     string content = readStream.ReadToEnd() ;

                     Logger.Log (LogLevel.Info, $"Site '{website.DisplayText}' is available.") ;
                     // return new SiteDownloadResult.Success (website, stopwatch.ElapsedMilliseconds, content, certificateExpiryDate) ;
                     return new SiteDownloadResult (website, stopwatch.ElapsedMilliseconds, content, null) ;
                  }
               }
            }
         } catch (Exception e) {
            Logger.Log ($"Error accessing website '{website.DisplayText}'. Message: {e.GetAllMessages()}") ;

            // return new SiteDownloadResult.Fail (website, e.Message) ;
            return new SiteDownloadResult (website, e.Message, e.GetAllMessages (false)) ;
         }
      }
   }
}

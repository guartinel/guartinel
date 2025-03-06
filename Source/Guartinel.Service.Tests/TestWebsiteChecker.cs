using System ;
using System.Threading ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Service.MessageQueues ;
using Guartinel.Kernel.Utility ;
using Guartinel.Service.WebsiteChecker ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.Service.Tests {
   [TestFixture]
   public class WebsiteCheckerTests {
      [SetUp]
      public void Setup() { }

      [TearDown]
      public void TearDown() { }

      [Test]
      public void TestInstantiateServer() {
         using (var application = new Application()) {
            application.Run() ;
         }
      }

      [Test]
      public void CheckWebsiteCheckerServer() {
         using (var application = new Application()) {
            application.Run() ;
            new TimeoutSeconds (20).Wait() ;

            using (MessageConnection messageConnection = SetupMessageQueue()) {
               // const string ADDRESS = @"https://www.google.com" ;
               // const string CAPTION = @"Gooooooogle" ;

               // const string ADDRESS = @"https://www.sysment.hu";
               //const string ADDRESS = @"https://www.index.hu";
               // const string ADDRESS = @"https:www.papapapa.fr" ;
               const string ADDRESS = @"https://boragen-ps.chemaxon.com/";
               const string CAPTION = @"Sysment" ;

               // Send request
               using (var client = messageConnection.CreateServiceClient ("WebsiteChecker.1.0", null)) {
                  var message = new SiteDownloadRequest (new Website (ADDRESS, CAPTION), "ChromeRemote", 10, 1, 1, string.Empty) ;

                  var resultFromService = client.CallSync (message.AsJObject(), CancellationToken.None, new[] {TagLogger.CreateTag ("website", "www.index.hu")}) ;

                  SiteDownloadResult result = SiteDownloadResult.FromJObject (resultFromService) ;
                  Assert.IsTrue (result.Success == SiteDownloadResultSuccess.Success, result.Message) ;
                  Logger.Info ($"Downloading {ADDRESS} was successful, download time {result.LoadTimeMilliseconds} ms.") ;
               }
            }
         }
      }

      //[Test]
      //public void CheckWebsiteWithApp () {
      //   ConcurrentQueue<string> results = new ConcurrentQueue<string>();

      //   // Start website checker app
      //   using (var application = new WebsiteChecker.Application()) {
      //      application.Run();

      //      new Timeout(TimeSpan.FromSeconds(5)).Wait();

      //      // Send request
      //      using (var queueConnection = new MessageQueueConnection()) {
      //         queueConnection.Connect();
      //         var messageQueue = new MessageQueue();
      //         messageQueue.Configure(queueConnection, "Test11");

      //         var resultMessageQueue = new MessageQueue();
      //         resultMessageQueue.Configure(queueConnection, "Test11Result");
      //         resultMessageQueue.RegisterMessageHandler(message => {
      //            results.Enqueue(message);
      //         });

      //         // Send request
      //         messageQueue.Send ("https://www.index.hu") ;

      //         // Wait
      //         new Timeout(TimeSpan.FromSeconds(20)).WaitFor(() => results.Count > 0);
      //         Assert.IsTrue (results.TryDequeue (out string messageGot)) ;
      //         Assert.IsTrue (messageGot.StartsWith ("Success"), messageGot) ;
      //      }
      //   }
      //}

      [Test]
      public void CheckWebsiteViaQueue() {
         using (MessageConnection messageConnection = SetupMessageQueue()) {
            // const string ADDRESS = @"https://www.google.com" ;
            // const string CAPTION = @"Gooooooogle" ;

            const string ADDRESS = @"https://www.sysment.hu" ;
            //const string ADDRESS = @"https://www.index.hu";
            const string CAPTION = @"Sysment" ;

            // Send request
            using (var client = messageConnection.CreateServiceClient ("WebsiteChecker.1.0", null)) {
               var message = new SiteDownloadRequest (new Website (ADDRESS, CAPTION), "ChromeRemote", 10, 1, 1, String.Empty) ;

               var resultFromService = client.CallSync (message.AsJObject(), CancellationToken.None, new[] {"test1"}) ;

               SiteDownloadResult result = SiteDownloadResult.FromJObject (resultFromService) ;
               Assert.IsTrue (result.Success == SiteDownloadResultSuccess.Success, result.Message) ;
               Logger.Info ($"Downloading {ADDRESS} was successful, download time {result.LoadTimeMilliseconds} ms.") ;
            }
         }
      }

      private MessageConnection SetupMessageQueue() {
         JObject configuration = GlobalConfiguration.Use.Read (@"Guartinel/WebsiteChecker.1.0/SzTZDevelopment", "68e13ac4-3ac8-4f6a-a75a-e147a579d89c") ;
         string queueServiceAddress = configuration.GetStringValue ("QueueServiceAddress") ;
         string queueServiceUserName = configuration.GetStringValue ("QueueServiceUserName") ;
         string queueServicePassword = configuration.GetStringValue ("QueueServicePassword") ;
         return new MessageConnection (queueServiceAddress, queueServiceUserName, queueServicePassword) ;
      }

      //[Test]
      //public void TestWebSiteWithPort () {
      //   WebsiteDownloadResult result = new SiteDownloader().DownloadSite(new Website("https://backend.guartinel.com:9090"), SiteDownloadKind.Chrome);
      //   Assert.IsInstanceOf<WebsiteDownloadResult.Success>(result);
      //}


      protected static bool IsValidWebsiteUri (string website) {
         return Uri.TryCreate(website, UriKind.Absolute, out var uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
      }

      protected static string BuildUri (string website,
                                        string scheme) {
         try {
            var builder = new UriBuilder(website);
            builder.Scheme = scheme;
            return builder.Uri.ToString();
         } catch (UriFormatException) {
            throw new Exception($"The address format of '{website}' is invalid. Use a valid website or IP address (like https://www.guartinel.com).");
         }
      }

      protected static string CorrectWebsiteSyntax (string website) {
         const string HTTP_PREFIX = "http";
         const string HTTPS_PREFIX = "https";

         if (IsValidWebsiteUri(website)) return website;

         // Try to add http
         var correctedWebsite = BuildUri(website, HTTP_PREFIX);
         if (IsValidWebsiteUri(correctedWebsite)) return correctedWebsite;

         // Try to add https
         correctedWebsite = BuildUri(website, HTTPS_PREFIX);
         if (IsValidWebsiteUri(correctedWebsite)) return correctedWebsite;

         return website;
      }

      [Test]
      public void TestWebSite() {
         const string DEFAULT_USER_AGENT = @"Mozilla / 5.0(compatible; Guartinel / 1.0) (+https://www.guartinel.com/about-us)" ;
         var correctedWebsite = new Website (CorrectWebsiteSyntax(@"https://boragen-compreg.chemaxon.com/RegistryCxn/"), "Test") ;

         new HttpRequest().GetSiteProperties (correctedWebsite,
                                              0,
                                              DEFAULT_USER_AGENT) ;
      }

      //[Test]
      //public void TestUtf8WebSiteWithHttpWebRequest () {
      //   WebsiteDownloadResult result = new SiteDownloader().DownloadSite(new Website("facebook.com"), SiteDownloadKind.HttpWebRequest);
      //   Assert.IsInstanceOf<WebsiteDownloadResult.Success>(result);
      //}

      //[Test]
      //public void TestSiteWithHttpsWithHttpWebRequest () {
      //   WebsiteDownloadResult result = new SiteDownloader().DownloadSite(new Website("https://sso.secureserver.net/"), SiteDownloadKind.HttpWebRequest);
      //   Assert.IsInstanceOf<WebsiteDownloadResult.Success>(result, (result as WebsiteDownloadResult.Fail)?.Message);
      //}

      //[Test]
      //public void CheckHttpsCertification () {
      //   WebsiteDownloadResult result = new SiteDownloader().DownloadSite(new Website("http://www.index.hu"), SiteDownloadKind.Chrome);
      //   Assert.IsInstanceOf<WebsiteDownloadResult.Success>(result);
      //   Assert.Greater(((WebsiteDownloadResult.Success) result).CertificateExpiryDate, new DateTime(2020, 07, 23, 0, 0, 0));

      //   WebsiteDownloadResult result2 = new SiteDownloader().DownloadSite(new Website("https://www.sysment.hu"), SiteDownloadKind.Chrome, 5, 500);
      //   Assert.IsInstanceOf<WebsiteDownloadResult.Success>(result2);
      //   Assert.Less(((WebsiteDownloadResult.Success) result2).CertificateExpiryDate, new DateTime(2021, 01, 12, 0, 0, 0));
      //   Assert.Greater(((WebsiteDownloadResult.Success) result2).CertificateExpiryDate, new DateTime(2020, 07, 10, 0, 0, 0));
      //}
   }
}

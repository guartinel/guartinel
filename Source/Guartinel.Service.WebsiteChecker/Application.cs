using System;
using System.Text;
using Guartinel.Kernel;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.WebsiteChecker {
   public class Application : ApplicationBase {
      public Application() {
         IoC.Use.Single.RegisterLazy<IApplicationSettingsReader> (() => new ApplicationSettingsReaderGlobal (ServiceName)) ;

         IoC.Use.Multi.Register<SiteDownloader> (() => new HttpRequest(), nameof(HttpRequest)) ;
         IoC.Use.Multi.Register<SiteDownloader> (() => new Chrome(), nameof(Chrome)) ;
         IoC.Use.Multi.Register<SiteDownloader> (() => new ChromeRemote(), nameof(ChromeRemote)) ;
      }

      public static class Constants {
         public const int DEFAULT_TRY_COUNT = 3 ;
         public const int DEFAULT_RETRY_WAITING_SECONDS = 5 ;
         public const int OVERALL_TRY_COUNT = 20 ;
      }

      protected override string Name => "WebsiteChecker" ;
      protected override string QueueServiceAddress => ApplicationSettings.Use.QueueServiceAddress ;
      protected override string QueueServiceUserName => ApplicationSettings.Use.QueueServiceUserName ;
      protected override string QueueServicePassword => ApplicationSettings.Use.QueueServicePassword ;

      protected override JObject ProcessRequest1 (JObject request) {
         SiteDownloadResult result = null ;

         Logger.Info ($"Website check request arrived. Data: {request.ToString (Formatting.Indented)}") ;
         SiteDownloadRequest downloadRequest = SiteDownloadRequest.FromJObject (request) ;

         var downloaderType = downloadRequest.DownloadType ;
         var downloader = IoC.Use.Multi.GetInstance<SiteDownloader> (downloaderType) ;

         var tryCount = downloadRequest.TryCount ?? Constants.DEFAULT_TRY_COUNT ;
         var retryWaitingSeconds = downloadRequest.RetryWaitingSeconds ?? Constants.DEFAULT_RETRY_WAITING_SECONDS;
         int tryIndex = 0 ;
         int overallTryIndex = 0 ;

         while (tryIndex < tryCount &&
                overallTryIndex < Constants.OVERALL_TRY_COUNT) {
            try {
               result = downloader.DownloadSite (downloadRequest.Website,
                                                 downloadRequest.TimeoutSeconds,
                                                 downloadRequest.SearchInPage) ;
            } catch (Exception e) {
               Logger.InfoWithDebug ($"Cannot access website '{downloadRequest.Website.Caption}'. Message: {e.Message}", e.GetAllMessages ());
               result = new SiteDownloadResult (downloadRequest.Website, $"Cannot access website '{downloadRequest.Website.Caption}'.", e.Message) ;
            }

            // Success, get out!
            if (result.Success == SiteDownloadResultSuccess.Success) break ;

            new TimeoutSeconds (retryWaitingSeconds).Wait() ;

            if (result.Success == SiteDownloadResultSuccess.Error) {
               tryIndex++ ;
            }
            overallTryIndex++ ;
         }

         return result?.AsJObject() ;
      }
   }
}
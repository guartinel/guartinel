using System;
using System.Linq;
using System.Text;
using System.Threading ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.Service.MessageQueues ;
using Guartinel.Service.WebsiteChecker ;

namespace Guartinel.WatcherServer.Supervisors.WebsiteSupervisor {
   public class WebsiteCheckSender : IWebsiteCheckSender {
      public void SendRequest (string name,
                               SiteDownloadRequest request,
                               Action<SiteDownloadResult, string[]> resultArrived,
                               CancellationToken cancellation,
                               TimeSpan timeout,
                               string[] tags) {
         // Canel after timeout or cancelled by caller
         var cancellationWithTimeout = new CancellationTokenSource() ;
         new CancellationTokenSource().CancelAfter (timeout) ;
         var cancellationAggregate = CancellationTokenSource.CreateLinkedTokenSource (cancellation, cancellationWithTimeout.Token).Token ;

         var logger = new TagLogger (tags) ;
         var connection = IoC.Use.Single.GetInstance<IMessageConnection>() ;
         logger.Info ("Create website check client.") ;

         logger.InfoWithDebug ("Calling website check.", $"Request: {request.AsJObject().ConvertToLog()}") ;
         connection.CallServiceClient (name, request.AsJObject(), result => {
            if (cancellation.IsCancellationRequested) return ;

            logger.InfoWithDebug ("Website check returned.", result.ConvertToLog (200)) ;
            SiteDownloadResult siteDownloadResult = SiteDownloadResult.FromJObject (result) ;
            logger.Info ($"Website check result: {siteDownloadResult.Success}. Load time (ms): {siteDownloadResult.LoadTimeMilliseconds}") ;

            resultArrived?.Invoke (siteDownloadResult, tags) ;
         }, cancellationAggregate, logger.Tags) ;
      }
   }
}
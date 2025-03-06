using System ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using Guartinel.Service.WebsiteChecker ;
using Guartinel.WatcherServer.Supervisors.WebsiteSupervisor ;

namespace Guartinel.WatcherServer.Tests {
   public class WebsiteCheckSenderMock : IWebsiteCheckSender {
      public void SendRequest (string name,
                               SiteDownloadRequest request,
                               Action<SiteDownloadResult, string[]> resultArrived,
                               CancellationToken cancellation,
                               TimeSpan timeout,
                               string[] tags) {
         throw new NotImplementedException() ;
      }
   }
}

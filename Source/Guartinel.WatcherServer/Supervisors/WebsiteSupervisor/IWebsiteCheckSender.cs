using System;
using System.Linq;
using System.Text;
using System.Threading ;
using Guartinel.Service.WebsiteChecker ;

namespace Guartinel.WatcherServer.Supervisors.WebsiteSupervisor {
   public interface IWebsiteCheckSender {
      void SendRequest (string name,
                        SiteDownloadRequest request,
                        Action<SiteDownloadResult, string[]> resultArrived,
                        CancellationToken cancellation,
                        TimeSpan timeout,
                        string[] tags) ;
   }
}
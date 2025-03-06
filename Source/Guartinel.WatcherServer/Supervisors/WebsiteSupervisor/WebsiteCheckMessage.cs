using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;

namespace Guartinel.WatcherServer.Supervisors.WebsiteSupervisor {
   public class WebsiteCheckMessage {
      public WebsiteCheckMessage (string packageID,
                                  string website,
                                  bool success,
                                  long? loadingTimeMilliSeconds,
                                  DateTime? certificateExpiryDate,
                                  XString message,
                                  XString details) {
         PackageID = packageID ;
         Website = website ;
         Success = success ;
         LoadingTimeMilliSeconds = loadingTimeMilliSeconds ;
         CertificateExpiryDate = certificateExpiryDate ;
         Message = message ;
         Details = details ;
      }

      public readonly string PackageID ;
      public readonly string Website ;
      public readonly bool Success ;
      public readonly long? LoadingTimeMilliSeconds ;
      public readonly DateTime? CertificateExpiryDate ;
      public readonly XString Message ;
      public readonly XString Details ;

      public static class Constants { }
   }
}
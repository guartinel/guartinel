using System;
using System.Linq;
using System.Text;

namespace Guartinel.Service.WebsiteChecker {
   public class SiteProperties {
      public SiteProperties (DateTime? certificateExpiryDate) {
         CertificateExpiryDate = certificateExpiryDate ;
      }

      public DateTime? CertificateExpiryDate {get ;}
   }
}
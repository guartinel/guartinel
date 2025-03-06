using Guartinel.Communication.Languages ;

namespace Guartinel.Communication.Supervisors.WebsiteSupervisor.Languages {
   public class English : EnglishBase, Strings.IMessages {
      // public const string ALERT = "There is something wrong with #WEB_SITE_ADDRESS#. #WEBSITE_IS_NOT_AVAILABLE#" ;
      // public const string RECOVERY_ALERT = "Website #WEB_SITE_ADDRESS#, is available again." ;
      // public const string CANNOT_READ_FROM_WEBSITE = "Cannot read data from website #WEB_SITE_ADDRESS#." ;

      public string WebsiteIsNotAvailable => $"Website '{Parameter (Strings.Parameters.Website)}', is not accessible." ;
      public string CheckTextPatternFailed => $"Website '{Parameter (Strings.Parameters.Website)}' does not contain '{Parameter (Strings.Parameters.CheckTextPattern)}'." ;
      public string WebsiteLoadTimeTooMuch => $"Website '{Parameter (Strings.Parameters.Website)}' load time is {Parameter (Strings.Parameters.WebSiteLoadTimeSeconds)} seconds, which is higher than the defined value." ;
      public string CheckCertificateMinDaysFailed => $"Website '{Parameter (Strings.Parameters.Website)}' certificate expires in {Parameter (Strings.Parameters.CertificateExpiryDays)} days." ;
      public string CertificateExpired => $"Website '{Parameter (Strings.Parameters.Website)}' certificate is expired on {Parameter (Strings.Parameters.CertificateExpiryDate)}." ;
      public string WebsiteIsOKMessage => $"Website '{Parameter (Strings.Parameters.Website)}' is accessible." ;
      public string WebsiteIsOKDetails => $"Website '{Parameter(Strings.Parameters.Website)}' is accessible in {Parameter(Strings.Parameters.WebSiteLoadTimeSeconds)} seconds." ;
      public string WebsiteIsOKDetailsExpireDate => $"Certificate expires on '{Parameter (Strings.Parameters.CertificateExpiryDate)}'." ;
      public string WebsiteIsOKExtract => $"Website is accessible in {Parameter(Strings.Parameters.WebSiteLoadTimeSeconds)} seconds. " ;
      public string WebsiteIsOKExtractExpireDate => $"Expires on {Parameter (Strings.Parameters.CertificateExpiryDate)}." ;
      // public string WebsiteIsOKDetails => $"{Parameter(Strings.Parameters.Details)}";
      public string WebsiteCheckErrorMessage => $"Error when checking access to website '{Parameter (Strings.Parameters.Website)}'." ;
      public string WebsiteCheckErrorDetails => $"{Parameter(Strings.Parameters.ErrorMessage)}" ;
      public string WebsiteCheckErrorExtract => $"Website access error." ;
      public string WebsiteCheckErrorsMessage => $"Website '{Parameter(Strings.Parameters.Website)}' has multiple check errors, please see details.";
      public string WebsiteCheckErrorsDetails => $"{Parameter(Strings.Parameters.ErrorMessage)}";
      public string WebsiteCheckErrorsExtract => $"Website is not accessible." ;
   }
}
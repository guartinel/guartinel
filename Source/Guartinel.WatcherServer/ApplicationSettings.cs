using System;
using System.Linq;
using Guartinel.Kernel ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.WatcherServer {
   public class ApplicationSettings : ApplicationSettingsBase<ApplicationSettings> {
      public string ServerAddress {
         get => Data.GetStringValue(nameof(ServerAddress)) ;
         set => Data[nameof(ServerAddress)] = value ;
      }      

      public string ServerID {
         get => Data.GetStringValue (nameof(ServerID)) ;
         set => Data [nameof(ServerID)] = value ;
      }

      public string ManagementServerAddress {
         get => Data.GetStringValue (nameof(ManagementServerAddress)) ;
         set => Data[nameof(ManagementServerAddress)] = value ;
      }

      public string ManagementServerID {
         get => Data.GetStringValue (nameof(ManagementServerID)) ;
         set => Data[nameof(ManagementServerID)] = value ;
      }

      public string ManagementServerUID {
         get => Data.GetStringValue (nameof(ManagementServerUID)) ;
         set => Data[nameof(ManagementServerUID)] = value ;
      }

      public string ManagementServerRegistrationToken {
         get => Data.GetStringValue (nameof(ManagementServerRegistrationToken)) ;
         set => Data[nameof(ManagementServerRegistrationToken)] = value ;
      }

      public string RegistrationUserName {
         get => Data.GetStringValue (nameof(RegistrationUserName)) ;
         set => Data[nameof(RegistrationUserName)] = value ;
      }

      public string RegistrationPasswordHash {
         get => Data.GetStringValue (nameof(RegistrationPasswordHash)) ;
         set => Data[nameof(RegistrationPasswordHash)] = value ;
      }

      public int TokenExpirySeconds {
         get => Data.GetIntegerValue (nameof(TokenExpirySeconds), 3 * 60 * 60) ;
         set => Data[nameof(TokenExpirySeconds)] = value ;
      }

      public int WatcherRunnerCount => Data.GetIntegerValue (nameof(WatcherRunnerCount), 10) ;
      public int CheckersPerPackage => Data.GetIntegerValue (nameof(CheckersPerPackage), 4) ;

      public string WebsiteChecker {
         get => Data.GetStringValue (nameof(WebsiteChecker)) ;
         set => Data[nameof(WebsiteChecker)] = value ;
      }

      //public int WebsiteCheckRequestCount {
      //   get => Data.GetIntegerValue (nameof(WebsiteCheckRequestCount), 2) ;
      //   set => Data [nameof(WebsiteCheckRequestCount)] = value ;
      //}

      public int WebsiteCheckerPoolSize => Data.GetIntegerValue (nameof(WebsiteCheckerPoolSize), 10) ;

      // public int DefaultWebsiteTimeoutSeconds => _data.GetIntegerValue (nameof(DefaultWebsiteTimeoutSeconds), 300) ;

      public Categories Categories => new Categories (ToList (Data.GetStringValue (nameof(Categories)))) ;

      public int Decimals => Data.GetIntegerValue (nameof(Decimals), 2) ;

      #region Email checker

      public string EmailCheckerEmailAddress => Data.GetStringValue (nameof(EmailCheckerEmailAddress), "test@sysment.net") ;
      public string EmailCheckerFingerprintToken => Data.GetStringValue (nameof(EmailCheckerFingerprintToken), "774ed86f-4af4-4f91-b1a7-bdde48c9314c") ;
      public string EmailCheckerSmtpServerAddress => Data.GetStringValue (nameof(EmailCheckerSmtpServerAddress), "mail.sysment.net") ;
      public int EmailCheckerSmtpServerPort => Data.GetIntegerValue (nameof(EmailCheckerSmtpServerPort), 465) ;
      public bool EmailCheckerSmtpUseSSL => Data.GetBooleanValue (nameof(EmailCheckerSmtpUseSSL), true) ;

      public string EmailCheckerSmtpUser => Data.GetStringValue (nameof(EmailCheckerSmtpUser), "test@sysment.net") ;
#warning SzTZ: needs encoding!
      public string EmailCheckerSmtpPassword => Data.GetStringValue (nameof(EmailCheckerSmtpPassword), "sX6lA6y5w7M12DYZnT6g") ;

      public string EmailCheckerImapServerAddress => Data.GetStringValue (nameof(EmailCheckerImapServerAddress), "mail.sysment.net") ;
      public int EmailCheckerImapServerPort => Data.GetIntegerValue (nameof(EmailCheckerImapServerPort), 993) ;
      public bool EmailCheckerImapUseSSL => Data.GetBooleanValue (nameof(EmailCheckerImapUseSSL), true) ;
      public string EmailCheckerImapUser => Data.GetStringValue (nameof(EmailCheckerImapUser), "test@sysment.net") ;
#warning SzTZ: needs encoding!
      public string EmailCheckerImapPassword => Data.GetStringValue (nameof(EmailCheckerImapPassword), "sX6lA6y5w7M12DYZnT6g") ;

      public string[] EmailCheckerSpamFolderNames => Data.AsStringArray (nameof(EmailCheckerSpamFolderNames), new[] {"spam", "junk"}) ;
      #endregion

      public string GetFullRoutePath (string route) {
         var result = $"{ServerAddress}{route}" ;
         return StringEx.EnsureEnds (result, @"/") ;
      }
   }
}
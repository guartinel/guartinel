using System;
using System.Linq;
using System.Text;
using Guartinel.Communication ;
using Guartinel.WatcherServer.Supervisors.EmailSupervisor ;
using Newtonsoft.Json.Linq ;
using SaveConstants = Guartinel.Communication.Supervisors.EmailSupervisor.Strings.WatcherServerRoutes.Save;

namespace Guartinel.WatcherServer.Tests.Supervisors.EmailSupervisor {
   public static class Configuration {

      public static void ConfigureChecker (EmailChecker checker,
                                           string packageID,
                                           string testEmailAddress,
                                           int? retryCount = 4,
                                           int? waitTimeSeconds = 5) {

         checker.Configure ("checker1", packageID, "test2@sysment.info") ;
      }

      public static void CreatePackageConfiguration (JObject configuration,
                                                     SmtpConfiguration smtpConfiguration,
                                                     ImapConfiguration imapConfiguration,
                                                     string testEmailAddress,
                                                     int timeoutInSeconds = 60) {
         if (configuration == null) return ;
         configuration [WatcherServerAPI.Packages.Save.Request.TIMEOUT_INTERVAL_SECONDS] = timeoutInSeconds.ToString() ;

         configuration [SaveConstants.Request.Configuration.TEST_EMAIL_ADDRESS] = testEmailAddress ;

         var smtp = new JObject() ;
         smtp [SaveConstants.Request.Configuration.Imap.USER] = smtpConfiguration.User ;
         smtp [SaveConstants.Request.Configuration.Imap.PASSWORD] = smtpConfiguration.Password ;
         smtp [SaveConstants.Request.Configuration.Imap.SERVER_PORT] = smtpConfiguration.ServerPort ;
         smtp [SaveConstants.Request.Configuration.Imap.SERVER_ADDRESS] = smtpConfiguration.ServerAddress ;
         smtp [SaveConstants.Request.Configuration.Imap.USE_SSL] = smtpConfiguration.UseSSL ;
         configuration [SaveConstants.Request.Configuration.SMTP] = smtp ;


         var imap = new JObject() ;
         imap [SaveConstants.Request.Configuration.Imap.USER] = imapConfiguration.User ;
         imap [SaveConstants.Request.Configuration.Imap.PASSWORD] = imapConfiguration.Password ;
         imap [SaveConstants.Request.Configuration.Imap.SERVER_PORT] = imapConfiguration.ServerPort ;
         imap [SaveConstants.Request.Configuration.Imap.SERVER_ADDRESS] = imapConfiguration.ServerAddress ;
         imap [SaveConstants.Request.Configuration.Imap.USE_SSL] = imapConfiguration.UseSSL ;
         configuration [SaveConstants.Request.Configuration.IMAP] = imap ;
      }
   }
}
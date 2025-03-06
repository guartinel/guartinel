using System.Collections.Generic;
using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Checkers;
using Guartinel.WatcherServer.Communication.ManagementServer;
using Guartinel.WatcherServer.Packages;
using Strings = Guartinel.Communication.Supervisors.EmailSupervisor.Strings;
using SaveRequestConstants = Guartinel.Communication.Supervisors.EmailSupervisor.Strings.WatcherServerRoutes.Save.Request;

namespace Guartinel.WatcherServer.Supervisors.EmailSupervisor {
   public class EmailSupervisorPackage : Package {
      public new static class Constants {
         public const string CAPTION = "Email Supervisor Package" ;
         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> {Strings.Use.PackageType} ;
      }

      protected SmtpConfiguration _userSmtpConfiguration = new SmtpConfiguration() ;
      protected ImapConfiguration _userImapConfiguration = new ImapConfiguration() ;
      protected string _testEmailAddress ; //DTAP this is the email address of the user where we are going to send the test emails
      protected Emailer _emailer ;

      protected override void SpecificConfigure (ConfigurationData configuration) {
         var smtp = configuration.GetChild (SaveRequestConstants.Configuration.SMTP) ;
         _userSmtpConfiguration.Configure (smtp) ;
         var imap = configuration.GetChild (SaveRequestConstants.Configuration.IMAP) ;
         _userImapConfiguration.Configure (imap) ;
         _testEmailAddress = configuration [SaveRequestConstants.Configuration.TEST_EMAIL_ADDRESS] ;

         SetInstances (new List<string> {_testEmailAddress}) ;

         _emailer = new Emailer (_testEmailAddress, _userSmtpConfiguration, _userImapConfiguration) ;
      }

      protected override List<Checker> CreateCheckers1() {
         if (_emailer == null) return new List<Checker>() ;

         var measurementStore = IoC.Use.Single.GetInstance<IMeasuredDataStore>() ;
         var checker = new EmailChecker (measurementStore, _emailer) ;
         checker.Configure (Name, ID, _testEmailAddress) ;

         return new List<Checker> {checker} ;
      }

      private string LogTemplate (string message) => $"<Email Supervisor Package>{ID}: {message}" ;
   }
}
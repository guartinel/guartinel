using System;
using System.Collections.Generic ;
using System.Linq;
using System.Text;
using Guartinel.Kernel ;
using Guartinel.Kernel.Utility;
using Guartinel.WatcherServer.CheckResults;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Supervisors.EmailSupervisor;
using Guartinel.WatcherServer.Tests.Packages;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Guartinel.WatcherServer.Tests.Supervisors.EmailSupervisor {
   [TestFixture]
   public class PackageTests : HttpPackageTestsBase {
      protected string SavePackage (string token,
                                    DateTime modificationDate,
                                    Action<JObject> configure,
                                    string packageName,
                                    string packageID,
                                    bool addMailAlerts,                                    
                                    int checkIntervalSeconds = 30,
                                    int timeoutIntervalSeconds = 30,
                                    int startupDelaySeconds = 1) {

         return base.SavePackageX (token, Guartinel.Communication.Supervisors.EmailSupervisor.Strings.Use.PackageType,
                                  modificationDate,
                                  configure,
                                  packageName, packageID, addMailAlerts,
                                  null,
                                  checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,
                                  false) ;
      }

      private void CreateSetupAddPackageRunCheck (ImapConfiguration imapConfiguration,
                                                  SmtpConfiguration smtpConfiguration,
                                                  string email) {
         StartServer() ;

         var token = Login() ;

         var packageID = Guid.NewGuid().ToString() ;
         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, smtpConfiguration, imapConfiguration, email, 90), "packageName1", packageID, true, 35, 90) ;
         CheckStatus (token, 1) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            EmailSupervisorPackage testPackage = package as EmailSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            // var checker = testPackage.CreateCheckers() [0].CastTo<EmailChecker>() ;
            //Assert.AreEqual ("x1", checker.UserSmtpConfiguration.ServerAddress) ;
            //Assert.AreEqual ("x2", checker.UserSmtpConfiguration.User) ;
            //Assert.AreEqual ("x3", checker.UserSmtpConfiguration.Password) ;
            //Assert.AreEqual (true, checker.UserSmtpConfiguration.UseSSL) ;
            //Assert.AreEqual (1, checker.UserSmtpConfiguration.ServerPort) ;

            //Assert.AreEqual ("y1", checker.UserImapConfiguration.ServerAddress) ;
            //Assert.AreEqual ("y2", checker.UserImapConfiguration.User) ;
            //Assert.AreEqual ("y3", checker.UserImapConfiguration.Password) ;
            //Assert.AreEqual (true, checker.UserImapConfiguration.UseSSL) ;
            //Assert.AreEqual (2, checker.UserImapConfiguration.ServerPort) ;

            // Assert.AreEqual ("x@x.x", checker._testEmailAddress) ;
         }) ;

         // Wait a bit
         new TimeoutSeconds (80).WaitFor (() => (ManagementServer.DeviceAlerts.Count > 0) &&
                                                (ManagementServer.MailAlerts.Count > 0)) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count, ManagementServer.DeviceAlerts.Select (x => x.Message).Concat ("\n")) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count, ManagementServer.MailAlerts.Select (x => x.Message).Concat ("\n")) ;

         // Check states
         Assert.Greater (ManagementServer.MeasuredDataList.Count, 0) ;
      }

      [Test]
      public void CreateSetupAddPackageWithValidAddress_RunCheck1() {
         //ImapConfiguration imapConfiguration = new ImapConfiguration() {
         //         ServerAddress = "mail.sysment.hu",
         //         ServerPort = 993,
         //         UseSSL = true,
         //         User = "guartinel1@sysment.hu",
         //         Password = "wDth8txC6YkZ"
         //} ;

         //SmtpConfiguration smtpConfiguration = new SmtpConfiguration() {
         //         ServerAddress = "mail.sysment.hu",
         //         ServerPort = 587,
         //         UseSSL = true,
         //         User = "guartinel1@sysment.hu",
         //         Password = "wDth8txC6YkZ"
         //} ;

         SmtpConfiguration smtpConfiguration = new SmtpConfiguration() {
                  ServerAddress = "smtp.gmail.com",
                  ServerPort = 465,
                  UseSSL = true,
                  User = "jazminporkolab@gmail.com",
                  Password = "1ArbgMMI0pv4uerMGzhN"
         };
         ImapConfiguration imapConfiguration = new ImapConfiguration() {
                  ServerAddress = "imap.gmail.com",
                  ServerPort = 993,
                  UseSSL = true,
                  User = "jazminporkolab@gmail.com",
                  Password = "1ArbgMMI0pv4uerMGzhN"
         };

         CreateSetupAddPackageRunCheck (imapConfiguration, smtpConfiguration, "jazminporkolab@gmail.com") ;
      }

      [Test]
      public void CreateSetupAddPackageWithValidAddress_RunCheck2() {
         ImapConfiguration imapConfiguration = new ImapConfiguration() {
            //ServerAddress = "mail.sysment.hu",
            //ServerPort = 993,
            //UseSSL = true,
            //User = "guartinel1@sysment.hu",
            //Password = "wDth8txC6YkZ"
            ServerAddress = "smtp.gmail.com",
                  ServerPort = 993,
                  UseSSL = true,
                  User = "jazminporkolab@gmail.com",
                  Password = "1ArbgMMI0pv4uerMGzhN"
         };

         SmtpConfiguration smtpConfiguration = new SmtpConfiguration() {
            //ServerAddress = "mail.sysment.hu",
            //ServerPort = 465,
            //UseSSL = true,
            //User = "guartinel1@sysment.hu",
            //Password = "wDth8txC6YkZ"
            ServerAddress = "imap.gmail.com",
                  ServerPort = 465,
                  UseSSL = true,
                  User = "jazminporkolab@gmail.com",
                  Password = "1ArbgMMI0pv4uerMGzhN"
         };

         // CreateSetupAddPackageRunCheck (imapConfiguration, smtpConfiguration, "guartinel1@sysment.hu") ;
         CreateSetupAddPackageRunCheck (imapConfiguration, smtpConfiguration, "jazminporkolab@gmail.com") ;
      }

      [Test]
      public void TestChecker() {
         //SmtpConfiguration smtpConfiguration = new SmtpConfiguration() {
         //   ServerAddress = "mail.sysment.info",
         //   ServerPort = 465,
         //   UseSSL = true,
         //         User = "test",
         //   Password = "i2NozbIgMr1y7dFqt6Iu"
         //} ;
         //ImapConfiguration imapConfiguration = new ImapConfiguration() {
         //   ServerAddress = "mail.sysment.info",
         //   ServerPort = 993,
         //   UseSSL = true,
         //         User = "test",
         //   Password = "i2NozbIgMr1y7dFqt6Iu"
         //} ;

         ImapConfiguration imapConfiguration = new ImapConfiguration() {
                  ServerAddress = "mail.sysment.hu",
                  ServerPort = 993,
                  UseSSL = true,
                  User = "guartinel1@sysment.hu",
                  Password = "wDth8txC6YkZ"
         } ;

         SmtpConfiguration smtpConfiguration = new SmtpConfiguration() {
                  ServerAddress = "mail.sysment.hu",
                  ServerPort = 587,
                  UseSSL = true,
                  User = "guartinel1@sysment.hu",
                  Password = "wDth8txC6YkZ"
         } ;

         var emailer = new Emailer ("guartinel1@sysment.hu", smtpConfiguration, imapConfiguration) ;
         EmailChecker checker = new EmailChecker (IoC.Use.Single.GetInstance<IMeasuredDataStore>(), emailer) ;
         checker.Configure ("test", "test", "guartinel1@sysment.hu") ;
         var checkResults = checker.Check(null) ;
         Assert.AreEqual (CheckResultKind.Undefined, checkResults[0].CheckResultKind) ;

         IList<CheckResult> checkResults2 ;
         Timeout timeout = new TimeoutSeconds (60) ;
         Assert.IsTrue (timeout.WaitFor (() => {
            checkResults2 = checker.Check (null) ;
            return checkResults2 [0].CheckResultKind == CheckResultKind.Success ;
         }, TimeSpan.FromSeconds (20))) ;
      }
   }
}
using System ;
using System.Net ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Supervisors.EmailSupervisor;
using NUnit.Framework;

namespace Guartinel.WatcherServer.Tests.Supervisors.EmailSupervisor {
   [TestFixture]
   public class EmailerTest : TestsBase {
      [Test]
      public void TestSendAndEmailRetrieval() {
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
         var emailer = new Emailer ("jazminporkolab@gmail.com", smtpConfiguration, imapConfiguration) ;
         emailer.SendOutTestEmails() ;

         // Wait a bit
         Timeout wait = new TimeoutSeconds (61) ;
         var success = wait.WaitFor (() => emailer.CheckAndDeleteTestEmail (TestEmailType.FromGuartinelToUser), TimeSpan.FromSeconds (10)) ;
         Assert.IsTrue (success, "Test email is cannot be retrieved (source: Guartinel).") ;
         wait = new TimeoutSeconds(61);
         success = wait.WaitFor(() => emailer.CheckAndDeleteTestEmail(TestEmailType.FromUserToGuartinel), TimeSpan.FromSeconds(10));
         Assert.IsTrue(success, "Test email is cannot be retrieved (source: user).");

         Assert.IsFalse (emailer.CheckAndDeleteTestEmail (TestEmailType.FromUserToGuartinel), "Test email was not deleted!") ;
      }

      [Test]
      public void CheckSpamFolder () {
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
         var emailer = new Emailer("jazminporkolab@gmail.com", smtpConfiguration, imapConfiguration);
         emailer.SendOutTestEmails();

         Timeout wait = new TimeoutSeconds(61);
         var success = wait.WaitFor(() => emailer.CheckAndDeleteTestEmail(TestEmailType.FromGuartinelToUser), TimeSpan.FromSeconds(10));
         Assert.IsTrue(success, "Test email is cannot be retrieved (source: Guartinel).");
         wait = new TimeoutSeconds(61);
         success = wait.WaitFor(() => emailer.CheckAndDeleteTestEmail(TestEmailType.FromUserToGuartinel), TimeSpan.FromSeconds(10));
         Assert.IsTrue(success, "Test email is cannot be retrieved (source: user).");

         Assert.IsFalse(emailer.CheckAndDeleteTestEmail(TestEmailType.FromUserToGuartinel), "Test email was not deleted!");
      }

      [Test]
      public void CheckSpamFolder25 () {
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

         var emailer = new Emailer("jazminporkolab@gmail.com", smtpConfiguration, imapConfiguration);
         emailer.SendOutTestEmails();

         Timeout wait = new TimeoutSeconds(61);
         var success = wait.WaitFor(() => emailer.CheckAndDeleteTestEmail(TestEmailType.FromGuartinelToUser), TimeSpan.FromSeconds(10));
         Assert.IsTrue(success, "Test email is cannot be retrieved (source: Guartinel).");
         wait = new TimeoutSeconds(61);
         success = wait.WaitFor(() => emailer.CheckAndDeleteTestEmail(TestEmailType.FromUserToGuartinel), TimeSpan.FromSeconds(10));
         Assert.IsTrue(success, "Test email is cannot be retrieved (source: user).");

         Assert.IsFalse(emailer.CheckAndDeleteTestEmail(TestEmailType.FromUserToGuartinel), "Test email was not deleted!");         
      }
   }
}
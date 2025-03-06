
using System;
using System.Collections.Generic;
using System.Linq ;
using System.Text.RegularExpressions ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;

namespace Guartinel.WatcherServer.Supervisors.EmailSupervisor {
   public enum TestEmailType {
      FromGuartinelToUser,
      FromUserToGuartinel
   }

   public static class MailServiceEx {
      public static void Connect (Action<string, int, SecureSocketOptions> connect,
                                  string host,
                                  int port,
                                  bool useSsl) {
         if (connect == null) return ;

         try {
            SecureSocketOptions sslOptions = useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto ;
            connect (host, port, sslOptions) ;
         } catch (System.IO.IOException) {
            connect (host, port, SecureSocketOptions.StartTls) ;
         }
      }
   }

   public interface IEmailer {
      string TestEmailAddress {get ;}

      void SendOutTestEmails() ;

      bool CheckAndDeleteTestEmail (TestEmailType emailType) ;

      // void DeleteTestEmails (TestEmailType emailType) ;

      void DeleteAllUserTestEmails() ;

      bool TestMailsSent {get ;}
      string ImapServerAddress {get ;}
      string SmtpServerAddress {get ;}
   }

   public class Emailer : IEmailer {
      protected TagLogger _logger ;

      private SmtpConfiguration CreateGuartinelSmtpConfiguration() {
         return new SmtpConfiguration() {
                  ServerAddress = ApplicationSettings.Use.EmailCheckerSmtpServerAddress,
                  ServerPort = ApplicationSettings.Use.EmailCheckerSmtpServerPort,
                  UseSSL = ApplicationSettings.Use.EmailCheckerSmtpUseSSL,
                  User = ApplicationSettings.Use.EmailCheckerSmtpUser,
                  Password = ApplicationSettings.Use.EmailCheckerSmtpPassword
         } ;
      }

      private ImapConfiguration CreateGuartinelImapConfiguration() {
         return new ImapConfiguration() {
                  ServerAddress = ApplicationSettings.Use.EmailCheckerImapServerAddress,
                  ServerPort = ApplicationSettings.Use.EmailCheckerImapServerPort,
                  UseSSL = ApplicationSettings.Use.EmailCheckerImapUseSSL,
                  User = ApplicationSettings.Use.EmailCheckerImapUser,
                  Password = ApplicationSettings.Use.EmailCheckerImapPassword
         } ;
      }

      public Emailer (string testEmailAddress,
                      SmtpConfiguration userSmtpConfiguration,
                      ImapConfiguration userImapConfiguration) {
         _logger = new TagLogger (TagLogger.CreateTag (nameof(Emailer), testEmailAddress)) ;
         _testEmailAddress = testEmailAddress ;
         _userSmtpConfiguration = userSmtpConfiguration ;
         _userImapConfiguration = userImapConfiguration ;

         _logger.InfoWithDebug ("Emailer created.", $"SMTP: {userSmtpConfiguration.Data.ConvertToLog()} IMAP: {userImapConfiguration.Data.ConvertToLog()}") ;
      }

      private string _fromUserToGuartinelMailToken ;
      private string _fromGuartinelToUserMailToken ;

      private readonly string _testEmailAddress ;
      public string TestEmailAddress => _testEmailAddress ;
      private readonly SmtpConfiguration _userSmtpConfiguration ;
      private readonly ImapConfiguration _userImapConfiguration ;

      public void SendOutTestEmails() {
         _logger.Info ("Send out test emails.") ;

         if (_fromGuartinelToUserMailToken == null) {
            _fromGuartinelToUserMailToken = Guid.NewGuid().ToString() ;
         }

         // Testing users incoming email by sending email from guartinel to user's IMAP         
         SendEmail (_testEmailAddress,
                    ApplicationSettings.Use.EmailCheckerEmailAddress,
                    _fromGuartinelToUserMailToken,
                    ApplicationSettings.Use.EmailCheckerFingerprintToken,
                    CreateGuartinelSmtpConfiguration()) ;

         if (_fromUserToGuartinelMailToken == null) {
            _fromUserToGuartinelMailToken = Guid.NewGuid().ToString() ;
         }

         // Testing users outgoing email by sending email from user to guartinels IMAP
         SendEmail (ApplicationSettings.Use.EmailCheckerEmailAddress,
                    _testEmailAddress,
                    _fromUserToGuartinelMailToken,
                    ApplicationSettings.Use.EmailCheckerFingerprintToken,
                    _userSmtpConfiguration) ;
      }

      private string GetToken (TestEmailType emailType) {
         string token = string.Empty;
         switch (emailType) {
            case TestEmailType.FromGuartinelToUser:
               token = _fromGuartinelToUserMailToken ;
               break ;
            case TestEmailType.FromUserToGuartinel:
               token = _fromUserToGuartinelMailToken ;
               break ;
         }

         return token;
      }

      private void GenerateNewToken (TestEmailType emailType) {         
         switch (emailType) {
            case TestEmailType.FromGuartinelToUser:
               string token1 = _fromGuartinelToUserMailToken;
               _fromGuartinelToUserMailToken = Guid.NewGuid().ToString();
               _logger.Debug($"Token regenerated, old: '{token1}' to '{_fromGuartinelToUserMailToken}'.");
               break;
            case TestEmailType.FromUserToGuartinel:
               string token2 = _fromUserToGuartinelMailToken;
               _fromUserToGuartinelMailToken = Guid.NewGuid().ToString();
               _logger.Debug($"Token regenerated, old: '{token2}' to '{_fromUserToGuartinelMailToken}'.");
               break;
         }         
      }

      private ImapConfiguration GetImapConfiguration (TestEmailType emailType) {
         ImapConfiguration result = null;
         switch (emailType) {
            case TestEmailType.FromGuartinelToUser:
               result = _userImapConfiguration ;
               break;
            case TestEmailType.FromUserToGuartinel:
               result = CreateGuartinelImapConfiguration() ;
               break;
         }

         return result;
      }

      private void SendEmail (string address,
                              string sender,
                              string emailToken,
                              string fingerPrintToken,
                              SmtpConfiguration configuration) {

         try {
            var message = new MimeMessage() ;
            message.From.Add (new MailboxAddress ("Guartinel Email Tester", sender)) ;
            message.To.Add (new MailboxAddress (address, address)) ;
            message.Subject = "Guartinel Email Supervisor check email" ;

            message.Body = new TextPart ("plain") {
                     Text = $"{emailToken}|{fingerPrintToken}\nPlease do not delete this email to keep the email checking process intact."
            } ;

            using (SmtpClient client = new SmtpClient()) {

               _logger.Info ($"Connecting to Smtp server. Server: {configuration.ServerAddress}.") ;
               MailServiceEx.Connect ((address1,
                                       port1,
                                       options1) => client.Connect (address1, port1, options1),
                                      configuration.ServerAddress, configuration.ServerPort, configuration.UseSSL) ;
               try {
                  _logger.Debug ("Connected to Smtp server.") ;
                  // Note: since we don't have an OAuth2 token, disable
                  // the XOAUTH2 authentication mechanism.
                  client.AuthenticationMechanisms.Remove ("XOAUTH2") ;
                  client.Authenticate (configuration.User, configuration.Password) ;

                  _logger.InfoWithDebug ("Sending test mail to Smtp server.", $"From: {message.From}. To: {message.To}. Body: {message.Body}") ;
                  client.Send (message) ;
                  _logger.Debug ($"Test mail sent to Smtp server. Token: {emailToken}") ;
               } finally {
                  client.Disconnect (true) ;
               }
            }
         } catch (Exception e) {
            _logger.InfoWithDebug ($"Error when sending out test emails. Server: {configuration.ServerAddress}. Error: {e.Message}", $"Details: {e.GetAllMessages()}") ;

            throw new Exception ($"Cannot send test email on server '{configuration.ServerAddress}'. Message: {e.Message}") ;
         }
      }

      private bool FolderContainsMessageWithToken (IMailFolder mailFolder,
                                                   string token,
                                                   FolderAccess? openMode) {
         bool needToClose = false ;

         if (mailFolder == null) return false ;
         mailFolder.Open (openMode ?? FolderAccess.ReadOnly) ;
         try {
            needToClose = openMode == null ;

            var list = mailFolder.Search (SearchQuery.MessageContains (token)) ;
            if (list == null) {
               // If not found, then close
               needToClose = true ;
               return false ;
            }

            var listAll = mailFolder.Search (SearchQuery.All) ;
            _logger.Debug ($"Mails in {mailFolder.FullName}: {mailFolder.Count}") ;
            //foreach (var messageID in listAll) {
            //   _logger.Debug ($"Mail in {mailFolder.FullName}: {mailFolder.GetMessage (messageID).Body}") ;
            //}

            return list.Any() ;
         } finally {
            if (needToClose) {
               mailFolder.Close() ;
            }
         }
      }

      private IMailFolder GetAndOpenMailFolder (ImapClient client,
                                                string token,
                                                FolderAccess openMode) {

         _logger.Debug ($"Checking for token {token}...") ;
         // Try inbox first
         _logger.Debug ($"Checking inbox '{client.Inbox.Name}' for test message...") ;
         if (FolderContainsMessageWithToken (client.Inbox, token, openMode)) return client.Inbox ;
         // Try spam
         string[] spamFolderNames = ApplicationSettings.Use.EmailCheckerSpamFolderNames;
         _logger.Debug ($"Checking spam folders '{spamFolderNames.Concat (", ")}' for test message...") ;

         var spamFolders = client.GetFolders (client.PersonalNamespaces [0]).Where (x => spamFolderNames.Any(y => x.Name.ToLowerInvariant().EndsWith(y.ToLowerInvariant()))) ;
         foreach (IMailFolder spamFolder in spamFolders) {
            _logger.Debug($"Checking spam folder '{spamFolder.FullName}' for test message...");
            if (FolderContainsMessageWithToken (spamFolder, token, openMode)) return spamFolder ;
         }

         return null ;
      }

      private IMailFolder OpenMailFolder (ImapClient client,
                                          string token,
                                          FolderAccess openMode) {

         var result = GetAndOpenMailFolder (client, token, openMode) ;
         if (result == null) return null ;

         return result ;
      }

      public bool CheckAndDeleteTestEmail (TestEmailType emailType) {
         bool isFound = false ;
         var configuration = GetImapConfiguration (emailType) ;
         // var configuration = GetConfiguration(emailType);

         var logger = new TagLogger (_logger.Tags, TagLogger.CreateTag (nameof(configuration.User), configuration.User)) ;

         try {
            var token = GetToken (emailType) ;
            if (string.IsNullOrEmpty (token)) return false ;

            using (ImapClient client = new ImapClient()) {
               logger.Debug ($"Connecting to {configuration.ServerAddress}:{configuration.ServerPort}...");
               MailServiceEx.Connect ((address1,
                                       port1,
                                       secureOptions1) => client.Connect (address1, port1, secureOptions1), configuration.ServerAddress, configuration.ServerPort, configuration.UseSSL) ;
               try {
                  logger.Debug ($"Authenticating as {configuration.User}...") ;
                  client.Authenticate (configuration.User, configuration.Password) ;
                  var mailFolder = OpenMailFolder (client, token, FolderAccess.ReadWrite) ;
                  if (mailFolder == null) return false ;
                  

                  logger.Info ($"Check emails with token: {token}.") ;

                  IList<UniqueId> list = mailFolder.Search (SearchQuery.MessageContains (token)) ;
                  if (list != null && list.Count > 0) {
                     isFound = true ;

                     logger.Info ($"Deleting {list.Count} test emails.") ;

                     mailFolder.AddFlags (list, MessageFlags.Deleted, true) ;
                     mailFolder.Expunge() ;

                     GenerateNewToken (emailType) ;

                  }
               } finally {
                  client.Disconnect (true) ;
               }

               logger.Info ($"Test email existence: {isFound}.") ;
               return isFound ;
            }
         } catch (Exception e) {
            // Ignore errors
            logger.InfoWithDebug ($"Error when checking test email existence. Error: {e.Message}", $"Details: {e.GetAllMessages()}") ;
            return false ;
         }
      }

      private void DeleteTestEmails (string token,
                                     ImapConfiguration configuration) {
         if (string.IsNullOrEmpty (token)) return ;

         try {
            using (ImapClient client = new ImapClient()) {
               MailServiceEx.Connect ((address1,
                                       port1,
                                       options1) => client.Connect (address1, port1, options1), configuration.ServerAddress, configuration.ServerPort, configuration.UseSSL) ;
               try {
                  client.Authenticate (configuration.User, configuration.Password) ;
                  var mailFolder = OpenMailFolder (client, token, FolderAccess.ReadWrite) ;
                  if (mailFolder == null) return ;

                  IList<MailKit.UniqueId> list = mailFolder.Search (SearchQuery.MessageContains (token)) ;
                  if (list != null && list.Count > 0) {
                     _logger.Info ($"Deleting {list.Count} test emails.") ;

                     mailFolder.AddFlags (list, MessageFlags.Deleted, true) ;
                     mailFolder.Expunge() ;
                  }
               } finally {
                  client.Disconnect (true) ;
               }
            }

            _logger.Info ("Deleting test emails succeeded.") ;
         } catch (Exception e) {
            // Ignore errors
            _logger.InfoWithDebug ($"Error when deleting test emails. Message: {e.Message}", $"Details: {e.GetAllMessages()}") ;
         }
      }

      //public void DeleteTestEmails (TestEmailType emailType) {
      //   DeleteTestEmails (GetToken (emailType), GetConfiguration (emailType)) ;
      //}

      public void DeleteAllUserTestEmails() {
         // delete emails in guartinel inbox related to this user
         DeleteTestEmails (ApplicationSettings.Use.EmailCheckerFingerprintToken, _userImapConfiguration) ;
      }

      public bool TestMailsSent => !string.IsNullOrEmpty (_fromUserToGuartinelMailToken) || !string.IsNullOrEmpty (_fromGuartinelToUserMailToken) ;

      public string ImapServerAddress => _userImapConfiguration.ServerAddress ;
      public string SmtpServerAddress => _userSmtpConfiguration.ServerAddress ;
   }
}
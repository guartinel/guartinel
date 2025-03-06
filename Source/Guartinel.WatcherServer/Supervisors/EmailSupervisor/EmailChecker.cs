using System;
using System.Collections.Generic;
using System.Linq ;
using Guartinel.Kernel;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Checkers;
using Guartinel.WatcherServer.CheckResults;
using Guartinel.WatcherServer.Communication.ManagementServer;
using Newtonsoft.Json.Linq;
using MeasurementConstants = Guartinel.Communication.Supervisors.EmailSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement;
using Strings = Guartinel.Communication.Supervisors.EmailSupervisor.Strings;

namespace Guartinel.WatcherServer.Supervisors.EmailSupervisor {
   public class EmailChecker : CheckerWithMeasuredData {
      public EmailChecker (IMeasuredDataStore measuredDataStore,
                           IEmailer emailer) : base (measuredDataStore) {
         _emailer = emailer ;
      }

      public static class Constants {
         public const string CAPTION = "Email Availability Checker" ;
      }

      public static class Defaults { }

      private readonly IEmailer _emailer ;

      #region Configuration
      public new EmailChecker Configure (string name,
                                         string packageID,
                                         string testEmailAddress
               // SmtpConfiguration smtpConfiguration,
               // ImapConfiguration imapConfiguration
      ) {

         base.Configure (name, packageID, testEmailAddress) ;
         // UserImapConfiguration = imapConfiguration ;
         // UserSmtpConfiguration = smtpConfiguration ;

         return this ;
      }
      #endregion Configuration

      protected void StoreMeasurement (CheckResultKind success,
                                       string email,
                                       XString message,
                                       XString details) {
         if (_measuredDataStore == null) return ;

         JObject measurement = new JObject() ;
         measurement.Add (MeasurementConstants.Request.SUCCESS, success.ToString()) ;
         measurement.Add (MeasurementConstants.Request.EMAIL, email) ;
         measurement.Add (MeasurementConstants.Request.MESSAGE, message?.AsJObject()) ;
         measurement.Add (MeasurementConstants.Request.DETAILS, details?.AsJObject()) ;

         //measurement.Add (MeasurementConstants.WEBSITE, url) ;
         //measurement.Add (MeasurementConstants.LOAD_TIME_MILLISECONDS, loadingTimeMilliSeconds == Constants.DO_NOT_CHECK_LOAD_TIME ? (long?) null : loadingTimeMilliSeconds) ;
         _measuredDataStore.StoreMeasuredData (PackageID,
                                               MeasurementConstants.TYPE_VALUE,
                                               DateTime.UtcNow,
                                               measurement) ;
      }

      //To Check user's incoming email 
      //    send email from GuartinelSMTP -> UsersIMAP
      //    check UsersIMAP if our message with GuartinelSMTPToken is arrived
      //To check user's outgoing email
      //    send email from UsersSMTP -> GuartinelIMAP
      //    check GuartinelIMAP if users's message with UserSMTPToken is arrived
      protected CheckResult CheckEmail (string[] tags) {
         var checkResult = CheckResult.CreateUndefined (Name) ;
         var logger = new TagLogger (tags, _emailer.TestEmailAddress) ;

         logger.Debug ($"Test mails sent out? {_emailer.TestMailsSent}") ;

         if (!_emailer.TestMailsSent) {
            logger.Debug ($"This is the first check, delete remaining test emails.") ;
            //this is the first check 
            //try to delete every other remaining test email by the checker fingerprint
            _emailer.DeleteAllUserTestEmails() ;

            try {
               _emailer.SendOutTestEmails() ;
            } catch (Exception e) {
               logger.InfoWithDebug ("Error when sending out test mails.", e.GetAllMessages ()) ;

               checkResult = new CheckResult().Configure (Name, CheckResultKind.Fail) ;

               checkResult.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.EmailSendingErrorMessage),
                                                          new XConstantString.Parameter (Strings.Parameters.IncomingServer, _emailer.ImapServerAddress)) ;
               checkResult.Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.EmailSendingErrorDetails),
                                                                           new XConstantString.Parameter (Strings.Parameters.Error, e.Message)) ;

               checkResult.Extract = new XConstantString(Strings.Use.Get(Strings.Messages.Use.EmailSendingErrorExtract));
            }

            return checkResult ;
         }

         // Let's check if the previous emails are arrived
         bool incomingServerTestMailArrived = _emailer.CheckAndDeleteTestEmail (TestEmailType.FromGuartinelToUser) ;
         var incomingMailExistsString = incomingServerTestMailArrived ? "exists" : "does not exist" ;
         logger.Debug ($"Mail in user's account {incomingMailExistsString}.") ;
         if (!incomingServerTestMailArrived) {
            checkResult.CheckResultKind = CheckResultKind.Fail ;
            checkResult.Message = XStrings.Append (checkResult.Message,
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.IncomingServerErrorMessage),
                                                                        new XConstantString.Parameter (Strings.Parameters.IncomingServer, _emailer.ImapServerAddress)
                                                                       ), true) ;
            checkResult.Details = XStrings.Append (checkResult.Details,
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.IncomingServerErrorDetails),
                                                                        new XConstantString.Parameter (Strings.Parameters.Error, "Cannot find test email in inbox")
                                                                       ), true) ;

            checkResult.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.TestEmailNotArrivedErrorExtract)) ;
         }

         bool outgoingServerTestMailArrived = _emailer.CheckAndDeleteTestEmail (TestEmailType.FromUserToGuartinel) ;
         var outgoingMailExistsString = outgoingServerTestMailArrived ? "exists" : "does not exist" ;
         logger.Debug ($"Mail in Guartinel account {outgoingMailExistsString}.") ;
         if (!outgoingServerTestMailArrived) {
            checkResult.CheckResultKind = CheckResultKind.Fail ;
            checkResult.Message = XStrings.Append (checkResult.Message,
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.OutgoingServerErrorMessage),
                                                                        new XConstantString.Parameter (Strings.Parameters.OutgoingServer, _emailer.SmtpServerAddress)
                                                                       ), true) ;
            checkResult.Details = XStrings.Append (checkResult.Details,
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.OutgoingServerErrorDetails),
                                                                        new XConstantString.Parameter (Strings.Parameters.Error, "Cannot find test email in inbox")
                                                                       ), true) ;
            checkResult.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.TestEmailNotArrivedErrorExtract)) ;
         }

         //Previous check was successfull so send the test emails for the next check iteration
         _emailer.SendOutTestEmails() ;

         if (checkResult.CheckResultKind == CheckResultKind.Fail) {
            logger.Debug ("Check returns failure.") ;
            StoreMeasurement (CheckResultKind.Fail, _emailer.TestEmailAddress, checkResult.Message, checkResult.Details) ;
            // no more check until the servers are not working to prevent filling the mailbox of the test user
            return checkResult ;
         }

         StoreMeasurement (CheckResultKind.Success, _emailer.TestEmailAddress, null, null) ;

         logger.Debug ($"Check returns success.") ;
         checkResult.Configure (Name, CheckResultKind.Success,
                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.EverythingIsOKMessage)),
                                XStrings.Append (new XConstantString (Strings.Use.Get (Strings.Messages.Use.IncomingServerOKDetails),
                                                                      new XConstantString.Parameter (Strings.Parameters.IncomingServer,
                                                                                                     _emailer.ImapServerAddress)
                                                                     ),
                                                 new XConstantString (Strings.Use.Get (Strings.Messages.Use.OutgoingServerOKDetails),
                                                                      new XConstantString.Parameter (Strings.Parameters.OutgoingServer,
                                                                                                     _emailer.SmtpServerAddress)
                                                                     ), true),
                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.EverythingIsOKExtract))
                               ) ;
         return checkResult ;
      }

      protected override IList<CheckResult> Check1 (string[] tags) {
         var result = CheckEmail (tags) ;
         return new List<CheckResult> {result} ;
      }
   }
}
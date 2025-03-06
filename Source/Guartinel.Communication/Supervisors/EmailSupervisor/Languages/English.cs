
namespace Guartinel.Communication.Supervisors.EmailSupervisor.Languages {
   public class English : Communication.Languages.EnglishBase, Strings.IMessages {
      public string OutgoingServerErrorMessage => $"Error when trying to send email on '{Parameter(Strings.Parameters.OutgoingServer)}'." ;
      public string OutgoingServerErrorDetails => $"Outgoing server error: {Parameter(Strings.Parameters.Error)}" ;
      public string IncomingServerErrorMessage => $"Error when trying to receive email on '{Parameter(Strings.Parameters.IncomingServer)}'.";
      public string IncomingServerErrorDetails => $"Incoming server error: {Parameter(Strings.Parameters.Error)}";
      public string TestEmailNotArrivedErrorExtract => "Test email did not arrive." ;
      public string EverythingIsOKMessage => "Incoming and outgoing mails are delivered successfully." ;
      public string EverythingIsOKExtract => "Everything works fine!" ;
      public string OutgoingServerOKDetails => $"Outgoing server '{Parameter(Strings.Parameters.OutgoingServer)}' works fine!" ;
      public string IncomingServerOKDetails => $"Incoming server '{Parameter(Strings.Parameters.IncomingServer)}' works fine!" ;

      public string EmailSendingErrorMessage => "Error sending out test emails." ;
      public string EmailSendingErrorDetails => $"{Parameter (Strings.Parameters.Error)}" ;
      public string EmailSendingErrorExtract => $"Cannot send test emails." ;
   }
}
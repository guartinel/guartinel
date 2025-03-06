using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guartinel.Communication.Strings;
using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Communication.Supervisors.EmailSupervisor {
   public class Strings : StringsBase {
      public override string Prefix => PackageType;
      public string PackageType => "EMAIL_SUPERVISOR";

      private Strings () {
         _languages.Add(new Languages.English());
      }

      public static Strings Use { get; } = new Strings();

      public interface IMessages {
         string OutgoingServerErrorMessage { get; }
         string OutgoingServerErrorDetails { get; }
         string IncomingServerErrorMessage { get; }
         string IncomingServerErrorDetails { get; }
         string TestEmailNotArrivedErrorExtract { get; }         
         string EverythingIsOKMessage { get; }
         string EverythingIsOKExtract { get; }
         string OutgoingServerOKDetails { get; }
         string IncomingServerOKDetails { get; }
         string EmailSendingErrorMessage { get; }
         string EmailSendingErrorDetails { get; }
         string EmailSendingErrorExtract { get; }         
      }

      public class Messages : IMessages {
         public static Messages Use {get ;} = new Messages() ;
         public string OutgoingServerErrorMessage => nameof(OutgoingServerErrorMessage) ;
         public string OutgoingServerErrorDetails => nameof(OutgoingServerErrorDetails) ;
         public string IncomingServerErrorMessage => nameof(IncomingServerErrorMessage) ;
         public string IncomingServerErrorDetails => nameof(IncomingServerErrorDetails) ;
         public string TestEmailNotArrivedErrorExtract => nameof(TestEmailNotArrivedErrorExtract) ;
         public string EverythingIsOKMessage => nameof(EverythingIsOKMessage) ;
         public string EverythingIsOKExtract => nameof(EverythingIsOKExtract) ;
         public string OutgoingServerOKDetails => nameof(OutgoingServerOKDetails) ;
         public string IncomingServerOKDetails => nameof(IncomingServerOKDetails) ;
         public string EmailSendingErrorMessage => nameof(EmailSendingErrorMessage) ;
         public string EmailSendingErrorDetails => nameof(EmailSendingErrorDetails) ;
         public string EmailSendingErrorExtract => nameof(EmailSendingErrorExtract) ;
      }

      public static class Parameters {
         public static string Address => nameof(Address);
         public static string Error => nameof(Error);
         public static string IncomingServer => nameof(IncomingServer);
         public static string OutgoingServer => nameof(OutgoingServer);

      }

      public override Dictionary<string, string> GetProperties () {
         return Helper.ObjectToDictionary(new Properties());
      }

      public class Properties {
         [JsonProperty]
         public const string IMAP = "imap";
         [JsonProperty]
         public const string SMTP = "smtp";
         [JsonProperty]
         public const string SERVER_ADDRESS = "server_address";
         [JsonProperty]
         public const string SERVER_PORT = "server_port";
         [JsonProperty]
         public const string USER = "user";
         [JsonProperty]
         public const string PASSWORD = "password";
         [JsonProperty]
         public const string USE_SSL = "use_ssl";

         [JsonProperty]
         public const string TEST_EMAIL_ADDRESS = "test_email_address";
      }

      public static class ManagementServerRoutes {
         private const string URL_BASE = "/emailSupervisor/";

         public static class RegisterMeasurement {
            private const string URL_PART = "registerMeasurement";
            public const string FULL_URL = URL_BASE + URL_PART;

            public const string TYPE_VALUE = "EmailChecker";

            public static class Request {
               public const string TOKEN = Communication.Strings.Strings.AllParameters.TOKEN;
               public const string MEASUREMENT = Communication.Strings.Strings.AllParameters.MEASUREMENT;

               public const string SUCCESS = AllParameters.SUCCESS;
               public const string EMAIL = AllParameters.EMAIL ;
               public const string MESSAGE = AllParameters.MESSAGE;
               public const string DETAILS = AllParameters.DETAILS;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = Communication.Strings.Strings.AllErrorValues.INTERNAL_SYSTEM_ERROR;
                  public const string ACCOUNT_EXPIRED = Communication.Strings.Strings.AllErrorValues.ACCOUNT_EXPIRED;
                  public const string INVALID_TOKEN = Communication.Strings.Strings.AllErrorValues.INVALID_TOKEN;
                  public const string NOT_USED_IN_PACKAGE = Communication.Strings.Strings.AllErrorValues.NOT_USED_IN_PACKAGE;
               }
            }
         }
      }

      public static class WatcherServerRoutes {
         public static class Save {
            public static class Request {
               public static class Configuration {
                  public const string TEST_EMAIL_ADDRESS = Properties.TEST_EMAIL_ADDRESS;

                  public const string IMAP = Properties.IMAP;
                  public static class Imap {
                     public const string SERVER_ADDRESS = Properties.SERVER_ADDRESS;
                     public const string SERVER_PORT = Properties.SERVER_PORT;
                     public const string USE_SSL = Properties.USE_SSL;
                     public const string USER = Properties.USER;
                     public const string PASSWORD = Properties.PASSWORD;
                  }

                  public const string SMTP = Properties.SMTP;

                  public static class Smtp {
                     public const string SERVER_ADDRESS = Properties.SERVER_ADDRESS;
                     public const string SERVER_PORT = Properties.SERVER_PORT;
                     public const string USE_SSL = Properties.USE_SSL;
                     public const string USER = Properties.USER;
                     public const string PASSWORD = Properties.PASSWORD;

                  }
               }
            }
         }
      }
   }
}

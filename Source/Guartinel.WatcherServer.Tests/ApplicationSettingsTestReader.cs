using System;
using System.Linq;
using System.Text;
using Guartinel.Kernel ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.WatcherServer.Tests {
   public class ApplicationSettingsTestReader : IApplicationSettingsReader {
      public static JObject Data ;

      static ApplicationSettingsTestReader() {
         const string SETTINGS = @"
      {
         ""QueueServiceAddress"":""http://10.0.75.1:5672/"",
         ""QueueServiceUserNameX"":""queue.test"",
         ""QueueServiceUserName"":""guest"",
         ""QueueServicePasswordX"":""VLTWnQ3eW6T1MOJsmMwI"",
         ""QueueServicePassword"":""guest"",

         ""ServerAddress"":""https://backend2.guartinel.com:5562/"",
         ""ServerID"":""0hIeNc5rAto-NQ"",
         
         ""RegistrationUserName"":""guartadmin"",
         ""RegistrationPasswordHash"":""27DF8B45F270A2A1F3A5ECB9555D75F7AE370CA16957759C086A82CB3A8AB7F6031373C0268F9D65345A035E5005B8FF94236D5D381D72F956CDC0FCB8045005"",
         ""ManagementServerID"":""b18f2f36-ab70-4a99-9e27-19cd7fd704eb"",
         ""ManagementServerUID"":""BFEBFBFF000106E5"",
         ""ManagementServerAddress"":""https://backend2.guartinel.com:9091"",
         ""ManagementServerRegistrationToken"":""5p9v6Lu6fbwkMw"",
         
         ""LogLevel"":""Debug"",
         ""LogFolder"":""C:\\Temp\\GuartinelLogs\\WatcherServer"",
         ""Categories"":""free;test;trial;business;home;premium"",
         
         ""Decimals"":""2"",
         ""TokenExpirySeconds"":""10800"",   
         
         ""WebsiteChecker"":""ChromeRemote"",
         ""WebsiteCheckerPoolSize"":30,
         
         ""EmailCheckerEmailAddress"":""test@sysment.net"",
         ""EmailCheckerFingerprintToken"":""774ed86f-4af4-4f91-b1a7-bdde48c9314c"",
         
         ""EmailCheckerSmtpServerAddress"":""mail.sysment.net"",
         ""EmailCheckerSmtpServerPort"":465,
         ""EmailCheckerSmtpUseSSL"":true,
         ""EmailCheckerSmtpUser"":""test@sysment.net"",
         ""EmailCheckerSmtpPassword"":""sX6lA6y5w7M12DYZnT6g"",
         
         ""EmailCheckerImapServerAddress"":""mail.sysment.net"",
         ""EmailCheckerImapServerPort"":993,
         ""EmailCheckerImapUseSSL"":true,
         ""EmailCheckerImapUser"":""test@sysment.net"",
         ""EmailCheckerImapPassword"":""sX6lA6y5w7M12DYZnT6g"",

         ""EmailCheckerSpamFolderNames"":[""spam"",""junk""]
      }";
         Data = JObject.Parse (SETTINGS) ;

      }

      public JObject ReadConfigurationObject() {
         //result [nameof(QueueServiceAddress)] = "" ;
         //result[nameof(ServerAddress)] = "";
         //result[nameof(ServerID)] = "";

         //result[nameof(ManagementServerAddress)] = "";
         //result[nameof(ManagementServerID)] = "";
         //result[nameof(ManagementServerUID)] = "";
         //result[nameof(ManagementServerRegistrationToken)] = "";
         //result[nameof(RegistrationUserName)] = "";
         //result[nameof(RegistrationPasswordHash)] = "";
         //result[nameof(TokenExpirySeconds)] = "";
         //result[nameof(WatcherRunnerCount)] = "";
         //result[nameof(CheckersPerPackage)] = "";
         //result[nameof(WebsiteChecker)] = "";
         //result[nameof(WebsiteCheckerPoolSize)] = "";
         //result[nameof(Categories)] = "";
         //result[nameof(Decimals)] = "";
         //result[nameof(EmailCheckerEmailAddress)] = "";
         //result[nameof(EmailCheckerFingerprintToken)] = "";
         //result[nameof(EmailCheckerSmtpServerAddress)] = "";
         //result[nameof(EmailCheckerSmtpServerPort)] = "";
         //result[nameof(EmailCheckerSmtpUseSSL)] = "";
         //result[nameof(EmailCheckerSmtpUser)] = "";
         //result[nameof(EmailCheckerSmtpPassword)] = "";
         //result[nameof(EmailCheckerImapServerAddress)] = "";
         //result[nameof(EmailCheckerImapServerPort)] = "";
         //result[nameof(EmailCheckerImapUseSSL)] = "";
         //result[nameof(EmailCheckerImapUser)] = "";
         //result[nameof(EmailCheckerImapPassword)] = "";

         return Data ;
      }

      public JObject GetConfigurationInfo() {
         return new JObject() ;
      }

      public void SubscribeForChange (int refreshIntervalSeconds,
                                      Action notification) {
         // Nothing to do, no change
      }
   }
}  
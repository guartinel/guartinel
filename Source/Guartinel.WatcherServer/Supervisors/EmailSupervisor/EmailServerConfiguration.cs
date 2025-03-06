using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.WatcherServer.Supervisors.EmailSupervisor {
   using ConfigurationConstants = Guartinel.Communication.Supervisors.EmailSupervisor.Strings.WatcherServerRoutes.Save.Request.Configuration;
   public class EmailServerConfiguration { 
      protected JObject _data = new JObject();
      public JObject Data => _data ;

      public string ServerAddress {
         get => _data.GetStringValue (nameof(ServerAddress)) ;
         set => _data [nameof(ServerAddress)] = value ;
      }
      public int ServerPort {
         get => _data.GetIntegerValue(nameof(ServerPort), 0) ;
         set => _data[nameof(ServerPort)] = value;
      }

      public bool UseSSL {
         get => _data.GetBooleanValue (nameof(UseSSL), false) ;
         set => _data[nameof(UseSSL)] = value;
      }

      public string User {
         get => _data.GetStringValue(nameof(User));
         set => _data[nameof(User)] = value;
      }

      public string Password {
         get => _data.GetStringValue(nameof(Password));
         set => _data[nameof(Password)] = value;
      }

      public void Configure(ConfigurationData configuration) {
         ServerAddress = configuration [ConfigurationConstants.Imap.SERVER_ADDRESS] ;
         ServerPort = configuration.AsInteger(ConfigurationConstants.Imap.SERVER_PORT);
         User = configuration[ConfigurationConstants.Imap.USER];
         Password = configuration[ConfigurationConstants.Imap.PASSWORD];
         UseSSL = configuration.AsBoolean(ConfigurationConstants.Imap.USE_SSL);
         
         Configure1(configuration) ;
      }

      protected virtual void Configure1 (ConfigurationData configuration) {         
      }

      public string AddressPlusPort => $"{ServerAddress}:{ServerPort}" ;
   }
   public class ImapConfiguration : EmailServerConfiguration { }

   public class SmtpConfiguration : EmailServerConfiguration { }
}



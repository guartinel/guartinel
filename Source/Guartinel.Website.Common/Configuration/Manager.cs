using System;
using System.IO;
using System.Text;
using Guartinel.Website.Common.Tools;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Website.Common.Configuration {
   public static class Manager {
      public static ISettings Load (Type settingsType,
                                    string configPath) {
         ISettings settings = null ;
         try {
            using (StreamReader streamReader = new StreamReader (configPath)) {
               string json = streamReader.ReadToEnd() ;
               MemoryStream ms = new MemoryStream (Encoding.Unicode.GetBytes (json)) ;
               System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer (settingsType) ;
               settings = (ISettings) serializer.ReadObject (ms) ;
               ms.Close() ;
            }
         } catch (Exception e) {
            Logger.Error ($"Cannot load settings Message: {e.GetAllMessages()}") ;
         }

         return settings ;
      }

      public static void Save (ISettings settings,
                               string configPath) {
         try {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer (settings.GetType()) ;
            MemoryStream ms = new MemoryStream() ;
            serializer.WriteObject (ms, settings) ;
            string json = Encoding.Default.GetString (ms.ToArray()) ;
            File.WriteAllText (configPath, json) ;
         } catch (Exception e) {
            Logger.Error ($"Cannot save settings : {e.GetAllMessages()}") ;
         }
      }
   }
}
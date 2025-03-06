using System ;
using System.Linq ;
using System.Net ;
using System.Net.Sockets ;
using System.Text ;

namespace Guartinel.WatcherServer.Communication {
   public static class Urls {

      public static class ManagementServer {

         public static string GetDebugIP() {
            string localIP = "" ;
            IPHostEntry host = Dns.GetHostEntry (Dns.GetHostName()) ;
            foreach (IPAddress ip in host.AddressList) {
               if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().Contains ("192.168.1.")
                   || ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().Contains ("10.140.")) {
                  localIP = ip.ToString() ;
                  break ;
               }
            }
            return "http://" + localIP + ":8080/" ;
         }

         public class Account {
            public const string LOGIN = "account/login/" ;
            public const string CREATE = "account/create/" ;
            public const string LOGOUT = "account/logout/" ;
            public const string INFO = "account/info/" ;
            public const string UPDATE = "account/update/" ;
         }

         public static class Device {
            public const string LOGIN = "device/login/" ;
            public const string REGISTER = "device/register/" ;
            public const string AGENT_CHECKRESULT = "device/agent/checkresult/" ;
            public const string ANDROID_SYNC = "device/android/sync/" ;
            public const string AVAILABLE = "device/available" ;
         }

         public static class Package {
            public const string ITSTATUS_CREATE = "package/itstatus/create" ;
            public const string ITSTATUS_UPDATE = "package/itstatus/update" ;
            public const string AVAILABLE = "package/available" ;
            public const string DELETE_PACKAGE = "package/delete/" ;
         }
      }

      public static class WatcherServer {
         public const string CHECK_RESULT = "checkresult/" ;
         public const string CREATE_ITSTATUS_PACKAGE = "package/itstatus/create/" ;
         public const string UPDATE_ITSTATUS_PACKAGE = "package/itstatus/update/" ;
         public const string DELETE_PACKAGE = "package/delete/" ;

         public static string GetDebugIP() {
            // todo: SZTZ: Check with DTAP

            string localIP = "" ;
            IPHostEntry host = Dns.GetHostEntry (Dns.GetHostName()) ;
            foreach (IPAddress ip in host.AddressList) {
               if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().Contains ("192.168.91.") ||
                   ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().Contains ("10.140.")) {
                  
                  localIP = ip.ToString() ;
                  break ;
               }
            }
            return "http://" + localIP + ":8081/" ;
         }
      }
   }
}

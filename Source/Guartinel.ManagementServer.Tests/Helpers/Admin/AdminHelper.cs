using System ;
using System.Text ;
using Guartinel.ManagementServer.Tests.Connection ;

namespace Guartinel.ManagementServer.Tests.Helpers.Admin {
   internal class AdminHelper {
      public static Response login (string userName,
            string password) {
         return Connector.MakeRequest ("/admin/login", new {
            user_name = userName,
            password = password,
         }) ;
      }

      public static Response registerWatcherServer (string token,
            string name,
            string address,
            string port,
            string userName,
            string password) {
         return Connector.MakeRequest ("/admin/watcherserver/register", new {
            token = token,
            name = name,
            address = address,
            port = port,
            user_name = userName,
            password = password,
            categories = new string[1] {"test"},
            new_password = password,
            new_user_name = userName
         }) ;
      }

      public static Response getWatcherServers (string token) {
         return Connector.MakeRequest ("/admin/watcherserver/getavailable", new {
            token = token
         }) ;
      }

      public static Response getStatusFromWatcherServer (string token,
            string watcherServerId) {
         return Connector.MakeRequest ("/admin/watcherserver/getStatus", new {
            token = token,
            watcher_server_id = watcherServerId
         }) ;
      }

      public static Response removeWatcherServer (string token,
            string watcherServerId) {
         return Connector.MakeRequest ("/admin/watcherserver/remove", new {
            token = token,
            watcher_server_id = watcherServerId
         }) ;
      }

      public static Response getEventsFromWatcherServer (string token,
            string watcherServerId) {
         return Connector.MakeRequest ("/admin/watcherserver/getevents", new {
            token = token,
            watcher_server_id = watcherServerId
         }) ;
      }
   }
}

using System ;
using System.Text ;
using Guartinel.ManagementServer.Tests.Connection ;

namespace Guartinel.ManagementServer.Tests.Helpers.Device {
   internal class DeviceHelper {
      public static Response registerDevice (string email,
            string password,
            string deviceType,
            string deviceName,
            string gcmId = null) {
         return Connector.MakeRequest ("/device/register", new {
            email = email,
            password = password,
            device_name = deviceName,
            device_type = deviceType,
            gcm_id = gcmId,
         }) ;
      }

      public static Response loginDevice (string email,
            string password,
            string deviceUUID,
            string gcmId = null) {
         return Connector.MakeRequest ("/device/login", new {
            email = email,
            password = password,
            device_uuid = deviceUUID,
            gcm_id = gcmId,
         }) ;
      }

      public static Response deleteDevice (string token,
            string deviceUUID) {
         return Connector.MakeRequest ("/device/delete", new {
            token = token,
            device_uuid = deviceUUID
         }) ;
      }

      public static Response getAvailable (string token) {
         return Connector.MakeRequest ("/device/getAvailable", new {
            token = token
         }) ;
      }
   }
}

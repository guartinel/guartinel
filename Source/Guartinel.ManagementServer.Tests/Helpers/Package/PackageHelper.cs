using System ;
using System.Text ;
using Guartinel.ManagementServer.Tests.Connection ;

namespace Guartinel.ManagementServer.Tests.Helpers.Package {
   internal class PackageHelper {
      public static Response savePackage (string token,
            string packageType,
            string packageName,
            string configData,
            string packageId = null) {
         return Connector.MakeRequest ("/package/save", new {
            token = token,
            package_id = packageId,
            package_type = packageType,
            package_name = packageName,
            configuration = configData,
            is_enabled = true
         }) ;
      }

      public static Response deletePackage (string token,
            string packageId) {
         return Connector.MakeRequest ("/package/delete", new {
            token = token,
            package_id = packageId
         }) ;
      }

      public static Response getAvailable (string token) {
         return Connector.MakeRequest ("/package/getavailable", new {
            token = token
         }) ;
      }
   }
}

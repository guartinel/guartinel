using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Configuration ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.ManagementServer.APITests.Tests {
   [TestFixture]
   public class ErrorTests : TestsBase {
      [Test]
      public void AddNewPackageWithWrongData_CheckIfError() {
         var token = GetToken() ;

         var packageName = Guid.NewGuid().ToString() ;

         JObject parameters = new JObject() ;
         parameters [Constants.TOKEN_PARAMETER] = token ;

         JObject package = new JObject() ;

         package ["package_name"] = packageName ;
         package ["package_type"] = "WEBSITE_SUPERVISOR" ;
         package ["check_interval_seconds"] = 100 ;
         parameters ["package"] = package + "wroooooooong" ;

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("api/package/save", parameters)) ;
         CheckError (resultParameters) ;
      }

      [Test]
      public void AddInvalidPackageType_CheckIfError() {
         var token = GetToken() ;

         var packageName = Guid.NewGuid().ToString() ;

         JObject parameters = new JObject() ;
         parameters [Constants.TOKEN_PARAMETER] = token ;

         JObject package = new JObject() ;

         package ["package_name"] = packageName ;
         package ["package_type"] = "Wrong" ;
         package ["check_interval_seconds"] = 100 ;
         parameters ["package"] = package ;

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("api/package/save", parameters)) ;
         CheckError (resultParameters) ;
      }
   }
}
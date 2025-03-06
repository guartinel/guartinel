using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Communication.Routes ;
using Guartinel.WatcherServer.Packages ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Routes {
   [TestFixture]
   public class TestRoutes : TestsBase {
      protected PackageController _packageController ;

      protected readonly string _managementServerUID = Guid.NewGuid().ToString() ;
      
      protected new void Setup() {
         _packageController = new PackageController() ;
      }

      [Test]
      public void RegisterServer_Check() {
         const string PARAMETERS = @"{{'user_name':'{0}', 'password':'{1}', 'management_server_address':'managementserver1'}}" ;

         Setup() ;
         
         RegisterServerRoute route = new RegisterServerRoute() ;
         Parameters parameters = new Parameters (String.Format (PARAMETERS, Constants.LOGIN_USER_NAME, Constants.LOGIN_PASSWORD_HASH)) ;
         Parameters results = new Parameters();         
         
         route.ProcessRequest (parameters, results, null) ;

         Assert.AreEqual (WatcherServerAPI.GeneralResponse.SuccessValues.SUCCESS, results [WatcherServerAPI.GeneralResponse.Names.SUCCESS]) ;
         Assert.IsFalse (string.IsNullOrEmpty (results ["token"])) ;
         
         Assert.AreEqual ("managementserver1", ApplicationSettings.Use.ManagementServerAddress) ;
      }

      [Test]
      public void Login_Check() {
         Setup() ;

         Login() ;
      }

      private string Login() {
         const string PARAMETERS = @"{{'password':'{0}'}}" ;
         ApplicationSettings.Use.ManagementServerID = Guid.NewGuid().ToString() ;
         ApplicationSettings.Use.ManagementServerAddress = Guid.NewGuid().ToString() ;
       //DTAP  Configuration.LoginPasswordHash = Hashing.GenerateHash (Hashing.GenerateHash (_managementServerUID, Configuration.ManagementServerID), Configuration.ManagementServerID) ;
         
         Parameters parameters = new Parameters (String.Format (PARAMETERS,
                                                                Hashing.GenerateHash (_managementServerUID, ApplicationSettings.Use.ManagementServerID))) ;
         LoginRoute route = new LoginRoute() ;
         Parameters results = new Parameters() ;

         route.ProcessRequest (parameters, results, null) ;

         Assert.AreEqual (WatcherServerAPI.GeneralResponse.SuccessValues.SUCCESS, results [WatcherServerAPI.GeneralResponse.Names.SUCCESS]) ;
         Assert.IsFalse (string.IsNullOrEmpty (results ["token"])) ;

         return results ["token"] ;
      }
   }
}

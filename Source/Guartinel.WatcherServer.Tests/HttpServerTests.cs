using System ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using Guartinel.Communication ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Tests ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Communication ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.WatcherServer.Tests {
   [TestFixture]
   public class HttpServerTests : HttpServerTestsBase {
      [Test]
      public void StartHttpServer_InvalidRequest() {
         StartServer() ;

         JObject parameters = new JObject() ;
         parameters ["bikkmakk"] = "ehune!" ;

         // AssertEx.ShouldThrowContaining (() => new Parameters (SendPostToServer ("wrong_route", parameters)), "path", "not found", "wrong_route") ;
         AssertEx.ShouldThrowContaining (() => new Parameters (SendPostToServer ("wrong_route", parameters)), "not found") ;
      }

      [Test]
      public void StartHttpServer_Login_CheckResult() {
         StartServer() ;
         Login() ;
      }

      [Test]
      public void StartHttpServer_RegisterServer_CheckResult() {
         StartServer() ;

         const string TEST_USER_NAME = "test1" ;
         const string TEST_PASSWORD = "t%uybdfest11" ;
         const string TEST_SERVER_ADDRESS = @"123.345.567.789" ;
         const string TEST_SERVER_UID = "E1E71505-FBA1-480E-8DCB-64805849A600" ;

         JObject parameters = new JObject() ;
         parameters ["token"] = null ;
         parameters ["user_name"] = Constants.LOGIN_USER_NAME ;
         parameters ["password"] = Constants.LOGIN_PASSWORD_HASH ;
         parameters ["new_user_name"] = TEST_USER_NAME ;
         parameters ["new_password"] = TEST_PASSWORD ;
         parameters ["management_server_address"] = TEST_SERVER_ADDRESS ;
         parameters ["uid"] = TEST_SERVER_UID ;

         Parameters result = new Parameters (SendPostToServer ("admin/register", parameters)) ;

         Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS], result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
         Assert.IsFalse (string.IsNullOrEmpty (result ["token"])) ;
         string serverID = result ["management_server_id"] ;
         ApplicationSettings.Use.ManagementServerID = serverID ;
         ApplicationSettings.Use.ManagementServerAddress = TEST_SERVER_ADDRESS ;
         Assert.IsFalse (string.IsNullOrEmpty (serverID)) ;

         // Check login with the new username and password
         parameters = new JObject() ;
         parameters ["password"] = Hashing.GenerateHash (TEST_SERVER_UID, serverID) ;
         result = new Parameters (SendPostToServer ("admin/login", parameters)) ;

         Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS], result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
         Assert.IsFalse (string.IsNullOrEmpty (result ["token"])) ;
      }

      [Test]
      public void StartHttpServer_Login_CheckEvents() {
         StartServer() ;
         var token = Login() ;

         JObject parameters = new JObject() ;
         parameters ["token"] = token ;

         Parameters result = new Parameters (SendPostToServer ("admin/getEvents", parameters)) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS], result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;

         var events = result.GetChildren ("events") ;
         Assert.AreEqual (2, events.Count) ;
      }

      [Test]
      public void StartHttpServer_Login_GetStatus_CheckResult() {
         StartServer() ;

         var token = Login() ;

         CheckStatus (token, 0) ;
      }

      [Test]
      public void StartHttpServer_Login_GetStatusWithInvalidToken() {
         StartServer() ;

         var token = Login() ;

         // Use invalid token
         JObject parameters = new JObject() ;
         parameters ["token"] = token + "_invalid" ;

         Parameters result = new Parameters (SendPostToServer ("admin/getStatus", parameters)) ;

         Assert.IsTrue (result.IsError()) ;
         Assert.AreEqual (AllErrorValues.INVALID_TOKEN, result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
         Assert.IsFalse (string.IsNullOrEmpty (result [WatcherServerAPI.GeneralResponse.Names.ERROR_UUID])) ;
      }

      [Test]
      public void StartHttpServer_Login_GetStatusWithExpiredToken() {
         StartServer() ;

         // Check configuration
         ApplicationSettings.Use.TokenExpirySeconds = 1 ;

         var token = Login() ;

         Thread.Sleep (2000) ;

         // Use invalid token
         JObject parameters = new JObject() ;
         parameters ["token"] = token ;

         Parameters result = new Parameters (SendPostToServer ("admin/getStatus", parameters)) ;

         Assert.IsTrue (result.IsError()) ;
         Assert.AreEqual (AllErrorValues.TOKEN_EXPIRED, result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
         Assert.IsFalse (string.IsNullOrEmpty (result [WatcherServerAPI.GeneralResponse.Names.ERROR_UUID])) ;
      }

      [Test]
      public void StartHttpServer_GetVersion() {
         StartServer() ;

         // Get version
         var result = new Parameters (SendPostToServer ("admin/getVersion", new JObject())) ;

         Assert.IsTrue (result.IsSuccess()) ;
         Logger.Log(result.ToString()) ;         
         Assert.IsFalse (string.IsNullOrEmpty (result ["version"])) ;
         var version = result.GetChild ("version").AsJObject ;
         var watcherVersion = version ["watcher_server"] ;
         Assert.IsNotNull (watcherVersion) ;
         Assert.IsFalse (watcherVersion.Contains (".")) ;
      }
   }
}
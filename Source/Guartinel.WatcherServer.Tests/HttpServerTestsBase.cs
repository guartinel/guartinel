using System;
using System.Diagnostics ;
using System.Linq;
using System.Text;
using System.Threading ;
using Guartinel.Communication ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Communication ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;

namespace Guartinel.WatcherServer.Tests {
   public class HttpServerTestsBase : TestsBase {
      public HttpServerTestsBase() {
      }

      protected readonly PostSender _postSender = new PostSender() ;
      protected ExecuteBehavior.InTaskQueue _postBehavior ;

      protected override void Setup1() {
         base.Setup1() ;

         _postBehavior = new ExecuteBehavior.InTaskQueue("Test server posts") ;

         // Factory.Use.RegisterCreator (new Creator (typeof (TestPackage), () => new TestPackage(), typeof (Package), "TestPackage")) ;
      }

      protected override void TearDown1() {
         _postBehavior.Cancel();
         _watcherServer.Stop();
         Thread.Sleep (300) ;

         // Factory.Use.UnregisterCreators<TestPackage>() ;

         base.TearDown1() ;
      }

      // private const string IP_ADDRESS = "127.0.0.1" ;
      protected readonly string _serverAddress = $"http://{Utility.GetLocalFixIPv4Address()}:81/" ;
      // private const string IP_ADDRESS = "169.254.79.79" ;

      protected void StartServer() {
         ApplicationSettings.Use.ServerAddress = _serverAddress ;

         var thread = new Thread (() => _watcherServer.Start()) ;
         Thread.Sleep (4000) ;
         thread.Start() ;
      }

      protected JObject SendPostToServer (string path,
                                          JObject values) {
         const int WAIT_TIME_SECONDS = 8 ;

         Debug.WriteLine($"Posting... {_serverAddress}{path}/") ;
         var result = _postSender.Post ($"{_serverAddress}{path}/", values, WAIT_TIME_SECONDS) ;
         Debug.WriteLine ($"Response {result}") ;
         
         Logger.Log ($"Posted: {path}.");

         if (!result.Properties().Any()) {
            throw new Exception ($"Empty response from address '{path}'.") ;
         }

         // Return result
         return result ;
      }

      protected void SendPostToServerAsync (string path,
                                            JObject values) {

         _postBehavior?.Execute ("Post", () => {
            try {
               SendPostToServer (path, values) ;
            } catch (Exception e) {
               Logger.Log (e.GetAllMessages()) ;
            }
         }) ;
      }

      protected string Login() {
         JObject parameters = new JObject() ;
         // Setup configuration
         if (string.IsNullOrEmpty (ApplicationSettings.Use.ManagementServerID)) {
            ApplicationSettings.Use.ManagementServerID = Guid.NewGuid().ToString() ;
         }

         if (string.IsNullOrEmpty (ApplicationSettings.Use.ManagementServerAddress)) {
            ApplicationSettings.Use.ManagementServerAddress = @"http://127.0.0.1" ;
         }
         var loginPassword = Hashing.GenerateHash (ManagementServer.AccessUID, ApplicationSettings.Use.ManagementServerID) ;
         //    Configuration.LoginPasswordHash = Hashing.GenerateHash (loginPassword, Configuration.ManagementServerID) ;DTAP not stored anymore

         parameters ["password"] = loginPassword ;

         Parameters result = new Parameters (SendPostToServer ("admin/login", parameters)) ;

         Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS],
                          result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
         string token = result ["token"] ;
         Assert.IsFalse (string.IsNullOrEmpty (token)) ;

         return token ;
      }

      protected void CheckStatus (string token,
                                  int packageCount,
                                  bool checkPackageCountOnly = false) {
         JObject parameters = new JObject() ;
         parameters ["token"] = token ;

         // Sleep a bit, to enable the performance counters collect data
         Thread.Sleep (1000) ;

         Parameters result = new Parameters (SendPostToServer ("admin/getStatus", parameters)) ;
         var status = new Parameters (result ["status"]) ;

         Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS], result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;

         if (!checkPackageCountOnly) {
            Assert.GreaterOrEqual (Kernel.Utility.Converter.StringToDouble (status ["cpu"], 0), 0.1) ;
            Assert.LessOrEqual (Kernel.Utility.Converter.StringToDouble (status ["cpu"], 0), 100) ;

            Assert.Greater (Kernel.Utility.Converter.StringToDouble (status ["memory"], 0), 0.1) ;
            Assert.Less (Kernel.Utility.Converter.StringToDouble (status ["memory"], 0), 20000) ;

            Assert.Greater (Kernel.Utility.Converter.StringToDouble (status ["hdd_free_gb"], 0), 0.1) ;

            //Assert.Greater (Conversion.StringToDouble (status ["average_latency"], 0), 0) ;
            //Assert.Less (Conversion.StringToDouble (status ["average_latency"], 0), 100) ;

            Assert.Greater (Kernel.Utility.Converter.StringToDouble (status ["stress_level"], 0), 0.1) ;
            Assert.Less (Kernel.Utility.Converter.StringToDouble (status ["stress_level"], 0), 100) ;
         }

         Assert.AreEqual (packageCount, Kernel.Utility.Converter.StringToInt (status ["package_count"], 0)) ;
      }
   }
}
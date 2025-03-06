using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Logging ;
using System.Diagnostics ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues ;

namespace Guartinel.ManagementServer.APITests {
   public class TestsBase {
      protected static class Constants {
         public static class Environment {
            public const string TEST_CONTEXT_PARAMETER_ENVIRONMENT_KEY = "environment" ;

            public static class Test {
               public const string VALUE = "TEST" ;
               public static string ADDRESS = @"https://test.guartinel.com:9091/" ;
            }

            public static class Dev {
               public const string VALUE = "DEV" ;
               public static string ADDRESS = @"https://dev.guartinel.com:9090/" ;
            }

            public static class Prod {
               public const string VALUE = "PROD" ;
               public static string ADDRESS = @"https://backend.guartinel.com:9090/" ;
            }
         }

         public const string EASY_TEST_CATEGORY = "EASY_TEST" ;
         public const string EASY_TEST_PREPARE = "EASY_TEST_PREPARE" ;
         public const string EASY_TEST_CLEAN_UP = "EASY_TEST_CLEAN_UP" ;

         public const string LOGIN_PASSWORD = @"zA9tXv378h" ; // TEST 
         // public const string PRODUCTION!!!! LOGIN_PASSWORD = "6w7GtuU1hdubILW1eQVN"; // PRODUCTION!!!!

         // public const int TEST_ACCOUNT_COUNT = 100 ;
         public const int TEST_ACCOUNT_COUNT = 1 ;

         public const string EMAIL_PARAMETER = "email" ;
         public const string PASSWORD_PARAMETER = "password" ;

         public const string TOKEN_PARAMETER = "token" ;
         public const string PACKAGE_NAME_PARAMETER = "package_name" ;

         public const double MAX_DIFF = 0.01 ;

         public const string API_IDENTITIFACTION_TOKEN = @"hxAIJWAvzmx07U7tuwI9" ;
      }

      private string ServerAddress {get ; set ;}

      [SetUp]
      public void Setup() {
         string testEnvironment = TestContext.Parameters [Constants.Environment.TEST_CONTEXT_PARAMETER_ENVIRONMENT_KEY] ;

         switch (testEnvironment) {
            case Constants.Environment.Test.VALUE:
               ServerAddress = Constants.Environment.Test.VALUE ;
               break ;
            case Constants.Environment.Dev.VALUE:
               ServerAddress = Constants.Environment.Dev.VALUE ;
               break ;
            case Constants.Environment.Prod.VALUE:
               ServerAddress = Constants.Environment.Prod.VALUE ;
               break ;
            default:
               ServerAddress = Constants.Environment.Dev.VALUE;
            break;
               ;
         }

         RegisterLoggers() ;
         Setup1() ;
      }

      [TearDown]
      public void TearDown() {
         TearDown1() ;
         IoC.Use.Clear() ;
      }

      protected virtual void RegisterLoggers() {
         Logger.Setup<NullLogger> ("GuartinelMSTests", "Guartinel Management Server Unit Tests") ;
         // Logger.Setup<SimpleConsoleLogger> (new LoggerSettings("GuartinelMSTests", "Guartinel Management Server Unit Tests"));

         // Logger.RegisterLogger (() => new ConsoleLogger (nameof (Timeout))) ;
         // Logger.Setup<ConsoleLogger>(new LoggerSettings ("GuartinelWatcherTest", "Guartinel Watcher Test"));
         // Logger.Setup (new LoggerSettings("GuartinelWatcherTest", "Guartinel Watcher Test"));
         // Logger.RegisterLogger (() => new FileLogger ("test1", "289E9A45-FFC8-43C6-9FA1-E47C586D4EEA")) ;         
      }

      protected virtual void Setup1() { }

      protected virtual void TearDown1() { }

      protected string GetAccount (int accountIndex) {
         var account = $"tester_account{accountIndex.ToString().PadLeft (4, '0')}@sysment.hu" ;
         //var account = "tester@sysment.hu";
         // var PRODUCTION!!!! account = "tester_account0001@sysment.hu";
         // Debug.WriteLine ($"Account: {account}") ;
         return account ;
      }

      protected readonly PostSender _postSender = new PostSender() ;

      protected JObject SendPostToServer (string path,
                                          JObject values) {
         const int WAIT_TIME_MILLISECONDS = 5000 ;
         var result = _postSender.Post ($@"{ServerAddress}{path}/", values, WAIT_TIME_MILLISECONDS) ;
         Assert.IsTrue (result.Properties().Any()) ;

         return result ;
      }

      protected void SendPostToServerAsync (string path,
                                            JObject values) {
         new ExecuteBehavior.InTaskQueue ("Test server posts").Execute ("Post", () => {
            try {
               SendPostToServer (path, values) ;
            } catch (Exception e) {
               Logger.Log (e.GetAllMessages()) ;
            }
         }) ;
      }

      protected void CheckSuccess (ConfigurationData result) {
         Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS],
                          result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
      }

      protected void CheckError (ConfigurationData result,
                                 string errorPart = null) {
         Assert.IsTrue (string.IsNullOrEmpty (result [WatcherServerAPI.GeneralResponse.Names.SUCCESS])) ;

         if (!string.IsNullOrEmpty (errorPart)) {
            Assert.IsTrue (result [WatcherServerAPI.GeneralResponse.Names.ERROR].Contains (errorPart)) ;
         }
      }

      protected string GetToken() {
         return GetToken (new Random().Next (Constants.TEST_ACCOUNT_COUNT)) ;
      }

      protected string GetToken (string accountEmail) {
         JObject parameters = new JObject() ;
         parameters ["email"] = accountEmail ;
         parameters ["password"] = Constants.LOGIN_PASSWORD ;

         ConfigurationData result = new ConfigurationData (SendPostToServer ("api/getToken", parameters)) ;

         CheckSuccess (result) ;

         string token = result [Constants.TOKEN_PARAMETER] ;
         Assert.IsFalse (string.IsNullOrEmpty (token)) ;

         return token ;
      }

      protected string GetToken (int accountIndex) {
         JObject parameters = new JObject() ;
         parameters ["email"] = GetAccount (accountIndex) ;
         parameters ["password"] = Constants.LOGIN_PASSWORD ;

         ConfigurationData result = new ConfigurationData (SendPostToServer ("api/getToken", parameters)) ;

         CheckSuccess (result) ;

         string token = result [Constants.TOKEN_PARAMETER] ;
         Assert.IsFalse (string.IsNullOrEmpty (token)) ;

         return token ;
      }

      protected int GetPackageVersion (string token,
                                       string packageName) {
         JObject parameters = new JObject() ;
         parameters [Constants.TOKEN_PARAMETER] = token ;
         parameters [Constants.PACKAGE_NAME_PARAMETER] = packageName ;

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("api/package/getVersion", parameters)) ;
         CheckSuccess (resultParameters) ;

         return resultParameters.AsInteger ("version") ;
      }

      protected void CreateAccount (string email,
                                    string password) {
         JObject parameters = new JObject() ;
         parameters [Constants.EMAIL_PARAMETER] = email ;
         parameters [Constants.PASSWORD_PARAMETER] = Hashing.GenerateHash (password, email) ;
         parameters [Constants.TOKEN_PARAMETER] = Constants.API_IDENTITIFACTION_TOKEN ;
         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("account/create", parameters)) ;
         CheckSuccess (resultParameters) ;
      }

      protected void DeleteAccount (string email,
                                    string password) {
         JObject parameters = new JObject() ;
         parameters [Constants.EMAIL_PARAMETER] = email ;
         parameters [Constants.PASSWORD_PARAMETER] = Hashing.GenerateHash (password, email) ;

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("account/delete", parameters)) ;
         CheckSuccess (resultParameters) ;
      }

      protected ConfigurationData GetPackage (string token,
                                              string packageName) {
         JObject parameters = new JObject() ;
         parameters [Constants.TOKEN_PARAMETER] = token ;
         parameters [Constants.PACKAGE_NAME_PARAMETER] = packageName ;

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("api/package/getPackage", parameters)) ;
         CheckSuccess (resultParameters) ;

         return resultParameters.GetChild ("package") ;
      }

      protected List<Package> GetAllPackages (string token) {
         JObject parameters = new JObject() ;
         parameters [Constants.TOKEN_PARAMETER] = token ;

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("api/package/getAll", parameters)) ;
         CheckSuccess (resultParameters) ;

         var result = new List<Package>() ;

         var packages = resultParameters.GetChildren ("packages") ;
         foreach (var package in packages) {
            result.Add (new Package (package ["package_name"], package.AsInteger ("version"))) ;
         }

         Logger.Log ($"{packages.Count} packages returned.") ;

         return result ;
      }

      protected void SavePackage (string token,
                                  string packageName,
                                  string packageType,
                                  List<string> alertEmails,
                                  List<string> alertDevices,
                                  int checkIntervalSeconds,
                                  bool isEnabled,
                                  Action<JObject> configure) {
         JObject parameters = new JObject() ;
         parameters [Constants.TOKEN_PARAMETER] = token ;

         JObject package = new JObject() ;

         package ["package_name"] = packageName ;
         package ["package_type"] = packageType ;
         package ["check_interval_seconds"] = checkIntervalSeconds ;

         if (alertEmails != null) {
            package ["alert_emails"] = Converter.CreateJArray (alertEmails) ;
         }

         if (alertDevices != null) {
            package ["alert_devices"] = Converter.CreateJArray (alertDevices) ;
         }

         package ["is_enabled"] = isEnabled ;

         if (configure != null) {
            JObject configuration = new JObject() ;
            configure (configuration) ;
            package ["configuration"] = configuration ;
         }

         parameters ["package"] = package ;

         // Debug.WriteLine(parameters.ToString());

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("api/package/save", parameters)) ;
         CheckSuccess (resultParameters) ;

         Logger.Log ($"Package {packageName} created.") ;
      }

      protected void SaveWebsiteSupervisorPackage (string token,
                                                   string packageName,
                                                   List<string> websites,
                                                   List<string> alertEmails,
                                                   List<string> alertDevices,
                                                   int checkIntervalSeconds = 60,
                                                   bool isEnabled = false) {
         SavePackage (token, packageName, "WEBSITE_SUPERVISOR", alertEmails, alertDevices, checkIntervalSeconds, isEnabled,
                      configuration => configuration ["websites"] = Converter.CreateJArray (websites)) ;
      }

      protected void SaveHostSupervisorPackage (string token,
                                                string packageName,
                                                List<string> hosts,
                                                List<string> alertEmails,
                                                List<string> alertDevices,
                                                int checkIntervalSeconds = 60,
                                                bool isEnabled = false) {
         SavePackage (token, packageName, "HOST_SUPERVISOR", alertEmails, alertDevices, checkIntervalSeconds, isEnabled,
                      configuration => {configuration ["hosts"] = Converter.CreateJArray (hosts) ;}) ;
      }

      protected void SaveApplicationSupervisorPackage (string token,
                                                       string packageName,
                                                       List<string> alertEmails,
                                                       List<string> alertDevices,
                                                       int checkIntervalSeconds = 60,
                                                       bool isEnabled = false) {
         SavePackage (token, packageName, "APPLICATION_SUPERVISOR", alertEmails, alertDevices, checkIntervalSeconds, isEnabled,
                      configuration => {configuration ["application_token"] = "" ;}) ;
      }

      public class ComputerSupervisorCheckThresholds {
         private readonly JObject _thresholds = new JObject() ;

         public ComputerSupervisorCheckThresholds CPU (int maxPercent) {
            _thresholds ["cpu"] = JObject.FromObject (new {max_percent = maxPercent}) ;
            return this ;
         }

         public ComputerSupervisorCheckThresholds Memory (int minFreeGB = -1,
                                                          int maxUsagePercent = -1) {
            JObject memory = new JObject() ;
            if (minFreeGB != -1) {
               memory ["min_free_gb"] = minFreeGB ;
            }

            if (maxUsagePercent != -1) {
               memory ["max_percent"] = maxUsagePercent ;
            }

            _thresholds ["memory"] = memory ;
            return this ;
         }

         public ComputerSupervisorCheckThresholds HDD (string volume,
                                                       int minFreeGB = -1,
                                                       int maxUsagePercent = -1) {
            JObject hdd = new JObject() ;
            if (minFreeGB != -1) {
               hdd ["min_free_gb"] = minFreeGB ;
            }

            if (maxUsagePercent != -1) {
               hdd ["max_percent"] = maxUsagePercent ;
            }

            if (_thresholds.GetValue ("hdd") == null) {
               _thresholds ["hdd"] = new JArray() ;
            }

            ((JArray) _thresholds ["hdd"]).Add (hdd) ;
            return this ;
         }

         public JObject GetJObject() {
            return _thresholds ;
         }
      }

      protected void SaveComputerSupervisorPackage (string token,
                                                    string packageName,
                                                    List<string> alertEmails,
                                                    List<string> alertDevices,
                                                    ComputerSupervisorCheckThresholds thresholds,
                                                    int checkIntervalSeconds = 300,
                                                    bool isEnabled = false) {
         SavePackage (token, packageName, "COMPUTER_SUPERVISOR", alertEmails, alertDevices, checkIntervalSeconds, isEnabled,
                      configuration => {
                         // Use specific parameters to configure package: cpu, mem, hdd ...
                         configuration ["check_thresholds"] = thresholds.GetJObject() ;
                         configuration ["agent_categories"] = new JArray {"test"} ;
                      }) ;
      }

      protected void SetPackageEnabled (string token,
                                        string packageName,
                                        bool isEnabled) {
         JObject parameters = new JObject() ;
         parameters [Constants.TOKEN_PARAMETER] = token ;

         JObject package = new JObject() ;

         package ["package_name"] = packageName ;
         package ["is_enabled"] = isEnabled ? "true" : "false" ;

         parameters ["package"] = package ;

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("api/package/save", parameters)) ;
         CheckSuccess (resultParameters) ;
      }

      protected void DisableAllPackages (string token) {
         var packages = GetAllPackages (token) ;
         foreach (var package in packages) {
            var elapsed = StopwatchEx.TimeIt (() => {
               SetPackageEnabled (token, package.Name, false) ;
            }) ;

            Debug.WriteLine ($"Package {package.Name} disabled in {elapsed.TotalSeconds} seconds.") ;
         }
      }

      protected void EnableAllPackages (string token) {
         var packages = GetAllPackages (token) ;
         foreach (var package in packages) {
            var elapsed = StopwatchEx.TimeIt (() => {
               SetPackageEnabled (token, package.Name, true) ;
            }) ;

            Debug.WriteLine ($"Package {package.Name} enabled in {elapsed.TotalSeconds} seconds.") ;
         }
      }

      protected void DeletePackage (string token,
                                    string packageName) {
         JObject parameters = new JObject() ;
         parameters [Constants.TOKEN_PARAMETER] = token ;

         parameters ["package_name"] = packageName ;

         ConfigurationData resultParameters = new ConfigurationData (SendPostToServer ("api/package/delete", parameters)) ;
         CheckSuccess (resultParameters) ;
      }

      protected void DeleteAllPackages (string token) {
         var packages = GetAllPackages (token) ;
         foreach (var package in packages) {
            var elapsed = StopwatchEx.TimeIt (() => {
               DeletePackage (token, package.Name) ;
            }) ;

            Debug.WriteLine ($"Package {package.Name} deleted in {elapsed.TotalSeconds} seconds.") ;
            // Logger.Log ($"Package {package.Name} deleted.") ;
         }
      }
   }
}

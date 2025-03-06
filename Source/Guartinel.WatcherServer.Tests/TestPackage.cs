using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using Guartinel.Kernel.Configuration ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Tests {
   public class TestChecker : Checker {
      public bool Success {get ; private set ;}
      public int? MinValue {get ; private set ;}
      public int? MaxValue {get ; private set ;}
      public int? CheckSleepMilliSeconds {get ; private set ;}

      public TestChecker Configure (string name,
                                    string packageID,
                                    string instanceID,
                                    bool success,
                                    int? minValue = null,
                                    int? maxValue = null,
                                    int? checkSleepMilliSeconds = null) {
         Configure (name, packageID, instanceID) ;

         Success = success ;
         MinValue = minValue ;
         MaxValue = maxValue ;
         CheckSleepMilliSeconds = checkSleepMilliSeconds ;

         return this ;
      }

      protected override IList<CheckResult> Check1 (string[] tags) {
         if (CheckSleepMilliSeconds != null) {
            Thread.Sleep (CheckSleepMilliSeconds.Value) ;
         }

         return new List<CheckResult> {new CheckResult().Configure ("Result1",
                                                                    Success ? CheckResultKind.Success : CheckResultKind.Fail,
                                                                    null, null, null)                  
         } ;
      }
   }

   public class TestAlertCounter {
      public TestAlertCounter() {}

      public volatile AlertInfo AlertInfo = null ;
      public volatile int AlertCount = 0 ;
   }

   public class TestPackage : Package {
      private bool _success ;
      private int _checkerCount ;
      public TestAlertCounter TestAlertCounter {get ; private set ;}

      private int? _minValue ;
      private int? _maxValue ;
      private int? _checkSleepInMilliSeconds ;

      public new static class Constants {
         public const string PACKAGE_TYPE = "TestPackage" ;
         public const string CAPTION = "Test Package" ;

         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> {PACKAGE_TYPE} ;

         public const string CHECKER_COUNT = "checker_count" ;
         public const string SUCCESS = "success" ;
         public const string MIN_VALUE = "min_value" ;
         public const string MAX_VALUE = "max_value" ;
         public const string CHECK_SLEEP_IN_MILLI_SECONDS = "check_sleep_in_milliseconds" ;
      }

      public TestPackage Configure (string id,
                                    bool success,
                                    TestAlertCounter testAlertCounter = null,
                                    List<string> alertEmails = null,
                                    List<string> alertDeviceIDs = null,
                                    int checkIntervalSeconds = 10,
                                    int timeoutIntervalSeconds = 30,
                                    int startupDelaySeconds = 0,
                                    int checkerCount = 1,
                                    int? minValue = null,
                                    int? maxValue = null,
                                    int? checkSleepInMilliSeconds = null) {
         Configure (id, alertEmails, alertDeviceIDs, checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds, true) ;

         TestAlertCounter = testAlertCounter ?? new TestAlertCounter() ;

         _alerts.Add (new TestAlert (TestAlertCounter).Configure (Name, id, 0)) ;

         _success = success ;
         _checkerCount = checkerCount ;
         List<string> instanceIDs = new List<string>() ;
         for (int checkerIndex = 0; checkerIndex < checkerCount; checkerIndex++) {
            instanceIDs.Add (GenerateInstanceID (checkerIndex)) ;
         }
         SetInstances (instanceIDs) ;

         _minValue = minValue ;
         _maxValue = maxValue ;
         _checkSleepInMilliSeconds = checkSleepInMilliSeconds ;

         return this ;
      }

      public TestPackage Configure (bool success,
                                    TestAlertCounter testAlertCounter = null,
                                    int checkerCount = 1,
                                    int? minValue = null,
                                    int? maxValue = null,
                                    int? checkSleepInMilliSeconds = null) {
         Configure (Guid.NewGuid().ToString(), success, testAlertCounter, null, null, 10, 15, 1, checkerCount, minValue, maxValue, checkSleepInMilliSeconds) ;

         return this ;
      }

      protected override void SpecificConfigure (ConfigurationData configuration) {
         _checkerCount = configuration.AsInteger (Constants.CHECKER_COUNT) ;
         _success = configuration.AsBoolean (Constants.SUCCESS) ;
         _minValue = configuration.AsInteger (Constants.MIN_VALUE) ;
         _maxValue = configuration.AsInteger (Constants.MAX_VALUE) ;
         _checkSleepInMilliSeconds = configuration.AsInteger (Constants.CHECK_SLEEP_IN_MILLI_SECONDS) ;
      }

      private static string GenerateInstanceID (int index) {
         return $"instanceID{index}" ;
      }

      protected override List<Checker> CreateCheckers1() {
         var checkers = new List<Checker>() ;
         for (var checkerIndex = 0; checkerIndex < _checkerCount; checkerIndex++) {
            checkers.Add (new TestChecker().Configure (Name, ID, GenerateInstanceID (checkerIndex), _success, _minValue, _maxValue, _checkSleepInMilliSeconds)) ;
         }

         return checkers ;
      }

      public TestAlert CreateTestAlert() {
         return Alerts.FirstOrDefault (x => x is TestAlert) as TestAlert ;
      }

      public override int CalculateWorkload() {
         return Package.Constants.DEFAULT_WORKLOAD * _checkerCount ;
      }
   }
}

using System;
using System.Collections.Generic;
using Guartinel.Kernel;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.WatcherServer.Checkers;
using Guartinel.WatcherServer.CheckResults;
using Guartinel.WatcherServer.Communication.ManagementServer;
using Newtonsoft.Json.Linq;
using MeasurementConstants = Guartinel.Communication.Supervisors.HostSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement ;
using Strings = Guartinel.Communication.Supervisors.HostSupervisor.Strings ;

namespace Guartinel.WatcherServer.Supervisors.HostSupervisor {
   public class HostChecker : CheckerWithMeasuredData {

      public class Constants {
         public const string CAPTION = "Host Availability Status Checker";
         //public const string SUCCESS_EXTRACT = "Device is accessible." ;
         //public const string FAIL_EXTRACT = "Device is unavailable." ;
         public const int DEFAULT_TRY_COUNT = 4;
         public const int DEFAULT_RETRY_WAIT_SECONDS = 5;
         public const int TIMEOUT_SECONDS = 10;
      }

      #region Construction
      public HostChecker (IMeasuredDataStore measuredDataStore) : base(measuredDataStore) { }
      #endregion

      protected void StoreMeasuredData (PingResult result) {
         if (result == null) return ;

         var measurement = new JObject() ;
         measurement.Add (MeasurementConstants.Request.SUCCESS, result.Success) ;
         measurement.Add (MeasurementConstants.Request.PING_TIME_MILLISECONDS, result.PingTimeInMilliseconds) ;
         measurement.Add (MeasurementConstants.Request.MESSAGE, result.Message?.AsJObject()) ;
         measurement.Add (MeasurementConstants.Request.DETAILS, result.Details?.AsJObject()) ;

         _measuredDataStore?.StoreMeasuredData (PackageID,
                                              MeasurementConstants.TYPE_VALUE,
                                              DateTime.UtcNow,
                                              measurement) ;
      }

      protected override IList<CheckResult> Check1(string[] tags) {
         var logger = new TagLogger (tags) ;
         var checkResult = new CheckResult() ;

         PingResult pingResult = new PingResult (new Pinger().Ping (Host,
                                                                    TryCount ?? Constants.DEFAULT_TRY_COUNT,
                                                                    false,
                                                                    RetryWaitSeconds ?? Constants.DEFAULT_RETRY_WAIT_SECONDS,
                                                                    Constants.TIMEOUT_SECONDS,
                                                                    logger.Error),
                                                 Host
                                                );

         StoreMeasuredData (pingResult) ;

         checkResult.CheckResultKind = pingResult.Success ? CheckResultKind.Success : CheckResultKind.Fail ;
         checkResult.Message = pingResult.Message ;
         checkResult.Details = pingResult.Details ;
         checkResult.Extract = pingResult.Extract ;
         checkResult.TimeStamp = DateTime.UtcNow ;
         checkResult.Name = Name ;

         return new List<CheckResult> {checkResult} ;
      }

      #region Ping result
      public class PingResult {
         public PingResult (Kernel.Network.PingResult pingResult,
                            Host host) {
            switch (pingResult.Success) {
               case PingSuccess.Success:
                  Success = true ;
                  PingTimeInMilliseconds = pingResult.RoundtripMilliseconds ;
                  Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.HostIsOKMessage),
                                                 new XConstantString.Parameter (Strings.Parameters.Host, host.DisplayText)) ;
                  Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.HostIsOKDetails),
                                                 new XConstantString.Parameter (Strings.Parameters.Host, host.DisplayText),
                                                 new XConstantString.Parameter (Strings.Parameters.LatencyInMilliseconds, pingResult.RoundtripMilliseconds)) ;
                  Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.HostIsOKExtract),
                                                 new XConstantString.Parameter (Strings.Parameters.LatencyInMilliseconds, pingResult.RoundtripMilliseconds)) ;
                  break ;

               case PingSuccess.Fail:
                  Success = false ;
                  PingTimeInMilliseconds = null ;
                  Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.HostIsNotAvailableAlertMessage),
                                                 new XConstantString.Parameter (Strings.Parameters.Host, host.DisplayText)) ;
                  Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.HostIsNotAvailableAlertDetails),
                                                 new XConstantString.Parameter (Strings.Parameters.Host, host.DisplayText),
                                                 new XConstantString.Parameter (Strings.Parameters.ErrorMessage, pingResult.Message)) ;
                  Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.HostIsNotAvailableAlertExtract)) ;
                  break ;
            }
         }

         public bool Success {get ;}
         public long? PingTimeInMilliseconds {get ;}
         public XString Message {get ;}
         public XString Details { get; }
         public XString Extract { get; }
      }

      #endregion

      #region Configuration
      public HostChecker Configure (string name,
                                    string packageID,
                                    Host host,
                                    int? tryCount = Constants.DEFAULT_TRY_COUNT,
                                    int? retryWaitSeconds = Constants.DEFAULT_RETRY_WAIT_SECONDS) {
         base.Configure (name, packageID, host.Address) ;

         Host = host ;
         TryCount = tryCount ?? Constants.DEFAULT_TRY_COUNT;
         RetryWaitSeconds = retryWaitSeconds ?? Constants.DEFAULT_RETRY_WAIT_SECONDS ;

         return this ;
      }

      //protected override void Duplicate1 (Entity target) {
      //   (target.CastTo<HostChecker>()).Configure (Name, PackageID, HostAddress, RetryCount.GetValueOrDefault(), WaitTimeSeconds.GetValueOrDefault()) ;
      //}

      //protected override Entity Create() {
      //   return new HostChecker (_measuredDataStore) ;
      //}
      #endregion

      #region Properties
      public Host Host {get ; private set ;}
      public int? TryCount {get ; private set ;}
      public int? RetryWaitSeconds {get ; private set ;}
      #endregion
   }
}
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Net.NetworkInformation ;
using System.Text;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.CheckResults ;

namespace Guartinel.WatcherServer.Checkers.Network {
   public class PingChecker : Checker, IDuplicable<PingChecker> {
      public class Constants {
         public const string CAPTION = "Ping Checker" ;

         public const int PING_TRY_COUNT = 4 ;
      }

      public static class ParameterNames {
         public const string ADDRESS = "address" ;
      }

      #region Construction
      public PingChecker() {
         Address = string.Empty ;
      }

      //public static Creator GetCreator() {
      //   return new Creator (typeof (PingChecker), () => new PingChecker(), typeof (Checker), Constants.CAPTION) ;
      //}
      #endregion

      #region Configuration
      public new PingChecker Configure (string name,
                                        string packageID,
                                        string address) {
         base.Configure (name, packageID, address) ;

         Address = address ;

         return this ;
      }

      public PingChecker Duplicate () {
         return new PingChecker().Configure (Name, PackageID, Address) ;
      }

      //protected override void Configure2 (ConfigurationData configurationData) {
      //   Address = configurationData [ParameterNames.ADDRESS] ;
      //}

      //protected override void Duplicate1 (Entity target) {
      //   // target.CastTo<PingChecker>().Configure (Name, Enabled, Address) ;
      //   target.CastTo<PingChecker>().Configure (Name, PackageID, Address) ;
      //}

      //protected override Entity Create() {
      //   return new PingChecker() ;
      //}
      #endregion

      #region Summary
      //protected override void AddSpecificSummary (List<string> result) {
      //   result.Add (String.Format ("{0}{1}", HTML.MakeBold ("Address: "), Address)) ;
      //}
      #endregion

      #region Properties
      public string Address {get ; set ;}
      #endregion

      protected override IList<CheckResult> Check1(string[] tags) {
         var pingResult = PingHost (Address, tags) ;

         return new List<CheckResult> {new CheckResult().Configure (Name,
                                                                    pingResult ? CheckResultKind.Success : CheckResultKind.Fail, null, null, null)                  
         } ;
      }

      private static bool PingHost (string nameOrAddress,
                                    string[] tags) {
         var logger = new TagLogger(tags, nameOrAddress) ;
         bool pingable = false ;
         Ping pinger = new Ping() ;

         logger.Debug ($"Pinging {nameOrAddress} {Constants.PING_TRY_COUNT} times...") ;

         for (var pingIndex = 0; pingIndex < Constants.PING_TRY_COUNT; pingIndex++) {
            logger.Debug ($"Ping try {pingIndex + 1}...") ;

            try {
               PingReply reply = pinger.Send (nameOrAddress) ;
               if (reply == null) {
                  logger.Debug ($"No ping reply.") ;
                  return false ;
               }
               pingable = reply.Status == IPStatus.Success ;
               // Console.WriteLine("Ping: " + nameOrAddress+ " pingable: "+ pingable);

               if (pingable) {
                  logger.Debug($"Successfully pinged.");
                  break ;
               }
            } catch (PingException e) {
               logger.Debug ($"Ping exception: {e.GetAllMessages()}") ;
               // Discard PingExceptions and return false;
            }
         }

         return pingable ;
      }
   }
}

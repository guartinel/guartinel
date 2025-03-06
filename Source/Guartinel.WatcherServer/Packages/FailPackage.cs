using System ;
using System.Linq ;
using System.Text ;
using Guartinel.WatcherServer.CheckResults ;

namespace Guartinel.WatcherServer.Packages {
   public class FailPackage : Package {
      public new static class Constants {
         public const string NAME = "FailPackage";
      }

      //protected override bool CheckIfAlertNeeded (CheckResult checkResult) {
      //   return checkResult.Success == CheckResultSuccess.Fail ;
      //}

      #region Construction

      //public new static Creator GetCreator() {
      //   return new Creator (typeof (FailPackage), () => new FailPackage(), typeof (Package), Constants.NAME) ;
      //}

      #endregion
   }
}
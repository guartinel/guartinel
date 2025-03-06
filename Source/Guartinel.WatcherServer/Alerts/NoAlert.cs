using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Entities ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Alerts {
   public class NoAlert : Alert {
      public new static class Constants {
         public const string CAPTION = "No Alert" ;
      }

      #region Construction

      //public static Creator GetCreator() {
      //   return new Creator (typeof (NoAlert), () => new NoAlert(), typeof (Alert), Constants.CAPTION) ;
      //}

      #endregion



      //protected override void Configure2 (ConfigurationData configurationData) {
      //   // Nothing to do
      //}

      //protected override void Duplicate2 (Alert target) {
      //   // Nothing to do
      //}

      //protected override Entity Create() {
      //   return new NoAlert();
      //}
      protected override string Caption => Constants.CAPTION ;

      protected override void Fire1 (Instance instance,
                                     AlertInfo alertInfo) {
         // Nothing to do here!
      }
   }
}
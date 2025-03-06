using System ;
using System.IO ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Configuration ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests {
   [TestFixture]
   public class ScheduleTests {
      [SetUp]
      public void SetUp() {
         Schedules.Register() ;
      }

      [Test]
      public void CreateScheduleFromConfiguration() {
         ConfigurationData configuration = new ConfigurationData() ;
         configuration ["type"] = "daily" ;
         configuration ["time"] = "04:00:00.313" ;
         configuration.AsJObject ["is_enabled"] = true ;
         configuration.AsJObject ["interval_in_seconds"] = 7200 ;
         var schedule = (ScheduleDaily) Schedule.CreateSchedule (configuration) ;

         // Assert.AreEqual (TimeSpan.Parse ("06:00:00.313"), schedule.StartTime) ;                  
         Assert.AreEqual (TimeZoneInfo.ConvertTimeFromUtc (new DateTime (2018, 07, 01, 4, 0, 0), TimeZoneInfo.Utc).TimeOfDay.Add (TimeSpan.FromMilliseconds (313)), schedule.StartTime) ;
      }
   }
}

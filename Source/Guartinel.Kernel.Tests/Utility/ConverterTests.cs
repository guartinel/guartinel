using System ;
using System.Linq ;
using System.Text ;
using NUnit.Framework ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Kernel.Tests.Utility {
   [TestFixture]
   internal class ConverterTests {
      [Test]
      public void TestConvertJsonDates() {
         string datetimeString1 = Converter.DateTimeToStringJson (new DateTime (2016, 06, 30, 18, 22, 45)) ;
         Assert.AreEqual (new DateTime (2016, 06, 30, 18, 22, 45), Converter.StringToDateTimeJson (datetimeString1)) ;
      }

      [Test]
      public void TestTimeSpan() {
         var timeSpan = Converter.StringToTimeSpan ("04:12:23.313Z") ;
         Assert.AreEqual (4, timeSpan.Hours) ;
         Assert.AreEqual (12, timeSpan.Minutes) ;
         Assert.AreEqual (23, timeSpan.Seconds) ;
         Assert.AreEqual (313, timeSpan.Milliseconds) ;
      }
   }
}
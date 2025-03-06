using System ;
using System.Diagnostics ;
using System.Linq ;
using System.Text ;
using NUnit.Framework ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Kernel.Tests.Utility {
   [TestFixture]
   internal class FilesExTests {
      [Test]
      public void TestFileAgeWithBigFolder() {
         int ageSeconds = 0 ;
         
         var elapsed = StopwatchEx.TimeIt (() => {
            ageSeconds = FilesEx.GetAgeOfLatestFile (@"c:\temp", true) ;
         }) ;

         Debug.WriteLine ($@"Processing c:\temp took {elapsed.TotalSeconds} seconds, the latest file is {ageSeconds} seconds.") ;
      }
   }
}
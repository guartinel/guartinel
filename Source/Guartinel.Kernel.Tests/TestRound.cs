using System ;
using System.Linq ;
using System.Text ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   [TestFixture]
   internal class TestRound {
      [Test]
      public void Test() {
         const double SOMETHING = 1.234564564 ;
         Assert.AreEqual (1.23, Math.Round (SOMETHING, 2)) ;
      }
   }
}

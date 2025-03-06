using System;
using System.Diagnostics ;
using System.Linq;
using System.Net ;
using System.Net.NetworkInformation ;
using System.Text;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests.Network {
   [TestFixture]
   class TestTracert {
      [Test]
      public void SimpleTest() {
         string result = Kernel.Network.Pinger.GetTrace ("www.index.hu") ;
         Assert.IsTrue (result.Contains ("Trace complete."), "Trace is not successful.") ;
      }
   }
}
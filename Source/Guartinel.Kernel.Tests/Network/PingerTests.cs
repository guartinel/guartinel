using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guartinel.Kernel.Network ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests.Network {
   [TestFixture]
   public class PingerTests {
      [Test]
      public void TestWithInvalidPort() {
         var result = new Pinger().Ping (new Host ("www.sysment.com:899"), 1, false, 1, 2) ;

         Assert.AreEqual (PingSuccess.Fail, result.Success, result.Message.ToJsonString()) ;
      }

      private void TestWithPort (string addressWithPort) {
         var result1 = new Pinger().Ping (new Host (addressWithPort), 1, false, 1, 2) ;
         Assert.AreEqual(PingSuccess.Success, result1.Success, result1.Message.ToJsonString()) ;
      }

      [Test]
      public void TestWithNoPort () {
         var result = new Pinger().Ping(new Host("sysment.hu"), 1, false, 1, 2) ;

         Assert.AreEqual(PingSuccess.Success, result.Success, result.Message.ToJsonString());
      }

      [Test]
      public void TestWithValidPort() {
         TestWithPort ("x5gym.dyndns.org:8900") ;
         
         TestWithPort("www.sysment.com:80") ;
         TestWithPort("backend2.guartinel.com:21");
         TestWithPort("x5gym.dyndns.org:8892");

         TestWithPort("manage.guartinel.com:80") ;
         TestWithPort("manage.guartinel.com:443") ;
         TestWithPort("manage.guartinel.com:8080") ;
         TestWithPort("manage.guartinel.com:8888") ;

         TestWithPort("backend.guartinel.com:8080") ;
         TestWithPort ("backend.guartinel.com:9090") ;

      }
   }
}
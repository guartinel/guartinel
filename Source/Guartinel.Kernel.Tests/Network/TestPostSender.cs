using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guartinel.Kernel.Network ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests.Network {   
   [TestFixture]
   public class TestPostSender {
      protected PostSender _postSender = new PostSender();

      [Test]
      public void TestInvalidPath() {
         var result = _postSender.Post(@"https://backend2.guartinel.com:444/thatLittleMouse", new JObject()) ;
         Assert.IsFalse (result.Properties().Any()) ;
      }
   }
}

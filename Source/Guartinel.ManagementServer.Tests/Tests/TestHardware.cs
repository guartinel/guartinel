using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strings = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings;

namespace Guartinel.ManagementServer.Tests.Tests {
   [TestFixture]
   public class TestHardware {

      [Test]
      public void TestCheckForUpdate () {
         var request = new {
            instance_id = "877e3e",
            instance_name = "asdasd",
            version = 1,
         };
         Response response = Connection.Connector.MakeRequest(Strings.ManagementServerRoutes.CheckForUpdate.FULL_URL, request);
         Assert.AreEqual(true, true);
      }
   }
}

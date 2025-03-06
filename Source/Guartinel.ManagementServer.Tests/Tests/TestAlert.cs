using System ;
using NUnit.Framework ;

namespace Guartinel.ManagementServer.Tests.Tests {
   [TestFixture]
   internal class TestAlert {
      /*
      public string _deviceType = "android";
      public string _email;
      public string _password = "test";
      public string _deviceName = "test";
      public string _deviceUUID;
      public string _acccountToken;
      public string _deviceToken;

      private string _checkResult = @"<Results><HddChecker volume=""Disk1"" type=""free%"">39.55297</HddChecker><HddChecker volume=""Disk1"" type=""freemb"">
64802</HddChecker><FileChecker File=""Test.txt"">true</FileChecker><CpuChecker type=""avg%"">2.991683</CpuChecker><MemoryChecker
 type=""free%"">39.95441</MemoryChecker><MemoryChecker type=""free%"">39.95441</MemoryChecker></Results> ";

      [Test]
      public void _0SetUp() {
         _email = "TEST" + new Random().Next(99999999).ToString();
         Response response = MakeRequest("http://localhost:8080/account/create", new {
            email = _email,
            password = _password,
            first_name = "test",
            last_name = "test",
         });

         Assert.AreEqual("SUCCESS", response.content);

         response = MakeRequest("http://localhost:8080/account/login", new {
            email = _email,
            password = _password
         });

         _acccountToken = response.token;
         response = MakeRequest("http://localhost:8080/device/register", new {
            email = _email,
            password = _password,
            device_name = _deviceName,
            device_type = _deviceType,
         });

         Assert.AreEqual("SUCCESS", response.content);
         Assert.NotNull(response.device_uuid);
         _deviceUUID = response.device_uuid;

         response = MakeRequest("http://localhost:8080/device/login", new {
            email = _email,
            password = _password,
            device_uuid = _deviceUUID
         });
         Assert.AreEqual("SUCCESS", response.content);
         _deviceToken = response.token;

         response = MakeRequest("http://localhost:8080/device/android/sync", new {
            token = _deviceToken,
            configuration_time_stamp = "2015-06-02T12:33:45+02:00",
            gcm_id = "test"
         });
         Assert.AreEqual("SUCCESS", response.content);
         }

      [Test]
      public void _1TestGCM() {
         Response response = MakeRequest("http://localhost:8080/alert/gcm", new {
            alert_device_id = "asdfghhhhh",
            message = "test",
         });

         Assert.AreEqual(ConnectionVars.Content.INVALID_DEVICE_UUID, response.content);

         response = MakeRequest("http://localhost:8080/alert/gcm", new {
            alert_device_id = _deviceUUID,
            message = "test",
         });

         Assert.AreEqual("SUCCESS", response.content, "If failed check alertDeviceID existance in DB");
         }*/
   }
}

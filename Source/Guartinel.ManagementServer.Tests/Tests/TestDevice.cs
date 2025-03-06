using System ;
using Guartinel.ManagementServer.Tests.Helpers.Account ;
using Guartinel.ManagementServer.Tests.Helpers.Device ;
using NUnit.Framework ;
using Helper = Guartinel.ManagementServer.Tests.Helpers.Helper ;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.ManagementServer.Tests.Tests {
   [TestFixture]
   internal class TestDevice {
      private AccountImpl _account = new AccountImpl() ;

      [SetUp]
      public void SetUp() {
         _account = new AccountImpl() ;
         _account.Email = Helper.getTestName() ;
         _account.Password = "test" ;

         Response createResponse = AccountHelper.createAccount (_account.Email, _account.Password) ;
         Assert.AreEqual ("SUCCESS", createResponse.success) ;

         Response loginResponse = AccountHelper.loginAccount (_account.Email, _account.Password) ;
         Assert.NotNull (loginResponse.token) ;
         _account.Token = loginResponse.token ;

         Response getInfoResponse = AccountHelper.getStatus (_account.Token) ;
         Assert.NotNull (getInfoResponse.account.id) ;
         _account.Id = getInfoResponse.account.id ;
         _account.ActivationCode = getInfoResponse.account.activation_code ;
         _account.IsActivated = getInfoResponse.account.is_activated ;
      }

      [TearDown]
      public void TearDown() {
         Response deleteResponse = AccountHelper.deleteAccount (_account.Id, _account.Email, _account.Password, _account.Token) ;
         Assert.AreEqual ("SUCCESS", deleteResponse.success) ;
         _account = null ;
      }

      [Test]
      public void TestRegister() {
         DeviceImpl deviceImpl = new DeviceImpl() ;
         deviceImpl.DeviceType = "test" ;
         deviceImpl.DeviceName = Helper.getTestName() ;

         Response response = DeviceHelper.registerDevice ("invalid_email", _account.Password, deviceImpl.DeviceType, deviceImpl.DeviceName) ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = DeviceHelper.registerDevice (_account.Email, "invalid password", deviceImpl.DeviceType, deviceImpl.DeviceName) ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = DeviceHelper.registerDevice (_account.Email, _account.Password, deviceImpl.DeviceType, deviceImpl.DeviceName) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.IsNotNull (response.device_uuid) ;
         Assert.IsNotNull (response.token) ;

         deviceImpl.DeviceUUID = response.device_uuid ;
         deviceImpl.Token = response.token ;

         //try to register with existing name
         response = DeviceHelper.registerDevice (_account.Email, _account.Password, deviceImpl.DeviceType, deviceImpl.DeviceName) ;
         Assert.AreEqual ("DEVICE_NAME_TAKEN", response.error) ;

         //check new device presence
         response = DeviceHelper.getAvailable (_account.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.devices) ;
         Assert.AreEqual (deviceImpl.DeviceUUID, response.devices [0].id) ;
      }

      [Test]
      public void TestAvailable() {
         DeviceImpl deviceImpl = registerDevice() ;

         Response response = DeviceHelper.getAvailable ("invalid_token") ;
         Assert.AreEqual ("INVALID_TOKEN", response.error) ;

         response = DeviceHelper.getAvailable (_account.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.devices) ;
         Assert.AreEqual (1, response.devices.Length) ;
      }

      [Test]
      public void _3TestLogin() {
         DeviceImpl deviceImpl = registerDevice() ;

         Response response = DeviceHelper.loginDevice (_account.Email, _account.Password, "invalid_deviceUUID") ;
         Assert.AreEqual (AllErrorValues.INVALID_DEVICE_UUID, response.error) ;

         response = DeviceHelper.loginDevice (_account.Email, "invalid password", deviceImpl.DeviceUUID) ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = DeviceHelper.loginDevice ("invalid_email", _account.Password, deviceImpl.DeviceUUID) ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = DeviceHelper.loginDevice (_account.Email, _account.Password, deviceImpl.DeviceUUID) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.token) ;
         deviceImpl.Token = response.token ;
      }

      [Test]
      public void TestUnregister() {
         DeviceImpl deviceImpl = registerDevice() ;

         Response response = DeviceHelper.deleteDevice ("invalid_token", deviceImpl.DeviceUUID) ;
         Assert.AreEqual ("INVALID_TOKEN", response.error) ;

         response = DeviceHelper.deleteDevice (_account.Token, "invalid_device_uuid") ;
         Assert.AreEqual ("INVALID_DEVICE_UUID", response.error) ;

         response = DeviceHelper.deleteDevice (_account.Token, deviceImpl.DeviceUUID) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
      }

      private DeviceImpl registerDevice() {
         DeviceImpl deviceImpl = new DeviceImpl() ;
         deviceImpl.DeviceType = "test" ;
         deviceImpl.DeviceName = Helper.getTestName() ;

         Response response = DeviceHelper.registerDevice (_account.Email, _account.Password, deviceImpl.DeviceType, deviceImpl.DeviceName) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.device_uuid) ;
         deviceImpl.DeviceUUID = response.device_uuid ;
         deviceImpl.Token = response.token ;
         return deviceImpl ;
      }

      /*       
      
      [Test]
      public void _4TestCheckResult() {

         Response response = MakeRequest("http://localhost:8080/device/agent/checkresult", new {
            token = "asdasd",
            check_result = _checkResult
         });
         Assert.AreEqual(ConnectionVars.Content.INVALID_TOKEN, response.content);

         response = MakeRequest("http://localhost:8080/device/agent/checkresult", new {
            token = _deviceToken,
            check_result = _checkResult
         });
         Assert.AreEqual("SUCCESS", response.content, "It could be failed if Watcher server isnt running.");
         }

      [Test]
      public void _5TestAndroidSync() {
         Response response = MakeRequest("http://localhost:8080/device/android/sync", new {
            token = "asdasd",
            configuration_time_stamp = new DateTime().ToString(),
            gcm_id = "test"
         });
         Assert.AreEqual(ConnectionVars.Content.INVALID_TOKEN, response.content);

         response = MakeRequest("http://localhost:8080/device/android/sync", new {
            token = _deviceToken,
            configuration_time_stamp = "2015-06-02T12:33:45+02:00",
            gcm_id = "test"
         });
         Assert.AreEqual("SUCCESS", response.content);
         }   


    */
   }
}

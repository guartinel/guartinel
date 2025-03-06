using System ;
using System.Text ;
using System.Web.Script.Serialization ;
using Guartinel.ManagementServer.Tests.Helpers.Account ;
using Guartinel.ManagementServer.Tests.Helpers.Device ;
using Guartinel.ManagementServer.Tests.Helpers.Package ;
using NUnit.Framework ;
using Helper = Guartinel.ManagementServer.Tests.Helpers.Helper ;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.ManagementServer.Tests.Tests {
   [TestFixture]
   internal class TestPackage {
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
         _account.Agent = registerDevice() ;
         _account.AlertDevice = registerDevice() ;
      }

      [TearDown]
      public void TearDown() {
         Response deleteResponse = AccountHelper.deleteAccount (_account.Id, _account.Email, _account.Password, _account.Token) ;
         Assert.AreEqual ("SUCCESS", deleteResponse.success) ;
         _account = null ;
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

      private string getConfigDataString (String agentDeviceId) {
         JavaScriptSerializer serializer = new JavaScriptSerializer() ;
         object configData = new {
            alert_emails = new[] {"test", "test2"},
            alert_devices = new[] {"asdasdasd", "asdasdasd"},
            agent_device = agentDeviceId,
            check_thresholds = new {
               cpu = new {max_percent = 12}
            },
            check_interval = 10
         } ;
         return serializer.Serialize (configData) ;
      }

        [Test]
        public void TestSave() {
            PackageImpl package = new PackageImpl();
            package.ConfigData = getConfigDataString(_account.Agent.DeviceUUID);
            package.PackageName = Helper.getTestName();
            package.PackageType = Communication.Supervisors.ComputerSupervisor.Strings.Use.PackageType ;

         Response response = PackageHelper.savePackage ("invalid_token", package.PackageType, package.PackageName, package.ConfigData) ;
         Assert.AreEqual (AllErrorValues.INVALID_TOKEN, response.error) ;

         // creating new package
         response = PackageHelper.savePackage (_account.Token, package.PackageType, package.PackageName, package.ConfigData) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;

         //testing existence
         response = PackageHelper.getAvailable (_account.Token) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;
         Assert.AreEqual (response.packages.Length, 1) ;
         Assert.AreEqual (package.PackageName, response.packages [0].package_name) ;
         package.Id = response.packages [0].id ;

         //testing package update
         string newPackageName = Helper.getTestName() ;
         response = PackageHelper.savePackage (_account.Token, package.PackageType, newPackageName, package.ConfigData, package.Id) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;

         //testing update success
         response = PackageHelper.getAvailable (_account.Token) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;
         Assert.AreEqual (response.packages.Length, 1) ;
         Assert.AreEqual (package.Id, response.packages [0].id) ;
         Assert.AreEqual (newPackageName, response.packages [0].package_name) ;
      }

        [Test]
        public void TestDelete() {
            PackageImpl package = new PackageImpl();
            package.ConfigData = getConfigDataString(_account.Agent.DeviceUUID);
            package.PackageName = Helper.getTestName();
            package.PackageType = Communication.Supervisors.ComputerSupervisor.Strings.Use.PackageType ;

         // creating new package
         Response response = PackageHelper.savePackage (_account.Token, package.PackageType, package.PackageName, package.ConfigData) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;

         //testing existence
         response = PackageHelper.getAvailable (_account.Token) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;
         Assert.AreEqual (response.packages.Length, 1) ;
         Assert.AreEqual (package.PackageName, response.packages [0].package_name) ;
         package.Id = response.packages [0].id ;

         response = PackageHelper.deletePackage (_account.Token, package.Id) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;

         //testing delete success
         response = PackageHelper.getAvailable (_account.Token) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;
         Assert.AreEqual (response.packages.Length, 0) ;
      }

      /*        

         private string config_data = @"

 ""check_interval_seconds"": ""12"",
     ""alert_emails"": [""email1@email.hu"",	""email2@email.hu""],
     ""alert_devices"": [""asdm892134eeds"",""asdswegh99sgjj""],
     ""agent_device"": ""56da9b0cdcd708b40eca8aef"",	
     ""check_thresholds"": {
         cpu: {
             ""max_percent"": ""12.5""
         },
         memory: {
             ""max_percent"": ""80"",
             min_free_gb: ""1""
         },
         hdd: [{
             ""volume"": ""c"",
             ""max_percent"": ""80"",
             ""min_free_gb"": ""1""
         },
         {
             ""volume"": ""d"",
             ""max_percent"": ""85"",
             ""min_free_gb"": ""2""
         }]
 ";
 
        

   

    [Test]
    public void _2TestAvailable() {
        Response response = MakeRequest(MANAGEMENT_SERVER_HOST + "/package/getavailable", new {
            token = "asdsdf",
            });

        Assert.AreEqual(ConnectionVars.Content.INVALID_TOKEN, response.content);

        response = MakeRequest(MANAGEMENT_SERVER_HOST + "/package/getavailable", new {
            token = _acccountToken,
            });

        Assert.AreEqual("SUCCESS", response.content);
        Assert.NotNull(response.packages);
        Assert.Greater(response.packages.Length, 0);
        foreach(var package in response.packages) {
            if (package.package_name.Equals("testPackage")){
                _packageId = response.packages[0]._id;
                }
            }
        Assert.NotNull(_packageId);           

        }

    [Test]
    public void _4TestUpdate() {
        Response response = MakeRequest(MANAGEMENT_SERVER_HOST + "/package/save", new {//UPDATE
            token = _acccountToken,
            package_type = "test",
            package_name = "test",
            package_id = _packageId,
            config_data = getConfigDataString(),
            });
        Assert.AreEqual("SUCCESS", response.content);
        }

    [Test]
    public void _5TestDelete() {
        Response response = MakeRequest(MANAGEMENT_SERVER_HOST + "/package/delete", new {
            token = "asdasd",
            package_id = _packageId,
            });

        Assert.AreEqual(ConnectionVars.Content.INVALID_TOKEN, response.content);

        response = MakeRequest(MANAGEMENT_SERVER_HOST + "/package/delete", new {
            token = _acccountToken,
            package_id = _packageId,
            });

        Assert.AreEqual("SUCCESS", response.content);

        response = MakeRequest(MANAGEMENT_SERVER_HOST + "/package/getavailable", new {
            token = _acccountToken,
            });
        Assert.AreEqual("SUCCESS", response.content);

        bool isFound = false;
        foreach (var package in response.packages) {
            if (package._id == _packageId) {
                isFound = true;
                }
            }
        Assert.False(isFound);
        }



*/
   }
}

using System.Web.Http.Results;
using Guartinel.Website.Admin.Controllers;
using Guartinel.Website.Admin.Persistance;
using Guartinel.Website.Common.Configuration;
using Guartinel.Website.Common.Tools;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Guartinel.Kernel.Logging;

namespace Guartinel.Website.Admin.Tests.Test {
   [TestFixture]
   public class TestWatcherServerController {
      private static GuartinelAdminWebSiteSettings _settings ;

      [SetUp]
        public void SetUp() {
         _settings = (GuartinelAdminWebSiteSettings) Manager.Load (typeof (GuartinelAdminWebSiteSettings), Constants.CONFIG_PATH) ;
         if (_settings == null) {
           Logger.Log ("Settings are missing. Creating default one.") ;
            _settings = new GuartinelAdminWebSiteSettings() ;
            _settings.ResetDefaultValues() ;
             _settings.ManagementServer = new Common.Configuration.Data.ManagementServer ("test", "192.168.1.82", "description", null);
            Manager.Save (_settings, "") ;
            return ;
         }
      }

      [Test]
      public void TestRegister() {
         AdministratorController administratorController = new AdministratorController() ;
         AdministratorController.AdministratorLoginModel loginModel = new AdministratorController.AdministratorLoginModel() {
            Password = "guar!tadminX12",
            User_Name = "guartadmin"
         } ;

         JsonResult<JObject> result = (JsonResult<JObject>) administratorController.Login (loginModel) ;
         Assert.IsTrue (MessageTool.IsSuccess (result.Content)) ;
         string token = MessageTool.GetToken (result.Content) ;

         ManagementServerController managementServerController = new ManagementServerController() ;
         JsonResult<JObject> managementServerAddResponse = (JsonResult<JObject>) managementServerController.Add (new ManagementServerController.ManagementServerAddModel() {
            Address = "192.168.1.82",
            Description = "Test",
            Email_Password = "test",
            Email_Provider = "Test",
            Email_User_Name = "Test",
            User_Name = "guartadmin",
            Password = "guar!tadminX12",
            Port = 8080,
            Name = "test",
            Token = token
         }) ;
         Assert.IsTrue (MessageTool.IsSuccess (managementServerAddResponse.Content)) ;

         WatcherServerController watcherServerController = new WatcherServerController() ;
         WatcherServerController.WatcherServerRegisterModel watcherServerRegisterModel = new WatcherServerController.WatcherServerRegisterModel() {
            Address = "192.168.1.82",
            Categories = new string[] {"teve"},
            Password = "guar!tadminX12",
            UserName = "guartadmin",
            Port = "9090",
            Name = "testWS",
            Token = token
         } ;
         JsonResult<JObject> watcherServerAddResponse = (JsonResult<JObject>) watcherServerController.Register (watcherServerRegisterModel) ;
         Assert.IsTrue (MessageTool.IsSuccess (watcherServerAddResponse.Content)) ;
      }
   }
}

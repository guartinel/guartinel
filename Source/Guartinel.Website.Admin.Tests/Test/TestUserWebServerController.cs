using System.Web.Http.Results;
using Guartinel.Website.Admin.Controllers ;
using Guartinel.Website.Common.Tools;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Guartinel.Website.Admin.Tests.Test {
   [TestFixture]
   public class TestUserWebServerController {

      [Test]
      public void TestRegister() {
         UserWebServerController administratorController = new UserWebServerController() ;
         UserWebServerController.UserWebServerRegisterModel registerModel = new UserWebServerController.UserWebServerRegisterModel() {
                  Password = "guartineladminX12",
                  User_Name = "guartadmin",
                  Name = "test",
                  User_Web_Server_Address = "188.213.174.153:80"
         } ;

         JsonResult<JObject> result = (JsonResult<JObject>) administratorController.Register (registerModel) ;
         Assert.IsTrue (MessageTool.IsSuccess (result.Content)) ;

         result = (JsonResult<JObject>) administratorController.G3tAvailable (new UserWebServerController.UserWebServerGetAvailableModel()) ;
         Assert.IsTrue (MessageTool.IsSuccess (result.Content)) ;

         result = (JsonResult<JObject>) administratorController.G3tStatus (new UserWebServerController.UserWebServerGetStatusModel()) ;
         Assert.IsTrue (MessageTool.IsSuccess (result.Content)) ;
      }
   }
}
using System.Web.Http.Results;
using Guartinel.Communication;
using Guartinel.Kernel.Utility;
using Guartinel.Website.Admin.Controllers;
using Guartinel.Website.Common.Error;
using Guartinel.Website.Common.Tools;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Guartinel.Website.Admin.Tests.Test {
   [TestFixture]
   public class TestAdministratorController {
      private const string DEFAULT_USER = "guartadmin" ;
      private const string DEFAULT_PASSWORD = "guar!tadminX12" ;

      [Test]
      [ExpectedException (typeof(CustomException.InvalidUserNameOrPasswordException))]
      public void TestLoginInvalidPassword() {
         AdministratorController administratorController = new AdministratorController() ;
         AdministratorController.AdministratorLoginModel loginModel = new AdministratorController.AdministratorLoginModel() {
                  Password = "INVALIDäđÄĐ~ˇł^äđdłäđĐPASSWORD123123123đĐ$[Đ[]Đ[]",
                  User_Name = "guartadmin"
         } ;
         var result = administratorController.Login (loginModel) ;
      }

      [Test]
      [ExpectedException (typeof(CustomException.InvalidUserNameOrPasswordException))]
      public void TestLoginInvalidUser() {
         AdministratorController administratorController = new AdministratorController() ;
         AdministratorController.AdministratorLoginModel loginModel = new AdministratorController.AdministratorLoginModel() {
                  Password = "guar!tadminX12",
                  User_Name = "guartasdadadmin"
         } ;
         var result = administratorController.Login (loginModel) ;
      }

      [Test]
      public void TestLogin() {
         AdministratorController administratorController = new AdministratorController() ;
         AdministratorController.AdministratorLoginModel loginModel = new AdministratorController.AdministratorLoginModel() {
                  Password = Hashing.GenerateHash (DEFAULT_PASSWORD, DEFAULT_USER),
                  User_Name = DEFAULT_USER
         } ;

         JsonResult<JObject> result = (JsonResult<JObject>) administratorController.Login (loginModel) ;
         Assert.IsTrue (MessageTool.IsSuccess (result.Content)) ;
      }

      [Test]
      public void TestG3tStatus() {
         AdministratorController administratorController = new AdministratorController() ;
         AdministratorController.AdministratorLoginModel loginModel = new AdministratorController.AdministratorLoginModel() {
                  Password = Hashing.GenerateHash (DEFAULT_PASSWORD, DEFAULT_USER),
                  User_Name = DEFAULT_USER
         } ;

         JsonResult<JObject> loginResult = (JsonResult<JObject>) administratorController.Login (loginModel) ;
         Assert.IsTrue (MessageTool.IsSuccess (loginResult.Content)) ;
         string token = MessageTool.GetToken (loginResult.Content) ;
         Assert.IsNotNull (token) ;

         AdministratorController.AdminidstratorGetStatusModel getStatusModel = new AdministratorController.AdminidstratorGetStatusModel() {
                  Token = token
         } ;
         JsonResult<JObject> getStatusResult = (JsonResult<JObject>) administratorController.G3tStatus (getStatusModel) ;
         Assert.IsTrue (MessageTool.IsSuccess (getStatusResult.Content)) ;
         Assert.NotNull (MessageTool.SafeGetValue (getStatusResult.Content, AdminWebsiteAPI.Administrator.GetStatus.Response.CONFIGURED)) ;
      }
   }
}
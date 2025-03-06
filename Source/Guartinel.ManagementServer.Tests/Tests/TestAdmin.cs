using System ;
using NUnit.Framework ;

namespace Guartinel.ManagementServer.Tests.Tests {
   [TestFixture]
   internal class TestAdmin2 {
      //private string _userName;
      //private string _password;
      //private string _token;

      /*
      [Test]
      public void Test0Login() {
         Response response = MakeRequest("http://localhost:8080/admin/login", new {
            user_name = "asdasdasd",
            password = "aasdasdasd",
         });
         Assert.NotNull(response.content);
         Assert.AreEqual(ConnectionVars.Content.INVALID_USERNAME, response.content);

         response = MakeRequest("http://localhost:8080/admin/login", new {
            user_name = "admin",
            password = "aaaaaaaaaaasdasdasd"
         });
         Assert.NotNull(response.content);
         Assert.AreEqual(ConnectionVars.Content.INVALID_PASSWORD, response.content);

         response = MakeRequest("http://localhost:8080/admin/login", new {
            user_name = "admin",
            password = "admin"
         });

         Assert.NotNull(response.content);
         Assert.AreEqual("SUCCESS", response.content);
         Assert.NotNull(response.token);
         _token = response.token;
         }

      [Test]
      public void Test1Update() {
         Response response = MakeRequest("http://localhost:8080/admin/update", new {
            user_name = "asdasdasd",
            password = "aasdasdasd",
            token = "asdasdasd",
            web_page_url = "asd"
         });
         Assert.NotNull(response.content);
         Assert.AreEqual(ConnectionVars.Content.INVALID_TOKEN, response.content);

         response = MakeRequest("http://localhost:8080/admin/update", new {
            user_name = "admin2",
            password = "admin2",
            token = _token,
            web_page_url = "asd"
         });
         Assert.NotNull(response.content);
         Assert.AreEqual("SUCCESS", response.content);

         }
      [Test]
      public void Test2Logout() {
         Response response = MakeRequest("http://localhost:8080/admin/logout", new {
            token = "asdasdasd",
         });
         Assert.NotNull(response.content);
         Assert.AreEqual(ConnectionVars.Content.INVALID_TOKEN, response.content);

         response = MakeRequest("http://localhost:8080/admin/logout", new {
            token = _token,
         });
         Assert.NotNull(response.content);
         Assert.AreEqual("SUCCESS", response.content);


         response = MakeRequest("http://localhost:8080/admin/login", new {
            user_name = "admin2",
            password = "admin2",
            token = _token,
         });
         Assert.NotNull(response.content);
         Assert.AreEqual("SUCCESS", response.content);
         Assert.NotNull(response.token);
         _token = response.token;
         }

      [Test]
      public void Test3GetInfo() {
         Response response = MakeRequest("http://localhost:8080/admin/status/getinfo", new {
            token = "asdasdasd",
         });
         Assert.NotNull(response.content);
         Assert.AreEqual(ConnectionVars.Content.INVALID_TOKEN, response.content);

         response = MakeRequest("http://localhost:8080/admin/status/getinfo", new {
            token = _token,
         });

         Assert.NotNull(response.content);
         Assert.AreEqual("SUCCESS", response.content);
         Assert.NotNull(response.info);
         }
*/
   }
}

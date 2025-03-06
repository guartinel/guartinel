using Guartinel.Kernel.Utility ;
using Guartinel.ManagementServer.Tests.Helpers.Admin ;
using NUnit.Framework;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;


namespace Guartinel.ManagementServer.Tests.Tests.TestAdmin {
   internal class TestAdminWatcherServer {
      private readonly AdminImpl _admin = new AdminImpl() ;

      [SetUp]
      public void SetUp() {
         _admin.Password = Hashing.GenerateHash ("guar!tadminX12", "guartadmin") ;
         _admin.UserName = "guartadmin" ;
         Response response = AdminHelper.login (_admin.UserName, _admin.Password) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.token) ;
         _admin.Token = response.token ;
      }

      [Test]
      public void TestLogin() {
         Response response = AdminHelper.login ("invalid_username", "admin") ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = AdminHelper.login ("guartadmin", "invalid password") ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = AdminHelper.login ("guartadmin", Hashing.GenerateHash ("guar!tadminX12", "guartadmin")) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.token) ;
      }

      [Test]
      public void TestWatcherServer() {
         WatcherServerImpl watcherServer = new WatcherServerImpl() ;
         watcherServer.Name = "test" ;
         watcherServer.Port = "80" ;
         watcherServer.Address = "192.168.1.179" ;

         Response response = AdminHelper.registerWatcherServer (_admin.Token, watcherServer.Name, watcherServer.Address, watcherServer.Port, _admin.UserName, _admin.Password) ;
         Assert.AreEqual ("SUCCESS", response.success) ;

         response = AdminHelper.getWatcherServers (_admin.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         //   Assert.AreEqual(1, response.servers.Length);
         watcherServer.Id = response.servers [0].id ;

         response = AdminHelper.getStatusFromWatcherServer (_admin.Token, watcherServer.Id) ;
         Assert.NotNull (response.status) ;

         response = AdminHelper.getEventsFromWatcherServer (_admin.Token, watcherServer.Id) ;
         Assert.NotNull (response.events) ;

         response = AdminHelper.removeWatcherServer (_admin.Token, watcherServer.Id) ;
         Assert.AreEqual ("SUCCESS", response.success) ;

         response = AdminHelper.getWatcherServers (_admin.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.AreEqual (0, response.servers.Length) ;
      }

      /*     

        private string _serverId;
        [Test]
        public void Test1RegisterWatcherServer() {
            Response response = MakeRequest(MANAGEMENT_SERVER_HOST +"/admin/watcherServer/register", new {
                token = "asdasdasd",
                name = "Test",
                address = "192.168.1.82",
                port = "8081",
                user_name = "newAdmin",
                password = "newPassword",
                });
            Assert.NotNull(response.content);
            Assert.AreEqual(ConnectionVars.Content.INVALID_TOKEN, response.content);

            response = MakeRequest(MANAGEMENT_SERVER_HOST + "/admin/watcherServer/register", new {
                token = _token,
                name = "Test",
                address = "0.168.1.82",
                port = "8081",
                user_name = "newAdmin",
                password = "newPassword",

                });
            Assert.NotNull(response.content);
            Assert.AreEqual(ConnectionVars.Content.INTERNAL_SYSTEM_ERROR, response.content);

            response = MakeRequest(MANAGEMENT_SERVER_HOST + "/admin/watcherServer/register", new {
                token = _token,
                name = "Test",
                address = "192.168.1.80",
                port = "8081",
                user_name = "newAdmin",
                password = "newPassword",

                });

            Assert.NotNull(response.content);
            Assert.AreEqual("SUCCESS", response.content);

            }

        [Test]
        public void Test2GetAvailableWatcherServer() {
            Response response = MakeRequest(MANAGEMENT_SERVER_HOST + "/admin/watcherServer/getavailable", new {
                token = _token,
                });
            Assert.NotNull(response.content);
            Assert.AreEqual("SUCCESS", response.content);
            Assert.NotNull(response.servers);
            Assert.NotNull(response.servers[0]);
            Response.Server server = response.servers[0];
            _serverId = server.id;
            }


        [Test]
        public void Test3GetEventsWatcherServer() {
            Response response = MakeRequest(MANAGEMENT_SERVER_HOST + "/admin/watcherServer/getEvents", new {
                token = _token,
                watcher_server_id = _serverId,
                });
            Assert.NotNull(response.content);
            Assert.AreEqual("SUCCESS", response.content);
            Assert.NotNull(response.events);

            }

        [Test]
        public void Test4GetInfoWatcherServer() {
            Response response = MakeRequest(MANAGEMENT_SERVER_HOST + "/admin/watcherServer/getInfo", new {
                token = _token,
                watcher_server_id = _serverId,
                });
            Assert.NotNull(response.content);
            Assert.AreEqual("SUCCESS", response.content);
            Assert.NotNull(response.events);

            }


        [Test]
        public void Test25emoveWatcherServer() {
            Response response = MakeRequest(MANAGEMENT_SERVER_HOST + "/admin/watcherServer/remove", new {
                token = _token,
                watcher_server_id = _serverId,
                });
            Assert.NotNull(response.content);
            Assert.AreEqual("SUCCESS", response.content);
            Assert.NotNull(response.events);

            }*/
   }
}

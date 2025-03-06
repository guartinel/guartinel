using Guartinel.Website.User.Controllers;
using Guartinel.Website.User.Models.Account;
using NUnit.Framework;

namespace Guartinel.Website.User.Tests.Test {
    [TestFixture]
    public class TestAccountController {
        [Test]
        public void TestLogin() {
            AccountController controller = new AccountController();
            var result = controller.Login(new AccountLoginModel() {
                Email = "testmail",
                Password = "testpassword"
                });

            }
        }
    }

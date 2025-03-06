using System ;
using Guartinel.Communication ;
using Guartinel.ManagementServer.Tests.Helpers.Account ;
using NUnit.Framework ;
using Helper = Guartinel.ManagementServer.Tests.Helpers.Helper ;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.ManagementServer.Tests.Tests {
   [TestFixture]
   public class TestAccount {
      private AccountImpl _account ;

      [SetUp]
      public void SetUp() {
         _account = new AccountImpl() ;
         _account.Email = Helper.getTestName() ;
         _account.Password = "test" ;

         Response createResponse = AccountHelper.createAccount (_account.Email, _account.Password) ;
         Assert.AreEqual (ManagementServerAPI.GeneralResponse.SuccessValues.SUCCESS, createResponse.success) ;

         Response loginResponse = AccountHelper.loginAccount (_account.Email, _account.Password) ;
         Assert.NotNull (loginResponse.token) ;
         _account.Token = loginResponse.token ;

         Response getStatussResponse = AccountHelper.getStatus (_account.Token) ;
         Assert.NotNull (getStatussResponse.account.id) ;
         _account.Id = getStatussResponse.account.id ;
         _account.ActivationCode = getStatussResponse.account.activation_code ;
         _account.IsActivated = getStatussResponse.account.is_activated ;
      }

      [TearDown]
      public void TearDown() {
         Response deleteResponse = AccountHelper.deleteAccount (_account.Id, _account.Email, _account.Password, _account.Token) ;
         Assert.AreEqual ("SUCCESS", deleteResponse.success) ;
         _account = null ;
      }

      [Test]
      public void TestLogin() {
         Response response = AccountHelper.loginAccount ("invalid_email", _account.Password) ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = AccountHelper.loginAccount (_account.Email, "invalid password") ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = AccountHelper.loginAccount (_account.Email, _account.Password) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;
         Assert.NotNull (response.token) ;
         _account.Token = response.token ;
      }

      [Test]
      public void TestInfo() {
         Response response = AccountHelper.getStatus ("invalid token") ;
         Assert.AreEqual (AllErrorValues.INVALID_TOKEN, response.error) ;

         response = AccountHelper.getStatus (_account.Token) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, response.success) ;
         Assert.NotNull (response.account) ;
         Assert.NotNull (response.account.id) ;

         _account.Id = response.account.id ;
         _account.ActivationCode = response.account.activation_code ;
      }

      [Test]
      public void TestUpdate() {
         Response response = AccountHelper.updateAccount ("invalid_token", _account.Id, _account.Password, _account.Email) ;
         Assert.AreEqual (AllErrorValues.INVALID_TOKEN, response.error) ;

         response = AccountHelper.updateAccount (_account.Token, _account.Id, "invalid password", _account.Email) ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         response = AccountHelper.updateAccount (_account.Token, _account.Id, _account.Password, "invalid_email") ;
         Assert.AreEqual (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD, response.error) ;

         string newFirstName = Helper.getRandomString() ;
         string newLastName = Helper.getRandomString() ;

         //lets change last and first name
         response = AccountHelper.updateAccount (_account.Token, _account.Id, _account.Password, _account.Email, newFirstName, newLastName) ;
         Assert.AreEqual ("SUCCESS", response.success) ;

         response = AccountHelper.getStatus (_account.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.account) ;
         Assert.AreEqual (newLastName, response.account.last_name) ;
         Assert.AreEqual (newFirstName, response.account.first_name) ;
         _account.LastName = response.account.last_name ;
         _account.FirstMame = response.account.first_name ;

         //lets change mail
         string newMail = Helper.getTestName() ;
         response = AccountHelper.updateAccount (_account.Token, _account.Id, _account.Password, _account.Email, newFirstName, newLastName, _account.Password, newMail) ;
         Assert.AreEqual ("SUCCESS", response.success) ;

         response = AccountHelper.getStatus (_account.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.account) ;
         Assert.AreEqual (newLastName, response.account.last_name) ;
         Assert.AreEqual (newFirstName, response.account.first_name) ;
         Assert.AreEqual (newMail, response.account.email) ;
         _account.LastName = response.account.last_name ;
         _account.FirstMame = response.account.first_name ;
         _account.Email = response.account.email ;

         //lets change password

         string newPassword = Helper.getRandomString() ;
         response = AccountHelper.updateAccount (_account.Token, _account.Id, _account.Password, _account.Email, newFirstName, newLastName, newPassword) ;
         Assert.AreEqual ("SUCCESS", response.success) ;

         response = AccountHelper.getStatus (_account.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.account) ;
         Assert.AreEqual (newLastName, response.account.last_name) ;
         Assert.AreEqual (newFirstName, response.account.first_name) ;
         Assert.AreEqual (newMail, response.account.email) ;
         _account.LastName = response.account.last_name ;
         _account.FirstMame = response.account.first_name ;
         _account.Email = response.account.email ;
         _account.Password = newPassword ;

         //try to login with new password
         response = AccountHelper.loginAccount (_account.Email, _account.Password) ;
         Assert.AreEqual ("SUCCESS", response.success) ;
         Assert.NotNull (response.token) ;
         _account.Token = response.token ;
      }

      [Test]
      public void TestLogout() {
         //false try
         Response response = AccountHelper.logoutAccount ("invalid_token") ;
         Assert.AreEqual (AllErrorValues.INVALID_TOKEN, response.error) ;

         //try if token still valid
         response = AccountHelper.getStatus (_account.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;

         //logout now with valid token
         response = AccountHelper.logoutAccount (_account.Token) ;
         Assert.AreEqual ("SUCCESS", response.success) ;

         //try with old token , it should fail
         response = AccountHelper.getStatus (_account.Token) ;
         Assert.AreEqual (AllErrorValues.INVALID_TOKEN, response.error) ;
      }

      [Test]
      public void TestActivation() {
         /*   Response response = AccountHelper.activate("invalid_activation_code"); // NO POINT TO TEST because activation code sent in email to the mail adress.
            Assert.AreEqual("INVALID_ACTIVATION_CODE", response.error); 

            response = AccountHelper.activate(_account.ActivationCode);
            Assert.AreEqual("SUCCESS", response.success);

            response = AccountHelper.getStatus(_account.Token);
            Assert.AreEqual("SUCCESS", response.success);
            Assert.AreEqual("True", response.account.is_activated);*/
      }
   }
}

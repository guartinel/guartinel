using System ;
using System.Text ;

namespace Guartinel.ManagementServer.Tests.Helpers.Account {
   internal class AccountHelper {
      public static Response createAccount (string email,
            string password) {
         return Connection.Connector.MakeRequest ("/account/create", new {
            email = email,
            password = password
         }) ;
      }

      public static Response deleteAccount (string accountId,
            string email,
            string password,
            string token) {
         return Connection.Connector.MakeRequest ("/account/delete", new {
            email = email,
            password = password,
            id = accountId,
            token = token
         }) ;
      }

      public static Response updateAccount (string token,
            string id,
            string password,
            string email,
            string firstName = null,
            string lastName = null,
            string passwordNew = null,
            string emailNew = null) {
         return Connection.Connector.MakeRequest ("/account/update", new {
            token = token,
            id = id,
            email = email,
            new_email = emailNew,
            password = password,
            new_password = passwordNew,
            first_name = firstName,
            last_name = lastName,
         }) ;
      }

      public static Response loginAccount (string email,
            string password) {
         return Connection.Connector.MakeRequest ("/account/login", new {
            email = email,
            password = password
         }) ;
      }

      public static Response logoutAccount (string token) {
         return Connection.Connector.MakeRequest ("/account/logout", new {
            token = token
         }) ;
      }

      public static Response getStatus (string token) {
         return Connection.Connector.MakeRequest ("/account/getStatus", new {
            token = token
         }) ;
      }

      public static Response activate (string activationCode) {
         return Connection.Connector.MakeRequest ("/account/activateAccount", new {
            activation_code = activationCode
         }) ;
      }
   }
}

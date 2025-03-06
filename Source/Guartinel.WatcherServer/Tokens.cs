using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.WatcherServer {
   public class Token {
      public Token() : this (
         // Use configuration settings
         DateTime.UtcNow.Add (new TimeSpan (0, 0, 0, ApplicationSettings.Use.TokenExpirySeconds))) {
      }

      public Token (DateTime expiry) {
         Key = Guid.NewGuid().ToString() ;
         Expiry = expiry ;
      }

      public string Key {get ;}
      public DateTime Expiry {get ;}
   }

   public class Tokens {
      private static readonly object _singletonLock = new object();
      private static Tokens _singleton ;
      
      public static Tokens Use() {
         lock (_singletonLock) {
            if (_singleton == null) {
               _singleton = new Tokens();
            }            
         }

         return _singleton ;
      }

      protected Dictionary<string, Token> _tokens = new Dictionary<string, Token>(); 

      public string GenerateToken() {
         var result = new Token() ;
         lock (_tokens) {
            _tokens.Add(result.Key, result);
         }
         
         return result.Key ;
      }

      public void CheckToken (string token) {
         lock (_tokens) {
            if (!_tokens.ContainsKey (token)) throw new InvalidTokenException() ;
            if (_tokens [token].Expiry < DateTime.UtcNow) throw new ExpiredTokenException() ;
         }
      }
   }

   public class InvalidTokenException : ServerException {
      public InvalidTokenException() : base (AllErrorValues.INVALID_TOKEN, "Invalid token.") {}
   }

   public class ExpiredTokenException : ServerException {
      public ExpiredTokenException() : base (AllErrorValues.TOKEN_EXPIRED, "Expired token.") {}
   }

   public class LoginException : ServerException {
      public LoginException() : base (AllErrorValues.INVALID_SERVER_LOGIN, "Invalid server login.") {}
   }
}
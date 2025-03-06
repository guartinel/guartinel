using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guartinel.Core.Logging ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility {
   public class CheckResult {
      public static class Constants {
         public const string SUCCESS = "success" ;
         public const string MESSAGE = "message" ;
         public const string DATA = "data" ;

         public static class Success {
            public const string SUCCESS_VALUE = "success" ;
            public const string ERROR_VALUE = "error" ;
         }
      }

      public CheckResult (bool success,
                          string message,
                          JObject data) {
         Success = success ;
         Message = message ;
         Data = data ;
      }

      private readonly JObject _checkResult = new JObject() ;

      public bool Success {
         get {return (_checkResult [Constants.SUCCESS]?.ToString().Equals (Constants.Success.SUCCESS_VALUE)).GetValueOrDefault (false) ;}
         private set {_checkResult [Constants.SUCCESS] = value ? Constants.Success.SUCCESS_VALUE : Constants.Success.ERROR_VALUE ;}
      }

      public string Message {
         get {return _checkResult [Constants.MESSAGE]?.ToString() ;}
         private set {_checkResult [Constants.MESSAGE] = value ;}
      }

      public JObject Data {
         get {return _checkResult [Constants.DATA] as JObject ;}
         private set {_checkResult [Constants.DATA] = value ?? new JObject();}
      }

      public override string ToString() {
         return _checkResult.ToString() ;
      }

      public JObject ToJObject() {
         return _checkResult.DeepClone() as JObject ;
      }

      public class InvalidParameters : CheckResult {
         public InvalidParameters() : this ("Invalid parameters.") {}
         public InvalidParameters (string message) : base (false, message, null) {}
      }
   }
}
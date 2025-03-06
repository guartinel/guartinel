using System;
using System.Linq;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI {
   public class CheckResult {
      public static class Constants {
         public const string SUCCESS = "success" ;
         public const string MESSAGE = "message" ;
         public const string DETAILS = "details" ;
         public const string EXTRACT = "extract" ;
         public const string DATA = "data" ;

         public static class Success {
            public const string SUCCESS_VALUE = "success" ;
            public const string ERROR_VALUE = "error" ;
         }
      }

      public CheckResult (bool success,
                          string message,
                          string details,
                          string extract,
                          JObject data) {
         Success = success ;
         Message = message ;
         Details = details ;
         Extract = extract ;
         Data = data ;
      }

      private readonly JObject _checkResult = new JObject() ;

      public bool Success {
         get => _checkResult.GetStringValue(Constants.SUCCESS, Constants.Success.ERROR_VALUE) == Constants.Success.SUCCESS_VALUE ;
         private set => _checkResult [Constants.SUCCESS] = value ? Constants.Success.SUCCESS_VALUE : Constants.Success.ERROR_VALUE ;
      }

      public string Message {
         get => _checkResult.GetStringValue (Constants.MESSAGE) ;
         private set => _checkResult [Constants.MESSAGE] = value ;
      }

      public string Details {
         get => _checkResult.GetStringValue (Constants.DETAILS) ;
         private set => _checkResult [Constants.DETAILS] = value ;
      }

      public string Extract {
         get => _checkResult.GetStringValue (Constants.EXTRACT) ;
         private set => _checkResult [Constants.EXTRACT] = value ;
      }

      public JObject Data {
         get => _checkResult [Constants.DATA] as JObject ;
         private set => _checkResult [Constants.DATA] = value ?? new JObject() ;
      }

      public string Name { get; set; }

      public override string ToString() {
         return $"{Success} Message: {Message} Data: {Data.ToString (Formatting.None)}" ;
      }

      public JObject AsJObject() {
         return _checkResult.DeepClone() as JObject ;
      }

      public class InvalidParameters : CheckResult {
         public InvalidParameters() : this ("Invalid parameters.") {}
         public InvalidParameters (string message) : base (false,
                                                           message,
                                                           "Invalid parameters.",
                                                           "Invalid parameters.",
                                                           null) {}
      }
   }
}
using System ;
using System.Text ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service {
   public class ServiceResult {
      protected ServiceResult (bool success,
                               JObject result,
                               string message,
                               string details) {
         Success = success ;
         Result = result ;
         Message = message ;
         Details = details ;
      }

      public ServiceResult() { }

      public ServiceResult (JObject source) {
         FromJObject (source) ;
      }

      public static ServiceResult CreateSuccess (JObject result) {
         return new ServiceResult (true, result, null, null) ;
      }

      public static ServiceResult CreateSuccess (string message) {
         return new ServiceResult (true, null, message, null) ;
      }

      public static ServiceResult CreateError (string message,
                                               string details) {
         return new ServiceResult (false, null, message, details) ;
      }

      public bool Success {get ; protected set ;}
      
      public JObject Result {get ; protected set ;}

      public string Message {get ; protected set ;}

      public string Details {get ; protected set ;}

      public JObject AsJObject() {
         JObject result = new JObject() ;

         result [nameof(Success).NameToJSONName()] = Success ;

         if (Result != null) {
            result [nameof(Result).NameToJSONName()] = Result ;
         }

         if (!string.IsNullOrEmpty (Message)) {
            result [nameof(Message).NameToJSONName()] = Message ;
         }

         if (!string.IsNullOrEmpty (Details)) {
            result [nameof(Details).NameToJSONName()] = Details ;
         }

         return result ;
      }

      public void FromJObject (JObject source) {
         Success = source.GetBoolValue (nameof(Success).NameToJSONName()) ;
         Result = (JObject) source [nameof(Result).NameToJSONName()] ;
         Message = source.GetStringValue (nameof(Message).NameToJSONName()) ;
         Details = source.GetStringValue (nameof(Details).NameToJSONName()) ;
      }
   }
}
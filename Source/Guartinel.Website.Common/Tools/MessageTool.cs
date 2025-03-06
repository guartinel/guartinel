using System;
using Guartinel.Communication;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.Website.Common.Tools {
   public static class MessageTool {
      public static bool IsSuccess (JObject jobject) {
         string success = null ;
         try {
            success = SafeGetValue (jobject, AllParameters.SUCCESS) ;
         } catch (Exception e) {
            Logger.Error ($"Cannot get success from jobject {e.GetAllMessages()}") ;
            return false ;
         }
         return success.Equals (ManagementServerAPI.GeneralResponse.SuccessValues.SUCCESS) ;
      }

      public static string GetError (JObject jobject) {
         return SafeGetValue (jobject, AllParameters.ERROR) ;
      }

      public static string GetToken (JObject jobject) {
         return SafeGetValue (jobject, AllParameters.TOKEN) ;
      }

      public static JObject CreateJObjectWithSuccess() {
         JObject jobject = new JObject() ;
         jobject.Add (AllParameters.SUCCESS, AllSuccessValues.SUCCESS) ;
         return jobject ;
      }

      public static JObject CreateJObjectWithError (string error) {
         string errorUUID = Guid.NewGuid().ToString() ;
         JObject jobject = new JObject() ;
         jobject.Add (AllParameters.ERROR, error) ;
         jobject.Add (AllParameters.ERROR_UUID, errorUUID) ;
         Logger.Error ($"ErrorObject created. Error  {error} uuid: {errorUUID}") ;
         return jobject ;
      }

      public static JObject CreateJObjectWithError (string error, string logMessage) {
         string errorUUID = Guid.NewGuid().ToString() ;
         JObject jobject = new JObject() ;
         jobject.Add (AllParameters.ERROR, error) ;
         jobject.Add (AllParameters.ERROR_UUID, errorUUID) ;
         Logger.Error ($"ErrorObject created. Logmessage {logMessage} Error  {error} uuid: {errorUUID}") ;

         return jobject ;
      }

      public static JObject CreateInternalSystemErrorJObject (string error) {
         string errorUUID = Guid.NewGuid().ToString() ;
         JObject jobject = new JObject() ;
         jobject.Add (AllParameters.ERROR, AllErrorValues.INTERNAL_SYSTEM_ERROR) ;
         jobject.Add (AllParameters.ERROR_UUID, errorUUID) ;
         Logger.Error ($"ErrorObject created. Error  {error} uuid: {errorUUID}") ;

         return jobject ;
      }

      public static string SafeGetValue (JObject jobject, string key) {
         string value = String.Empty ;
         if (string.IsNullOrEmpty (key) || jobject.Count == 0) {
            return value ;
         }

         try {
            value = jobject.GetValue (key).Value<string>() ;
         } catch (Exception e) {
            Logger.Error ($"Cannot get succes from JObject {e.GetAllMessages()}") ;

            return value ;
         }
         return value ;
      }
   }
}
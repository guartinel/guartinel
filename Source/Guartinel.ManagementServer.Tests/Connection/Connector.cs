using System ;
using System.IO ;
using System.Net ;
using System.Text ;
using System.Web.Script.Serialization ;
using Newtonsoft.Json ;

namespace Guartinel.ManagementServer.Tests.Connection {
   internal class Connector {
      protected static String MANAGEMENT_SERVER_HOST = "https://backend2.guartinel.com:9091";

      public static Response MakeRequest (string endPointURL,
            object parameters) {
         var httpWebRequest = (System.Net.HttpWebRequest) WebRequest.Create (MANAGEMENT_SERVER_HOST + endPointURL) ;
         httpWebRequest.ContentType = "application/json" ;
         httpWebRequest.Method = "POST" ;

         using (var streamWriter = new StreamWriter (httpWebRequest.GetRequestStream())) {
            string json = JsonConvert.SerializeObject (parameters) ; //new JavaScriptSerializer().Serialize(parameters);

            streamWriter.Write (json) ;
         }
         WebResponse httpResponse = null ;
         try {
            httpResponse = (HttpWebResponse) httpWebRequest.GetResponse() ;
         } catch (WebException e) {
            httpResponse = e.Response ;
         }
         using (var streamReader = new StreamReader (httpResponse.GetResponseStream())) {
            var result = streamReader.ReadToEnd() ;
            return new JavaScriptSerializer().Deserialize<Response> (result) ;
         }
      }
   }
}

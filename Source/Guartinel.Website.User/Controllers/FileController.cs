using System ;
using System.IO ;
using System.Linq ;
using System.Net ;
using System.Net.Http ;
using System.Web ;
using System.Web.Http ;
using Guartinel.Communication ;

namespace Guartinel.Website.User.Controllers {
   [RoutePrefix (UserWebsiteAPI.File.URL)]
   public class FileController : ApiController {
   /*   [Route (UserWebsiteAPI.File.WindowsAgent.URL_PART)]
      [HttpGet]
      public HttpResponseMessage WindowsAgent() {
         HttpResponseMessage result = null ;
         string localFilePath = HttpContext.Current.Server.MapPath ("~/Content/Files/WindowsAgent.zip") ;

         if (!File.Exists (localFilePath)) result = Request.CreateResponse (HttpStatusCode.Gone) ;
         else {
            // serve the file to the client
            result = Request.CreateResponse (HttpStatusCode.OK) ;
            result.Content = new StreamContent (new FileStream (localFilePath, FileMode.Open, FileAccess.Read)) ;
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue ("attachment") ;
            result.Content.Headers.ContentDisposition.FileName = "WindowsAgent.zip" ;

            // FileName dont work, had to add this line
            result.Content.Headers.Add ("x-filename", "WindowsAgent.zip") ;
         }

         return result ;
      }

      [Route (UserWebsiteAPI.File.LinuxAgent.URL_PART)]
      [HttpGet]
      public HttpResponseMessage LinuxAgent() {
         HttpResponseMessage result = null ;
         string localFilePath = HttpContext.Current.Server.MapPath ("~/Content/Files/LinuxAgent.zip") ;

         if (!File.Exists (localFilePath)) result = Request.CreateResponse (HttpStatusCode.Gone) ;
         else {
            // serve the file to the client
            result = Request.CreateResponse (HttpStatusCode.OK) ;
            result.Content = new StreamContent (new FileStream (localFilePath, FileMode.Open, FileAccess.Read)) ;
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue ("attachment") ;
            result.Content.Headers.ContentDisposition.FileName = "LinuxAgent.zip" ;

            // FileName dont work, had to add this line
            result.Content.Headers.Add ("x-filename", "LinuxAgent.zip") ;
         }
         return result ;
      }*/

      [Route ("Commons")]
      [HttpGet]
      public HttpResponseMessage Constants() {
         HttpResponseMessage result = null ;
         string localFilePath = HttpContext.Current.Server.MapPath ("~/Content/Files/commons.json") ;

         if (!File.Exists (localFilePath)) result = Request.CreateResponse (HttpStatusCode.Gone) ;
         else {
            // serve the file to the client
            result = Request.CreateResponse (HttpStatusCode.OK) ;
            result.Content = new StreamContent (new FileStream (localFilePath, FileMode.Open, FileAccess.Read)) ;
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue ("attachment") ;
            result.Content.Headers.ContentDisposition.FileName = "commons.json" ;

            // FileName dont work, had to add this line
            result.Content.Headers.Add ("x-filename", "commons.json") ;
         }
         return result ;
      }
   }
}

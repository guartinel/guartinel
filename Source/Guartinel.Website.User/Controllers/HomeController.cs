using Guartinel.Communication;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Guartinel.Website.User.Controllers
{
   [RoutePrefix("Home")]
   public class HomeController : Controller
   {
      [Route("Index")]
      public ActionResult Index()
      {
         Response.Cache.SetCacheability(HttpCacheability.NoCache);
         return View();
      }

      [Route("GetVersion")]
      public ActionResult GetVersion()
      {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.GetVersion.FULL_URL, false, true);
         return Json(result);
      }
   }
}

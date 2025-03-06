using System ;
using System.Linq ;
using System.Text ;
using System.Web ;
using System.Web.Mvc ;

namespace Guartinel.Website.Admin.Controllers {

   public class HomeController : Controller {
      public ActionResult Index() {
         Response.Cache.SetCacheability (HttpCacheability.NoCache) ;
         return View() ;
      }
   }
}
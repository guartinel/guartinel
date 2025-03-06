using Microsoft.AspNetCore.Mvc ;

namespace Guartinel.Service.Configuration.Controllers {
   public class GetModel {
      public string Key {get ; set ;}
      public string Token {get ; set ;}
   }

   public class HashModel : GetModel { } ;

   [Route ("get")]
   [ApiController]
   public class GetController : Controller {
      private RequestManager _requestManager ;

      public GetController (RequestManager requestManager) {
         _requestManager = requestManager ;
      }

      [HttpPost ("value")]
      public IActionResult Get (GetModel model) {
         return _requestManager.Get (model.Key, model.Token) ;
      }

      [HttpPost ("hash")]
      public IActionResult Hash (HashModel model) {
         return _requestManager.GetHash (model.Key, model.Token) ;
      }
   }
}

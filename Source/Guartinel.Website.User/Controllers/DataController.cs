using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sysment.Watcher.Website.Models ;

namespace Sysment.Watcher.Website.Controllers
{
    public class DataController : Controller
    {
        // GET: /Data/

       public JsonResult UserLogin (LoginData loginData) {

          using (MyDatabaseEntities myDatabaseEntities = new MyDatabaseEntities()) {
             var user = myDatabaseEntities.Users.Where(a => a.Email.Equals (loginData.Email) &&
                                                                 a.Password.Equals (loginData.Password)).FirstOrDefault() ;
             return new JsonResult {Data = user, JsonRequestBehavior = JsonRequestBehavior.AllowGet} ;
          }
       }
    }
}
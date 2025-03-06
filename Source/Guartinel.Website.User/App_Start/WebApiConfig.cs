using System.Net.Http.Headers ;
using System.Web.Http ;
using Guartinel.Website.User.Attributes ;

namespace Guartinel.Website.User {
   public class WebApiConfig {
      public static void Register (HttpConfiguration config) {
         // TODO: Add any additional configuration code.

         // Web API routes
         config.MapHttpAttributeRoutes() ;

         config.Routes.MapHttpRoute (
                                     name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new {id = RouteParameter.Optional}
               ) ;

         // Add this model state validator to every API call
         config.Filters.Add (new ValidateModelStateAttribute()) ;
         config.Filters.Add (new HandleExceptionAttribute()) ;

         // support for json!
         config.Formatters.JsonFormatter.SupportedMediaTypes.Add (new MediaTypeHeaderValue ("text/html")) ;

         // WebAPI when dealing with JSON & JavaScript!
         // Setup json serialization to serialize classes to camel (std. Json format)

         /*var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
         formatter.SerializerSettings.ContractResolver =
             new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

         }*/

         /*var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First() ;
         jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver() ;*/
      }
   }
}

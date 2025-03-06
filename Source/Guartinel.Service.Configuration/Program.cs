using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Net ;
using System.Threading.Tasks ;
using Microsoft.AspNetCore ;
using Microsoft.AspNetCore.Hosting ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.Logging ;

namespace Guartinel.Service.Configuration {
   public class Program {
      public static void Main (string[] args) {
         IWebHostBuilder host = new WebHostBuilder()
                  .UseKestrel (options => {
                     options.Listen (IPAddress.Any, 5000) ;
                    
                  }).ConfigureAppConfiguration ((context,
                                                 builder) => {
                     builder.AddJsonFile ("appsettings.json",
                                          optional: true, reloadOnChange: true) ;
                  })
                  .ConfigureLogging ((hostingContext,
                                      logging) => {
                     logging.AddConsole() ;
                     logging.AddDebug() ;
                  })
                  .UseStartup<Startup>() ;
        
         host.Build().Run() ;
      }
   }
}

using System ;
using Guartinel.Kernel.Logging ;
using Microsoft.AspNetCore.Builder ;
using Microsoft.AspNetCore.Hosting ;
using Microsoft.AspNetCore.Mvc ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.DependencyInjection ;
using Newtonsoft.Json ;

//using Guartinel.Kernel;
namespace Guartinel.Service.Configuration {
   public class Startup {
      public class ServiceConfiguration {
         public string DataPath {get ; set ;}
      }

      private RequestManager _requestManager ;

      public Startup (IConfiguration configuration) {
         Guartinel.Kernel.Logging.Logger.RegisterLogger<SimpleConsoleLogger>() ;
         Logger.Info (configuration.ToString()) ;
         Logger.Info ("Starting Guartinel.Service.Configuration! 1.0") ;
         Configuration = new ServiceConfiguration {DataPath = configuration.GetValue<string> ("DataPath")} ;
         _requestManager = new RequestManager (new PersistanceManager (@"/Data")) ;
      }

      public ServiceConfiguration Configuration {get ; set ;}

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices (IServiceCollection services) {
         services.AddMvc().SetCompatibilityVersion (CompatibilityVersion.Version_2_1).AddJsonOptions (options => options.SerializerSettings.Formatting = Formatting.None) ;
         services.AddSingleton (typeof(RequestManager), _requestManager) ;
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure (IApplicationBuilder app,
                             IHostingEnvironment env) {
         if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage() ;
         } else {
            app.UseHsts() ;
         }

         app.UseMvc() ;
      }
   }
}

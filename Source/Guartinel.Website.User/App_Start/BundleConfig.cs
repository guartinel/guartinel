using System.Web.Optimization;
using WebGrease.Css.Extensions;

namespace Guartinel.Website.User
{
   public class BundleConfig
   {
      // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
      public static void RegisterBundles(BundleCollection bundles)
      {

         // REMEMBER TO UPDATE _LAYOUT_.CS ASWELL!!!!!
         bundles.UseCdn = true;
         bundles.Add(new StyleBundle("~/styles/mainStyles").IncludeDirectory("~/Content", "*.css", true));

         bundles.Add(new ScriptBundle("~/bundles/libraries/angularjs", "https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.3/angular.min.js"));
         bundles.Add(new ScriptBundle("~/bundles/libraries/angularjsplugins").IncludeDirectory("~/Content/Scripts/Libraries/AngularJs", "*js", searchSubdirectories: true));
         bundles.Add(new ScriptBundle("~/bundles/libraries/angularmaterialicons").IncludeDirectory("~/Content/Scripts/Libraries/AngularMaterialIcons", "*js", true));

         bundles.Add(new ScriptBundle("~/bundles/libraries/angularmaterial", "https://cdnjs.cloudflare.com/ajax/libs/angular-material/1.1.6/angular-material.min.js"));
         bundles.Add(new StyleBundle("~/bundles/libraries/angularmaterialcss", "https://cdnjs.cloudflare.com/ajax/libs/angular-material/1.1.6/angular-material.min.css"));
         bundles.Add(new ScriptBundle("~/bundles/libraries/jquery").IncludeDirectory("~/Content/Scripts/Libraries/jQuery", "*js", searchSubdirectories: true));
          bundles.Add(new ScriptBundle("~/bundles/libraries/crypto-js").IncludeDirectory("~/Content/Scripts/Libraries/crypto-js", "*js", searchSubdirectories: true));
         bundles.Add(new ScriptBundle("~/bundles/libraries/ngsanitize").Include("~/Content/Scripts/Libraries/ngsanitize/ngsanitize.js"));
         bundles.Add(new ScriptBundle("~/bundles/libraries/angularchart").Include("~/Content/Scripts/Libraries/angular-chart/angular-chart.js"));

         bundles.Add(new ScriptBundle("~/bundles/libraries/ngInfiniteScroll").Include("~/Content/Scripts/Libraries/ngInfiniteScroll/ng-infinite-scroll.js"));
         bundles.Add(new ScriptBundle("~/bundles/libraries/chartjs", "https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.0/Chart.min.js"));
        
         bundles.Add(new ScriptBundle("~/bundles/components/barindicator").Include("~/Content/Scripts/Components/barIndicator-2015_08_24/jquery-barIndicator.js"));
         bundles.Add(new ScriptBundle("~/bundles/components/easing").Include("~/Content/Scripts/Components/barIndicator-2015_08_24/easing.js"));

         bundles.Add(new ScriptBundle("~/bundles/components/ngclipboard").IncludeDirectory("~/Content/Scripts/Components/ngclipboard", "*.js", searchSubdirectories: true));
         bundles.Add(new ScriptBundle("~/bundles/components/qrcode").Include("~/Content/Scripts/Components/qrcode/qrcodeGenerator.js"));
         bundles.Add(new ScriptBundle("~/bundles/components/roundprogressbar").Include("~/Content/Scripts/Components/round-progressbar-0.3.9/roundProgress.js"));

         bundles.Add(new ScriptBundle("~/bundles/guartinel/app").IncludeDirectory("~/Scripts/", "*.js", searchSubdirectories: true));
         bundles.Add(new ScriptBundle("~/bundles/guartinel/pluginScripts").IncludeDirectory("~/Plugins", "*js", searchSubdirectories: true));
         bundles.Add(new StyleBundle("~/styles/pluginStyles").IncludeDirectory("~/Plugins", "*css", searchSubdirectories: true));

         BundleTable.Bundles.ForEach(x => x.Transforms.Clear());

         // Set EnableOptimizations to false for debugging. For more information,
         // visit http://go.microsoft.com/fwlink/?LinkId=301862
         BundleTable.EnableOptimizations = true;
      }
   }
}

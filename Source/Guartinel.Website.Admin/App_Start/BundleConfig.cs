using System.Web.Optimization ;

namespace Guartinel.Website.Admin {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {

            // todo: find a way to add minified version of the scripts without specifying version
            // static content, 3rd party scripts
            bundles.Add(new ScriptBundle("~/bundles/libraries").IncludeDirectory("~/Content/Scripts/Libraries", "*js", true));
            bundles.Add(new ScriptBundle("~/bundles/components").IncludeDirectory("~/Content/Scripts/Components", "*js", true));
         
            // styles
            bundles.Add(new StyleBundle("~/styles/mainStyles").IncludeDirectory("~/Content", "*.css", true));
            // custom scripts written by developers
            bundles.Add(new ScriptBundle("~/bundles/customScripts").IncludeDirectory("~/Scripts", "*.js", true));
          
            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
            }
        }
    }
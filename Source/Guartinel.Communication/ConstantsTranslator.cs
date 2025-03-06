using System ;
using System.IO ;
using System.Text ;
using Guartinel.Communication.Strings ;
using Newtonsoft.Json ;

namespace Guartinel.Communication {
   public static class ConstantsTranslator {
      public const string USER_WEBSITE_CONSTANTS_PATH = "Guartinel.Website.User/Scripts/constants/" ;
      public const string ADMIN_WEBSITE_CONSTANTS_PATH = "Guartinel.Website.Admin/Scripts/constants/" ;
      public const string MANAGEMENT_SERVER_CONSTANTS_PATH = "Guartinel.ManagementServer/common/" ;

      public static void UpdateUSERWebSiteConstants() {
         string commonsSerialization = JsonConvert.SerializeObject (new Strings.Strings.CommonSerializationSummary()) ;
         string backendUserAPISerialization = JsonConvert.SerializeObject (new UserWebsiteAPI.AllURLSerializationSummary()) ;

         AllStrings plugins = new AllStrings() ;
         string packageTypesJSON = JsonConvert.SerializeObject (plugins.GetAllPackageTypeValues()) ;
         string constantsJSON = plugins.ConstantsToJSON() ;
         string messagesJSON = plugins.MessagesToJSON().Replace(@"\", @"\\").Replace ("'", @"\'") ;
         string constantsFileText =
               @"var commonConstants;
               var backendUserApiUrls;
               var languageTable;
               var plugins;  
               var pluginConstants;
                  try {  
                     languageTable = JSON.parse('" + messagesJSON + @"');
                     commonConstants = JSON.parse('" + commonsSerialization + @"');
                     backendUserApiUrls = JSON.parse('" + backendUserAPISerialization + @"');
                     plugins = JSON.parse('" + packageTypesJSON + @"') ;
                     pluginConstants = JSON.parse('" + constantsJSON + @"') ;
                } catch (error) {
              console.error('Cannot load constants.The application will not work reliable.' + error);
              }";
         CreateConstantsSerialization (Path.Combine (GetSolutionDir(), USER_WEBSITE_CONSTANTS_PATH), "generatedConstants.js", constantsFileText) ;
      }

      public static void UpdateADMINWebSiteConstants() {
         string commonsSerialization = JsonConvert.SerializeObject (new Strings.Strings.CommonSerializationSummary()) ;
         string backendAdminAPISerialization = JsonConvert.SerializeObject (new AdminWebsiteAPI.AllURLSerializationSummary()) ;

         string constantsFileText =
               @"var commonConstants;               
               var backendAdminApiUrls;
                 try {
                     commonConstants = JSON.parse('" + commonsSerialization + @"');
                     backendAdminApiUrls = JSON.parse('" + backendAdminAPISerialization + @"');
                } catch (error) {
              console.error('Cannot load constants.The application will not work reliable.' + error);
              }" ;

         CreateConstantsSerialization (Path.Combine (GetSolutionDir(), ADMIN_WEBSITE_CONSTANTS_PATH), "generatedConstants.js", constantsFileText) ;
      }

      public static void UpdateManagementServerConstants() {
         string commonsSerialization = JsonConvert.SerializeObject (new Strings.Strings.CommonSerializationSummary()) ;
         string managementServerAPISerialization = JsonConvert.SerializeObject (new ManagementServerAPI.AllURLSerializationSummary()) ;
         string watcherServerAPISerialization = JsonConvert.SerializeObject (new WatcherServerAPI.AllURLSerializationSummary()) ;

         AllStrings plugins = new AllStrings() ;
         string packageTypesJSON = JsonConvert.SerializeObject (plugins.GetAllPackageTypeValues()) ;
         string constantsJSON = plugins.ConstantsToJSON() ;
         string messagesJSON = plugins.MessagesToJSON().Replace (@"\", @"\\").Replace ("'", @"\'") ;
         string constantsFileText =

               @"export function registerGlobals() :void { 
                  try {
                     global.commonConstants = "+ commonsSerialization + @"
	                 global.managementServerUrls =" + managementServerAPISerialization + @"
	                 global.watcherServersUrls = " + watcherServerAPISerialization + @"
	                 global.plugins = " + packageTypesJSON + @"
	                 global.pluginConstants =" + constantsJSON + @"
	                 global.languageTable = " + messagesJSON + @"
                    Const.commonConstants = global.commonConstants;
                    Const.managementServerUrls = global.managementServerUrls;
                    Const.plugins = global.plugins;
                    Const.pluginConstants = global.pluginConstants;
                    Const.languageTable = global.languageTable;
	                 } catch (error) {
                     console.error('Cannot load constants.The application will not work reliable.' + error);
                    }
                  }
                     export module Const {
                     export let commonConstants;
                     export let managementServerUrls ;
                     export let plugins ;
                     export let pluginConstants ;
                     export let languageTable ;
                 }";
              

         CreateConstantsSerialization (Path.Combine (GetSolutionDir(), MANAGEMENT_SERVER_CONSTANTS_PATH), "constants.ts", constantsFileText) ;
      }

      private static void CreateConstantsSerialization (string folder,
                                                        string fileName,
                                                        string constantsFileText) {
         if (File.Exists (folder)) { File.Delete (folder) ; }
         WriteToFile (folder + '\\' + fileName, constantsFileText) ;
      }

      private static void WriteToFile (string path,
                                       string data) {
         using (FileStream fs = File.Create (path)) {
            Byte[] dataBytes = new UTF8Encoding (true).GetBytes (data) ;
            // Add some information to the file.
            fs.Write (dataBytes, 0, dataBytes.Length) ;
         }
      }

      private static string GetSolutionDir() {
         string currentDir = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly().Location) ;
         DirectoryInfo binDir = Directory.GetParent (currentDir) ;
         string solutionDir = binDir.Parent.Parent.FullName ;
         return solutionDir ;
      }
   }
}

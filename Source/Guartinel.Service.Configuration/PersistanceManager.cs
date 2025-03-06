using System ;
using System.Diagnostics ;
using System.IO ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.Service.Configuration.Replication ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.Configuration {
   public class PersistanceManager {
      private string _dataPath ;

      public string GetDataContentHash() {
         ProcessStartInfo processStartInfo = new ProcessStartInfo() ;
         processStartInfo.FileName = "/bin/bash" ;
         string command = "md5deep " + _dataPath + " -r -l | sort | md5sum" ;
         Console.WriteLine (command) ;
         processStartInfo.Arguments = "-c \"" + command + "\"" ;
         processStartInfo.UseShellExecute = false ;
         processStartInfo.RedirectStandardOutput = true ;

         Process process = new Process {
                  StartInfo = processStartInfo
         } ;

         process.EnableRaisingEvents = true ;
         process.Start() ;
         string result = process.StandardOutput.ReadToEnd() ;
         process.WaitForExit() ;
         return result.Split (' ') [0] ;
      }

      public PersistanceManager (string dataPath) {
         this._dataPath = dataPath ;
         Logger.Info ("Init persistance. DataHash path: " + dataPath) ;
      }

      public void Reset() {
         if (!Directory.Exists (_dataPath)) {
            return ;
         }
         DirectoryInfo directoryInfo = new DirectoryInfo (_dataPath) ;
         directoryInfo.Delete (true) ;
         System.IO.Directory.CreateDirectory (_dataPath) ;
      }

      public void SetToken (string key,
                            string token) {
         WriteTo (key, "token", token) ;
      }

      public string GetToken (string path) {
         return File.ReadAllText (Path.Join (_dataPath, path, "token")) ;
      }

      public string GetData (string path) {
         return File.ReadAllText (Path.Join (_dataPath, path, "data")) ;
      }

      public void SetData (string key,
                           string value) {
         WriteTo (key, "data", value) ;
      }

      private void WriteTo (string folderKey,
                            string fileName,
                            string value) {
         Logger.Info ($"FolderKey:{folderKey} FileName:{fileName} Value: {value} ") ;
         string folderPath = Path.Join (_dataPath, folderKey) ;
         string filePath = Path.Join (folderPath, fileName) ;
         Logger.Info ($"Folderpath:{folderPath} FilePath:{filePath}") ;
         System.IO.Directory.CreateDirectory (folderPath) ; // it wont create it if already exists

         if (!Directory.Exists (folderPath)) {
            Logger.Info ("Directory still not exists..") ;
         }

         File.WriteAllText (filePath, value) ;
      }

      public void IterateOverAllData (string currentDataPath,
                                      Replica replica,
                                      Action<Replica, string, string, JObject> doSet) {
         if (currentDataPath == null) {
            currentDataPath = _dataPath ;
         }

         Logger.Info ($"IterateOverAllData. Current dir: {currentDataPath}") ;

         string token = null ;
         string data = null ;
         string truncatedPath = currentDataPath.Replace (_dataPath, "") ;
         try {
            data = GetData (truncatedPath) ;
            token = GetToken (truncatedPath) ;
         } catch (Exception e) {
            Logger.Error ($"Could not load token/data {e.GetAllMessages()}") ;
         }

         if (token != null && data != null) {
            JObject parsedData = JObject.Parse (data) ;
            doSet (replica, token, truncatedPath, parsedData) ;
         }

         // Recurse into subdirectories of this directory.
         string[] subdirectoryEntries = Directory.GetDirectories (currentDataPath) ;
         foreach (string subdirectory in subdirectoryEntries) {
            IterateOverAllData (subdirectory, replica, doSet) ;
         }
      }
   }
}

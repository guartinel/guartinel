using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Guartinel.CLI.Files ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.CLI.OperatingSystem.Flavors {
   public class Linux : OperatingSystem {
      public override long GetFreeMemoryBytes(string[] tags) {
         string[] result = RunCommand (@"free -b |grep 'Mem: ' | awk '{usage=($4)} END {print usage}'").Split (new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ;
         if (result.Length == 0) {
            throw new Exception ("Cannot get GetFreeMemoryBytes.") ;
         }

         long memoryInByte = long.Parse (result [0]) ;
         return memoryInByte ;
      }

      public override long GetTotalMemoryBytes(string[] tags) {
         string[] result = RunCommand (@"free -b |grep 'Mem: ' | awk '{usage=($2)} END {print usage}'").Split (new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ;
         if (result.Length == 0) {
            throw new Exception ("Cannot get GetTotalMemoryBytes.") ;
         }

         long memoryInByte = long.Parse (result [0]) ;
         return memoryInByte ;
      }

      public override double GetProcessorUsage(string[] tags) {
         string[] result = RunCommand (@"grep 'cpu ' /proc/stat | awk '{usage=($2+$4)*100/($2+$4+$5)} END {print usage}'").Split (new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ;
         if (result.Length == 0) {
            throw new Exception ("Cannot get GetProcessorUsage.") ;
         }

         double usagePercent = double.Parse (result [0]) ;
         return usagePercent ;
      }

      public override ServiceState IsServiceRunning (string serviceName,
                                                     string[] tags) {
         var logger = new TagLogger (_logger.Tags, tags) ;
         
         string resultRaw = RunCommand (@"ps aux |grep " + serviceName) ;
         logger.Info ($"Result: {resultRaw}") ;

         string[] result = resultRaw.Split (new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ;
         if (result.Length == 3) {
            return ServiceState.NotFound ;
         }

         return ServiceState.Running ;
      }

      public override FreeSpace GetFolderSpace (string folderName,
                                                string[] tags) {
         return null ;
      }

      private string RunCommand (string command) {
         var process = new Process() {
                  StartInfo = new ProcessStartInfo {
                           FileName = "/bin/bash",
                           Arguments = $"-c \"{command}\"",
                           RedirectStandardOutput = true,
                           UseShellExecute = false,
                           CreateNoWindow = true,
                  }
         } ;
         process.Start() ;
         string result = process.StandardOutput.ReadToEnd() ;
         process.WaitForExit() ;
         return result ;
      }
   }
}
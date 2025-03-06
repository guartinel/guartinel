#if NET462
#define WINDOWS
#endif

using Guartinel.Kernel.Logging;
using System;
using System.Diagnostics ;
using System.Linq;
using System.Text;
using Guartinel.CLI.Files ;

namespace Guartinel.CLI.OperatingSystem.Flavors {
   public class Windows : OperatingSystem {
      public override long GetFreeMemoryBytes (string[] tags) {
         string[] result = RunCommand (@"wmic OS get FreePhysicalMemory").Split (new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ;
         if (result.Length == 0) {
            throw new Exception ("Cannot get free memory bytes.") ;
         }

         long memoryInByte = long.Parse (result [1]) * 1024 ;
         return memoryInByte ;
      }

      public override long GetTotalMemoryBytes (string[] tags) {
         string[] result = RunCommand (@"wmic computersystem get TotalPhysicalMemory").Split (new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ;
         if (result.Length == 0) {
            throw new Exception ("Cannot get total memory bytes.") ;
         }

         long memoryInByte = long.Parse (result [1]) ;
         return memoryInByte ;
      }

      public override double GetProcessorUsage(string[] tags) {
         string[] result = RunCommand (@"wmic cpu get loadpercentage").Split (new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ;
         if (result.Length == 0) {
            throw new Exception ("Cannot get processor usage.") ;
         }

         double usagePercent = double.Parse (result [1]) ;
         return usagePercent ;
      }

      public override ServiceState IsServiceRunning (string serviceName,
                                                     string[] tags) {
         Logger.Log (LogLevel.Info, "IsServiceRunning request to win IOperatingSystem instance") ;
         string[] result ;
         try {
            result = RunCommand ($@"sc query {serviceName}").Split (new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ;
         } catch (Exception exception) {
            Logger.Log (LogLevel.Error, $@"Exception: {exception.Message}") ;
            throw ;
         }

         if (result [0].Contains ("FAILED")) {
            return ServiceState.NotFound ;
         }

         if (result [2].Contains ("RUNNING")) {
            return ServiceState.Running ;
         }

         return ServiceState.Stopped ;
      }

      public override FreeSpace GetFolderSpace (string folderName,
                                                string[] tags) {
#if WINDOWS
         bool success = WinAPI.GetDiskFreeSpaceEx(folderName,
                                                  out var freeBytesAvailable,
                                                  out var totalNumberOfBytes,
                                                  out var totalNumberOfFreeBytes);
         if (!success) {
            throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
         }

         Logger.Log($"Folder '{folderName}' free space check: {freeBytesAvailable}, {totalNumberOfBytes}, {totalNumberOfFreeBytes}");
         return new FreeSpace (totalNumberOfFreeBytes, totalNumberOfBytes) ;
#else
         return null ;
#endif
      }

      private string RunCommand (string command) {
         try {
            Process p = new Process() ;
            p.StartInfo.UseShellExecute = false ;
            p.StartInfo.RedirectStandardOutput = true ;
            p.StartInfo.FileName = @"cmd.exe" ;
            p.StartInfo.Arguments = @"/c " + command ;
            p.Start() ;
            string output = p.StandardOutput.ReadToEnd() ;
            p.WaitForExit() ;
            return output ;
         } catch (Exception exception) {
            Logger.Log (LogLevel.Error, $@"Cannot run command {command} because exception: {exception.Message}") ;
            throw ;
         }
      }
   }

#if WINDOWS
   public class WinAPI {
      [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
      [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
      public static extern bool GetDiskFreeSpaceEx (string lpDirectoryName,
                                                    out ulong lpFreeBytesAvailable,
                                                    out ulong lpTotalNumberOfBytes,
                                                    out ulong lpTotalNumberOfFreeBytes);
   }
#endif
}

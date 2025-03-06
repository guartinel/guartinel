using Guartinel.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Guartinel.CLI.OperatingSystem.Flavors ;

namespace Guartinel.CLI.OperatingSystem {
   public class OperatingSystemSelector {
      public enum OSType {
         Windows,
         Linux
      }
      private static OSType _selectedOSType;
      public static IOperatingSystem CurrentOperatingSystem {
         get {
            if ( _selectedOSType == OSType.Windows ) {
               return new Windows();
            }
            if ( _selectedOSType == OSType.Linux ) {
               return new Linux();
            }
            throw new Exception("Cannot find flavor for selected OS");
         }
      }
      static OperatingSystemSelector () {
         string windir = Environment.GetEnvironmentVariable("windir");
         if ( !string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir) ) {
            _selectedOSType = OSType.Windows;
            Logger.Log("OSSelector identified OS: Windows");
            return;
         }
         
         if ( File.Exists(@"/proc/sys/kernel/ostype") ) {
            string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
            if ( osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase) ) {
               // Note: Android gets here too
               _selectedOSType = OSType.Linux;
               Logger.Log("OSSelector identified OS: Linux");
            }
            else {
               throw new Exception("Cannot recognize os: " + osType);
            }
         }
      }
   }
}

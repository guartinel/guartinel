using System;
using System.Linq;
using System.Text;
using Guartinel.CLI.Files ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.CLI.OperatingSystem {
   public abstract class OperatingSystem : IOperatingSystem {
      protected OperatingSystem() {
      }

      protected TagLogger _logger = new TagLogger() ;

      public abstract long GetFreeMemoryBytes(string[] tags) ;
      public abstract long GetTotalMemoryBytes(string[] tags) ;
      public abstract double GetProcessorUsage(string[] tags) ;
      public abstract ServiceState IsServiceRunning (string serviceName,
                                                     string[] tags) ;
      public abstract FreeSpace GetFolderSpace (string folderName,
                                                string[] tags) ;
   }
}

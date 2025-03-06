using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guartinel.CLI.Files ;

namespace Guartinel.CLI.OperatingSystem {
   public interface IOperatingSystem {
      long GetFreeMemoryBytes(string[] tags) ;
      long GetTotalMemoryBytes(string[] tags) ;
      double GetProcessorUsage(string[] tags) ;
      ServiceState IsServiceRunning (string serviceName,
                                     string[] tags) ;
      FreeSpace GetFolderSpace (string folderName,
                                string[] tags) ;
   }
}
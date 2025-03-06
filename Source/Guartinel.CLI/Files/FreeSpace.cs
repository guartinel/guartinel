using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guartinel.CLI.Files {
   public class FreeSpace {
      public FreeSpace (ulong freeSpaceBytes,
                            ulong allSpaceBytes) {
         FreeSpaceBytes = freeSpaceBytes ;
         // Convert to GBs
         FreeSpaceGBs = Kernel.Utility.Converter.NormalizeValue (freeSpaceBytes / 1024.0 / 1024.0 / 1024.0) ;
         FreeSpacePercents = Kernel.Utility.Converter.Percent (freeSpaceBytes, allSpaceBytes) ;
      }

      public double FreeSpaceGBs {get ;}
      public double FreeSpaceBytes {get ;}
      public int FreeSpacePercents {get ;}
   }
}
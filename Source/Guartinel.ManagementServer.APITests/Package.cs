using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guartinel.ManagementServer.APITests {
   public class Package {
      public Package(string name, int version) {
         Name = name ;
         Version = version ;
      }

      public string Name {get ; set ;}
      public int Version { get; set; }
   }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guartinel.Communication.Languages {
   public class MessageTable {
      private readonly Dictionary<string, string> _messages = new Dictionary<string, string>();
      
      public Dictionary<string, string> Messages => _messages ;
   }
}

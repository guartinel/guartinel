using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Communication.Plugins.WebsiteSupervisor {
   public class MessageTable : MessageTableBase {
      public MessageTable() {
         Add (new Languages.English()) ;
      }
   }
}

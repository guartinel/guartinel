using System.Collections.Generic ;
using Guartinel.Communication.Plugins ;

namespace Guartinel.Communication.Messages {
   public abstract class MessageTable {
      public abstract string Name { get; }
      public abstract string Language { get; }

      // Key: name of message
      // Value: message
      private readonly Dictionary<string, string> _messages = new Dictionary<string, string>() ;

      public Dictionary<string, string> Messages => _messages ;      
   }
}

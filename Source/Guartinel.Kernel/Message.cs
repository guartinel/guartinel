using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Entities ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Kernel {
   public delegate void MessageEvent (Message message);

   // public class Message : IDuplicable {
   public class Message {
      public static class ParameterNames {
         public const string MESSAGE_TEXT = "message_text" ;
      }

      #region Construction
      public Message() {}

      //public static Creator GetCreator() {
      //   return new Creator (typeof (Message), () => new Message()) ;
      //}

      #endregion

      #region Properties
      
      public string MessageText {get ; set ;}
      
      #endregion

      //public IDuplicable Duplicate() {
      //Message result = new Message();
      //   result.MessageText = MessageText ;
         
      //   return result ;
      //}      
   }
}
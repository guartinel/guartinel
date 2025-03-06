using Guartinel.Kernel.Logging;

namespace Guartinel.Website.Common.Tools {
   public static class StringTool {
      public static string EnsureStringEndsToBackSlash (string original) {
         if (string.IsNullOrEmpty (original)) {
             // Logger.Error($"EnsureStringEndsToBackSlash got null string.");
            return string.Empty ;
         }
         if (original.EndsWith ("/")) return original ;
         return original + "/" ;
      }
   }
}

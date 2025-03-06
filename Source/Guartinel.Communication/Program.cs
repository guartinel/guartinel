using System;

namespace Guartinel.Communication {
   public class Program {
      public static void Main (string[] args) {
         try {
            Console.WriteLine("Translating User website constants....");
            ConstantsTranslator.UpdateUSERWebSiteConstants();
         } catch (Exception e) {
            Console.WriteLine("Error while doing user website constants translation:\n" + e.Message);
         }

         try {
            Console.WriteLine("Translating Admin website constants....");
            ConstantsTranslator.UpdateADMINWebSiteConstants();
         } catch (Exception e) {
            Console.WriteLine("Error while doing admin website constants translation:\n" + e.Message);
         }
         try {
            Console.WriteLine("Translating Management Server constants...");
            ConstantsTranslator.UpdateManagementServerConstants();
         } catch (Exception e) {
            Console.WriteLine("Error while doing Management Server constants translation:\n" + e.Message);
         }
         Console.WriteLine("All work is done.\nPress a key to exit.");
         Console.ReadKey();
      }
   }
}

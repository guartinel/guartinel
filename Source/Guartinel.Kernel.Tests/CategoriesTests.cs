using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   [TestFixture]
   internal class CategoriesTests {
      [Test]
      public void CreateCategories_CheckMatch() {
         Categories emptyCategories = new Categories() ;         
         Categories categories = new Categories(new List<string> {"Free", "Paid", "VIP"}) ;         
         Categories categoriesNoMatch = new Categories(new List<string> {"Other1", "Other2"}) ;
         Categories categoriesOneMatch = new Categories(new List<string> {"Other1", "Free", "Other2"}) ;
         Categories categoriesMatchWithCase = new Categories(new List<string> {"Other1", "vip", "Other2"}) ;
         
         Assert.IsFalse (categories.Matches (categoriesNoMatch));
         Assert.IsFalse (categoriesNoMatch.Matches (categories));

         Assert.IsTrue (categories.Matches (categoriesOneMatch));
         Assert.IsTrue (categoriesOneMatch.Matches (categories));

         Assert.IsFalse (categories.Matches (categoriesMatchWithCase));
         Assert.IsFalse (categoriesMatchWithCase.Matches (categories));

         // If the other category list is empty, then matches!
         Assert.IsTrue (categories.Matches (emptyCategories));
         // If the this category list is empty, then no match!
         Assert.IsFalse (emptyCategories.Matches (categories));
      }
   }
}

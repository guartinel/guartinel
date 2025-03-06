using System ;
using System.Linq ;
using System.Text ;
using NUnit.Framework ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Kernel.Tests.Utility {
   [TestFixture]
   internal class HashingTests {
      [Test]
      public void TestHashings() {
         var hash1 = Hashing.GenerateHash ("tesT123PasSw412oRd6544", "test@ehune.com") ;
         Assert.IsNotNull (hash1) ;
         Assert.AreEqual ("460725C1AD8B82D903D9C6332E1C45F5EF4F2E23B51C15069D129E96BFC13D2969C8E3AF32BF31D315638B7F2B271068529B3C96E7029E7F8F15410BEE539DF1".ToLower(), hash1.ToLower()) ;

         var hash2 = Hashing.GenerateHash (hash1, "test@ehuneX.com") ;
         Assert.IsNotNull (hash2) ;
         Assert.AreEqual ("1FAD3CE0622872EF8B883E3D97CF405A31154270F57483B0603A41B1F4AE834BDCA271EE9C8262F4C5A5B892891100B75066F9E2EB84DBA0330F2B74FFC6281B".ToLower(), hash2.ToLower()) ;
      }
   }
}
using System ;
using System.Linq ;
using System.Text ;
// using Guartinel.Kernel.Factories ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   public class TestBase {
      [SetUp]
      public void Setup() {
         // Registration.Register() ;

         // Register test classes
         // Factory.Use.RegisterCreator (new Creator (typeof (TestBook), () => new TestBook())) ;

         IoC.Use.Clear();

         Setup1() ;
      }

      [TearDown]
      public void TearDown() {
         // Factory.Use.UnregisterCreators (typeof (TestBook)) ;

         // Registration.Unregister() ;

         IoC.Use.Clear();

         TearDown1() ;
      }

      protected virtual void Setup1() {         
      }

      protected virtual void TearDown1() {
      }      
   }
}
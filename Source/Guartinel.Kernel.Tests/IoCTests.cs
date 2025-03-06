using System;
using System.Linq;
using System.Text;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   public interface ITest1 { }

   public class Test1 : ITest1 {
      public Test1() { }
   }

   public class Test2 : ITest1 {
      public Test2 () { }
   }

   public class Test3 : ITest1 {
      public string Name = string.Empty;

      public Test3 (string name) {
         Name = name ;
      }
   }

   [TestFixture]
   public class IoCTests : TestBase {
      [Test]
      public void RegisterCollectionTwice () {
         IoC.Use.Multi.Register<ITest1, Test1>() ;
         IoC.Use.Multi.Register<ITest1, Test2>() ;
         IoC.Use.Multi.Register<ITest1> (() => new Test3 ("test3")) ;
      }

      [Test]
      public void RegisterInstance_RegisterCollection_Test () {
         IoC.Use.Multi.Register<ITest1, Test1>();

         Assert.IsTrue(IoC.Use.Multi.ImplementationExists<ITest1>());
         var instances = IoC.Use.Multi.GetInstances<ITest1>();

         Assert.AreEqual(1, instances.Count);
      }

      [Test]   
      public void TestNoRegisteredInstances() {
         Assert.IsFalse (IoC.Use.Multi.ImplementationExists<ITest1>()) ;
      }

      [Test]
      public void RegisterInstance_TestIfExists () {
         IoC.Use.Single.Register<ITest1, Test1>() ;
         
         IoC.Use.Multi.Register<ITest1, Test1, Test2>() ;

         Assert.IsTrue (IoC.Use.Multi.ImplementationExists<ITest1>()) ;
         var instances = IoC.Use.Multi.GetInstances<ITest1>() ;

         Assert.AreEqual (2, instances.Count) ;
      }

      [Test]
      public void RegisterCreator_TestIfExists () {
         IoC.Use.Single.Register<ITest1> (() => new Test3 ("testX")) ;

         Assert.IsTrue(IoC.Use.Single.ImplementationExists<ITest1>());
         var instance = IoC.Use.Single.GetInstance<ITest1>() ;
         Assert.AreEqual("testX", ((Test3) instance).Name);
      }

      [Test]
      public void RegisterAssembly_TestRegistrations () {
         IoC.Use.Multi.Register<ITest1>(typeof(ITest1).Assembly);

         Assert.Greater(IoC.Use.Multi.GetInstances<ITest1>().Count, 0) ;
      }

      [Test]
      public void RegisterAssembly_Twice () {
         IoC.Use.Multi.Register<ITest1>(typeof(ITest1).Assembly);
         IoC.Use.Multi.Register<ITest1>(typeof(ITest1).Assembly);
      }

      [Test]
      public void RegisterWithNames_TestIfFound () {
         IoC.Use.Multi.Register<ITest1, Test1>(new[]{"Test1_A", "Test1_B" }) ;
         IoC.Use.Multi.Register<ITest1, Test2>(new[] {"Test2_A", "Test2_B" });

         Assert.AreEqual ("Test1", IoC.Use.Multi.GetInstance<ITest1> ("Test1_A").GetType().Name) ;
         Assert.AreEqual ("Test2", IoC.Use.Multi.GetInstance<ITest1> ("Test2_B").GetType().Name) ;
      }
   }
}
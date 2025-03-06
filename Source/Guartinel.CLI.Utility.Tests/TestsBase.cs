using System;
using System.Linq;
using System.Reflection ;
using System.Text;
using Guartinel.CLI.Utility.Commands ;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core ;
using Guartinel.Core.Logging ;
using NUnit.Framework ;
using SimpleInjector ;

namespace Guartinel.CLI.Utility.Tests {
   public abstract class TestsBase {
      [SetUp]
      protected virtual void SetUp() {
         IoC.Clear() ;

         // No logger
         RegisterLoggers() ;
                  
         IoC.RegisterImplementations<ICommand> (Assembly.GetAssembly (typeof (Application))) ;         

         Register();

         // Verify the registration
         IoC.Use.Verify();
      }

      protected virtual void RegisterLoggers() {
         Logger.Setup<NullLogger> ("Test", "Test") ;
      }

      protected virtual void Register() {}

      [TearDown]
      protected virtual void TearDown() {
         IoC.Clear();
      }
   }
}
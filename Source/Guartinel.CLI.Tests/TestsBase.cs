using System;
using System.Collections.Generic ;
using System.Linq;
using System.Reflection ;
using System.Text ;
using Guartinel.CLI.Commands ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Microsoft.Extensions.CommandLineUtils ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests {
   public abstract class TestsBase {
      [SetUp]
      protected virtual void SetUp() {
         IoC.Use.Clear() ;

         // No logger
         RegisterLoggers() ;

         IoC.Use.Multi.Register<ICommand> (Assembly.GetAssembly (typeof(Application))) ;
         IoC.Use.Multi.Register<ICommandLineCommand> (Assembly.GetAssembly (typeof(Application))) ;

         Register() ;

         // Verify the registration
         IoC.Use.Verify() ;
      }

      protected virtual void RegisterLoggers() {
         Logger.Setup<NullLogger> ("Test", "Test") ;
      }

      protected virtual void Register() { }

      [TearDown]
      protected virtual void TearDown() {
         IoC.Use.Clear() ;
      }

      protected List<CheckResult> RunCommand (string commandName,
                                              Func<List<string>> createArguments) {
         ICommandLineCommand command = IoC.Use.Multi.GetInstances<ICommandLineCommand>().First (x => x.Command == commandName) ;
         CommandLineApplication parser = new CommandLineApplication() ;
         ICommandLineCommand commandToRun = null ;
         command.Setup (parser, command1 => commandToRun = command1) ;

         var arguments = createArguments?.Invoke() ;
         Assert.IsNotNull (arguments) ;

         parser.Execute (arguments.ToArray()) ;
         Assert.AreEqual (command, commandToRun) ;

         return command.Run() ;
      }
   }
}
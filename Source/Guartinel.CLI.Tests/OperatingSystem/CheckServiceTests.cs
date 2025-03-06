using Guartinel.Kernel.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guartinel.CLI.Tests.OperatingSystem {
   [TestFixture]
   public class CheckServiceTests : TestsBase {

      private const string EXISTING_SERVICE_NAME = @"EventSystem";
    
      [Test]
      public void CheckValidService () {
         var result1 = RunCommand(EXISTING_SERVICE_NAME, 101)[0];
         Assert.IsTrue(result1.Success);       
      }

      [Test]
      public void CheckInvalidService () {
         var result3 = RunCommand("ehune", 103)[0];
         Assert.IsFalse(result3.Success);
      }

      private List<CheckResult> RunCommand (string serviceName,
                                            int instanceID) {
         return RunCommand("checkService", () => CreateArguments(serviceName, instanceID));
      }

      private List<string> CreateArguments (string serviceName,
                                            int instanceID) {
         List<string> arguments = new List<string>();
         arguments.Add("checkService");
         arguments.Add($"--serviceName={serviceName.WrapInDoubleQuotesIfContainsWhitespace()}");
         arguments.Add($"--id={instanceID}");
         return arguments;
      }
   }
}

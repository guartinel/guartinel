using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guartinel.CLI.Tests.OperatingSystem {
   [TestFixture]
   class CheckMemoryTests : TestsBase {

      [Test]
      public void TestMemory () {
         var result1 = RunCommand(0.1, 1,666)[0];
         Assert.IsTrue(result1.Success);
         var result2 = RunCommand(100, 10,666)[0];
         Assert.IsFalse(result2.Success);
      }

      private List<CheckResult> RunCommand (double minFreeMemoryGBs,
                                            double minFreeMemoryPercents,
                                            int instanceID) {
         return RunCommand("checkMemory", () => CreateArguments(minFreeMemoryGBs, minFreeMemoryPercents, instanceID));
      }

      private List<string> CreateArguments (double minFreeMemoryGBs,
                                            double minFreeMemoryPercents,
                                            int instanceID) {
         List<string> arguments = new List<string>();
         arguments.Add("checkMemory");
         arguments.Add($"--minFreeMemoryGBs={minFreeMemoryGBs}");
         arguments.Add($"--minFreeMemoryPercents={minFreeMemoryPercents}");
         arguments.Add($"--id={instanceID}");
         return arguments;
      }
   }
}

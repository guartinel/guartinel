using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guartinel.CLI.Tests.OperatingSystem {
   [TestFixture]
   class CheckProcessorTests : TestsBase {


      [Test]
      public void TestCPU () {
          var result1 = RunCommand(100, 101)[0];
          Assert.IsTrue(result1.Success);
          var result2 = RunCommand(0, 102)[0];
          Assert.IsFalse(result2.Success);    
      }
   
      private List<CheckResult> RunCommand (double maxCPUPercents,
                                            int instanceID) {
         return RunCommand("checkProcessor", () => CreateArguments(maxCPUPercents, instanceID));
      }

      private List<string> CreateArguments (double maxCPUPercents,
                                            int instanceID) {
         List<string> arguments = new List<string>();
         arguments.Add("checkProcessor");
         arguments.Add($"--maxProcessorUsagePercents={maxCPUPercents}");
         arguments.Add($"--id={instanceID}");
         return arguments;
      }
   }
}

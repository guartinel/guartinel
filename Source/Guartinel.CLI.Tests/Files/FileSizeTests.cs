using System.Collections.Generic;
using System.IO ;
using System.Linq;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Files {
   [TestFixture]
   public class FileSizeTests : FileTestsBase {
      [Test]
      public void WriteFiles_CheckSize() {
         // First, get the appropriate command
         int fileSize = 100 ;

         WriteTestFile (null, "testfile.size", fileSize) ;

         var result = RunCommand (Path.Combine (_testFolder, "testfile.size"), fileSize + 1)[0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         // Invalid filename should be OK
         result = RunCommand (Path.Combine (_testFolder, "testfile.sizeX"), fileSize + 1)[0];
         Assert.IsTrue (result.Success, result.ToString()) ;

         // Pattern OK
         result = RunCommand (Path.Combine (_testFolder, "*.size"), fileSize + 1)[0];
         Assert.IsTrue (result.Success, result.ToString()) ;

         // Pattern not OK
         result = RunCommand (Path.Combine (_testFolder, "*.size"), fileSize - 1)[0];
         Assert.IsTrue (result.Success, result.ToString()) ;
      }

      private List<CheckResult> RunCommand (string pattern,
                                            int maxSize) {
         return RunCommand ("checkFileSize", () => CreateArguments (pattern, maxSize)) ;
      }

      private List<string> CreateArguments (string pattern,
                                            int maxSize) {

         

         List<string> arguments = new List<string>() ;
         arguments.Add ("checkFileSize") ;
         arguments.Add ($"--pattern=\"{pattern}\"") ;
         arguments.Add ($"--maxSize={maxSize}") ;
         return arguments ;
      }
   }
}
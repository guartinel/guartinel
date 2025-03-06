using System.Collections.Generic;
using System.Linq;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Files {
   [TestFixture]
   public class FileExistenceTests : FileTestsBase {
      [Test]
      public void WriteFile_CheckIfExists() {
         // First, get the appropriate command
         int fileSize = 100 ;

         var fileName = $"testfile.exists" ;

         WriteTestFile (null, $"testfile.exists", fileSize) ;

         var result = RunCommand (_testFolder, fileName)[0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (_testFolder, "invalid.filename")[0] ;
         Assert.IsFalse (result.Success, result.ToString()) ;

         result = RunCommand (_testFolder, "*.exists")[0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;
      }

      private List<CheckResult> RunCommand (string folder,
                                            string pattern) {
         return RunCommand ("checkFileExists", () => CreateArguments (folder, pattern)) ;
      }

      private List<string> CreateArguments (string folder,
                                            string pattern) {
         List<string> arguments = new List<string>() ;
         arguments.Add("checkFileExists");
         ;
         arguments.Add ($"--folder={folder}") ;
         arguments.Add ($"--pattern={pattern}") ;
         return arguments ;
      }
   }
}
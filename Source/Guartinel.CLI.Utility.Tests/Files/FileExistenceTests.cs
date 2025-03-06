using System.Collections.Generic;
using System.IO ;
using System.Linq;
using Fclp;
using Guartinel.CLI.Utility.Commands;
using Guartinel.Core ;
using NUnit.Framework;

namespace Guartinel.CLI.Utility.Tests.Files {
   [TestFixture]
   public class FileExistenceTests : FileTestsBase {
      [Test]
      public void WriteFile_CheckIfExists() {
         // First, get the appropriate command
         int fileSize = 100 ;

         var fileName = $"testfile.exists" ;

         WriteTestFile (null, $"testfile.exists", fileSize) ;

         var result = RunCommand (_testFolder, fileName) ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (_testFolder, "invalid.filename") ;
         Assert.IsFalse (result.Success, result.ToString()) ;

         result = RunCommand (_testFolder, "*.exists") ;
         Assert.IsTrue (result.Success, result.ToString()) ;
      }

      private CheckResult RunCommand (string folder,
                                      string pattern) {
         ICommand checkFileExistance = IoC.Use.GetAllInstances<ICommand>().FirstOrDefault (x => x.Command == "checkFileExists") ;
         if (checkFileExistance == null) return new CheckResult.InvalidParameters() ;

         FluentCommandLineParser parser = new FluentCommandLineParser() ;
         checkFileExistance.Setup (parser) ;

         var arguments = CreateArguments (folder, pattern) ;
         parser.Parse (arguments.ToArray()) ;
         return checkFileExistance.Run()[0] ;
      }

      private List<string> CreateArguments (string folder,
                                            string pattern) {
         List<string> arguments = new List<string>() ;
         arguments.Add ("--command:checkFileExists") ;
         arguments.Add ($"--folder:{folder}") ;
         arguments.Add ($"--pattern:{pattern}") ;
         return arguments ;
      }
   }
}
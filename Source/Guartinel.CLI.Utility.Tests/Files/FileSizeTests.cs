using System.Collections.Generic;
using System.IO ;
using System.Linq;
using Fclp;
using Guartinel.CLI.Utility.Commands;
using Guartinel.Core ;
using NUnit.Framework;

namespace Guartinel.CLI.Utility.Tests.Files {
   [TestFixture]
   public class FileSizeTests : FileTestsBase {
      [Test]
      public void WriteFiles_CheckSize() {
         // First, get the appropriate command
         int fileSize = 100 ;

         WriteTestFile (null, "testfile.size", fileSize) ;

         var result = RunCommand (Path.Combine (_testFolder, "testfile.size"), fileSize + 1) ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         // Invalid filename should be OK
         result = RunCommand (Path.Combine (_testFolder, "testfile.sizeX"), fileSize + 1) ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         // Pattern OK
         result = RunCommand (Path.Combine (_testFolder, "*.size"), fileSize + 1) ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         // Pattern not OK
         result = RunCommand (Path.Combine (_testFolder, "*.size"), fileSize - 1) ;
         Assert.IsFalse (result.Success, result.ToString()) ;
      }

      private CheckResult RunCommand (string pattern,
                                     int maxSize) {
         ICommand checkFileSize = IoC.Use.GetAllInstances<ICommand>().First (x => x.Command == "checkFileSize") ;
         FluentCommandLineParser parser = new FluentCommandLineParser() ;
         checkFileSize.Setup (parser) ;

         var arguments = CreateArguments (pattern, maxSize) ;
         parser.Parse (arguments.ToArray()) ;
         return checkFileSize.Run()[0] ;
      }

      private List<string> CreateArguments (string pattern,
                                            int maxSize) {
         List<string> arguments = new List<string>() ;
         arguments.Add ("--command:checkFileSize") ;
         arguments.Add ($"--pattern:\"{pattern}\"") ;
         arguments.Add ($"--maxSize:{maxSize}") ;
         return arguments ;
      }
   }
}
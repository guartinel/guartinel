using System.Collections.Generic;
using System.Linq;
using Fclp;
using Fclp.Internals.Extensions;
using Guartinel.CLI.Utility.Commands;
using Guartinel.CLI.Utility.Files ;
using Guartinel.Core ;
using NUnit.Framework;

namespace Guartinel.CLI.Utility.Tests.Files {
   [TestFixture]
   public class FolderSizeTests : FileTestsBase {
      public void WriteFilesCheckSize (FolderSizeChecker.SizeUnit sizeUnit) {
         // First, get the appropriate command
         int fileSize = 5 ;
         int fileCount = 3 ;
         int folderCount = 2 ;

         for (int folderIndex = 0; folderIndex < folderCount; folderIndex++) {
            for (int fileIndex = 0; fileIndex < fileCount; fileIndex++) {
               WriteTestFile ($"sub{folderIndex}", $"testfile.{fileIndex}", (int) FolderSizeChecker.ConvertToBytes (fileSize, sizeUnit)) ;
            }
         }

         var result = RunCommand (folderCount * fileCount * fileSize + 1, sizeUnit) ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (folderCount * fileCount * fileSize - 1, sizeUnit) ;
         Assert.IsFalse (result.Success, result.ToString()) ;

         result = RunCommand (folderCount * 1 * fileSize + 1, sizeUnit, "*.1") ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (folderCount * 1 * fileSize - 1, sizeUnit, "*.2") ;
         Assert.IsFalse (result.Success, result.ToString()) ;
      }

      [Test]
      public void WriteFiles_CheckSize() {
         WriteFilesCheckSize (FolderSizeChecker.SizeUnit.Byte) ;
         WriteFilesCheckSize (FolderSizeChecker.SizeUnit.MB) ;
      }

      private CheckResult RunCommand (double maxSize,
                                      FolderSizeChecker.SizeUnit sizeUnit,
                                      string pattern = null) {
         ICommand checkFolderSize = IoC.Use.GetAllInstances<ICommand>().First (x => x.Command == "checkFolderSize") ;
         FluentCommandLineParser parser = new FluentCommandLineParser() ;
         checkFolderSize.Setup (parser) ;

         var arguments = CreateArguments (maxSize, sizeUnit, pattern) ;
         parser.Parse (arguments.ToArray()) ;
         return checkFolderSize.Run()[0] ;
      }

      private List<string> CreateArguments (double maxSize,
                                            FolderSizeChecker.SizeUnit sizeUnit,
                                            string pattern = null) {
         List<string> arguments = new List<string>() ;
         arguments.Add ("--command:checkFolderSize") ;
         arguments.Add ($"--folder:{_testFolder.WrapInDoubleQuotesIfContainsWhitespace()}") ;

         if (!string.IsNullOrEmpty (pattern)) {
            arguments.Add ($"--include:\"{pattern}\"") ;
         }
         arguments.Add ($"--maxSize:{maxSize}") ;
         arguments.Add ($"--maxSizeUnit:{sizeUnit.ToString().ToLowerInvariant()}") ;

         return arguments ;
      }
   }
}
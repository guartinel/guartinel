using System;
using System.IO;
using System.Linq ;
using System.Text ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Files {
   public abstract class FileTestsBase : TestsBase {
      protected string _testFolder ;

      protected override void SetUp() {
         base.SetUp() ;

         _testFolder = Path.Combine (Kernel.Utility.AssemblyEx.GetAssemblyPath<Program>(), Guid.NewGuid().ToString()) ;
         Directory.CreateDirectory (_testFolder) ;
      }

      protected override void TearDown() {
         try {
            if (Directory.Exists (_testFolder)) {
               Directory.Delete (_testFolder, true) ;
            }
         } catch { }
         
         base.TearDown() ;
      }

      protected void ClearTestFolder() {
         DirectoryInfo directory = new DirectoryInfo (_testFolder) ;

         foreach (FileInfo file in directory.GetFiles()) {
            file.Delete() ;
         }
         foreach (DirectoryInfo subDirectory in directory.GetDirectories()) {
            subDirectory.Delete (true) ;
         }
      }

      protected void WriteTestFile (string subFolder,
                                    string fileName,
                                    int fileSize,
                                    int fileAgeInSeconds = 0) {
         string folderName = string.IsNullOrEmpty (subFolder) ? _testFolder : Path.Combine (_testFolder, subFolder) ;
         Directory.CreateDirectory (folderName) ;
         string fullFileName = Path.Combine (folderName, fileName) ;

         const int BUFFER_SIZE = 1000 ;

         byte[] buffer = Encoding.ASCII.GetBytes (new string ('X', BUFFER_SIZE)) ;
         int writtenBytes = 0 ;

         using (var stream = File.OpenWrite (fullFileName)) {
            while (writtenBytes < fileSize) {
               int bytesToWrite = Math.Min (fileSize - writtenBytes, BUFFER_SIZE) ;
               stream.Write (buffer, 0, bytesToWrite) ;
               writtenBytes = writtenBytes + bytesToWrite ;
            }
            stream.Flush (true) ;
            stream.Close() ;
         }

         if (fileAgeInSeconds > 0) {
            File.SetLastWriteTime (fullFileName, DateTime.UtcNow.AddSeconds (-fileAgeInSeconds)) ;
         }
      }
   }
}
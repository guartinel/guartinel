using NUnit.Framework;
using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Reflection ;

namespace Guartinel.ManagementServer.APITests.BenchmarkTests {
   [TestFixture]
   public class PilotTest : TestsBase {

      public class Constanst {
         public const string BENCHMARK_USER = "benchmark@guartinel.com" ;
      }

      // [Test]
      public void StopTest() {
         DeleteAccount (Constanst.BENCHMARK_USER, Constants.LOGIN_PASSWORD) ;
      }

      // [Test]
      public void StartTest() {
         const int PACKAGE_COUNT = 10 ;
         const int WEBSITE_COUNT_PER_PACKAGE = 99 ;
         const int CHECK_INTERVAL_SECONDS = 180 ;

         CreateAccount (Constanst.BENCHMARK_USER, Constants.LOGIN_PASSWORD) ;
         var token = GetToken (Constanst.BENCHMARK_USER) ;
         var originalPackages = GetAllPackages (token) ;
         List<string> websitesFromCSV = GetCSVFileDataByColumn(@"Assets\Websites.10000.csv", "URL").GetRange(0, 1000);

         for (int packageIndex = 0; packageIndex < PACKAGE_COUNT; packageIndex++) {
            var packageName = Guid.NewGuid().ToString();

            var websites = websitesFromCSV.GetRange (packageIndex * WEBSITE_COUNT_PER_PACKAGE, WEBSITE_COUNT_PER_PACKAGE) ;

            SaveWebsiteSupervisorPackage (token, packageName, websites, null, null, CHECK_INTERVAL_SECONDS, true) ;
         }

         var packages = GetAllPackages (token) ;

         Assert.AreEqual (originalPackages.Count + PACKAGE_COUNT, packages.Count) ;
      }

      private List<string> GetCSVFileDataByColumn (string file,
                                                   string columnName) {
         List<string> result = new List<string>() ;


         // using (TextFieldParser csvParser = new TextFieldParser (Path.Combine (TestContext.CurrentContext.WorkDirectory, file))) {
         using (TextFieldParser csvParser = new TextFieldParser (Path.Combine(Path.GetDirectoryName (Assembly.GetExecutingAssembly().Location), file))) {
            csvParser.CommentTokens = new string[] {"#"} ;
            csvParser.SetDelimiters (new string[] {","}) ;
            csvParser.HasFieldsEnclosedInQuotes = true ;

            // Skip the row with the column names
            string header = csvParser.ReadLine() ;
            string[] columns = header.Split (',') ;
            int columnIndex = -1 ;
            for (int index = 0; index < columns.Length; index++) {
               if (columns [index].ToLower().Contains (columnName.ToLower())) {
                  columnIndex = index ;
                  break ;
               }
            }

            if (columnIndex == -1) {
               throw new Exception ("Cannot found column: " + columnName) ;
            }

            while (!csvParser.EndOfData) {
               // Read current line fields, pointer moves to the next line.
               string[] fields = csvParser.ReadFields() ;
               result.Add (fields [columnIndex]) ;
            }
         }

         return result ;
      }
   }
}
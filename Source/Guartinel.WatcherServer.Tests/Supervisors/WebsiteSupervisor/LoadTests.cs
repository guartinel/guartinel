using System ;
using System.IO ;
using System.Linq ;
using System.Reflection ;
using System.Text ;
using Guartinel.Service.WebsiteChecker ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Supervisors.WebsiteSupervisor {
   // [TestFixture]
   public class LoadPackageTests : PackageTestsBase {
      // [Test]
      public void PilotTests_100() {
         const int WEBSITE_COUNT = 100 ;
         ApplicationSettings.Use.WebsiteChecker = nameof (Chrome) ;
         StartServer() ;

         var token = Login() ;

         var fileName = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly().Location), @"Assets\Websites.500.txt") ;
         var websiteList = File.ReadAllLines (fileName).ToList().GetRange (0, WEBSITE_COUNT) ;
         var websites = websiteList.Select (x => new Website (x)).ToList() ;
         var packageID = Guid.NewGuid().ToString() ;

         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, websites, 20),
                      Guid.NewGuid().ToString(), packageID, false, 10000, 15000, 1, false) ;

         new Kernel.Timeout (TimeSpan.FromSeconds (100)).WaitFor (() => ManagementServer.MeasuredDataList.Count >= WEBSITE_COUNT) ;
         Assert.GreaterOrEqual (ManagementServer.MeasuredDataList.Count, WEBSITE_COUNT) ;

         foreach (var website in websites) {
            Assert.IsTrue (ManagementServer.MeasuredDataList.Any (x => x.PackageID == packageID && x.Data.Contains (website.Address)),
                           $"Website {website.Address} was not checked.") ;
         }
      }
   }
}
using System;
using System.Linq;
using System.Text;
using System.Threading ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   [TestFixture]
   public class SystemTests {
      [Test]
      public void TestStringInterpolation() {
         string basePath = @"c:\temp" ;
         string additionalPath = @"subfolder1\subfolder2\" ;
         Assert.AreEqual (@"c:\temp\subfolder1\subfolder2\", $@"{basePath}\{additionalPath}") ;

         var protocol = @"http://" ;
         var url = @"www.sysment.hu" ;
         Assert.AreEqual (@"http://www.sysment.hu", $@"{protocol}{url}") ;

         protocol = @"http:" ;
         url = @"www.sysment.hu" ;
         Assert.AreEqual (@"http://www.sysment.hu", $@"{protocol}//{url}") ;

         var test1 = @"//\\//\\" ;
         var test2 = $"x1x{test1}x2x" ;
         Assert.AreEqual (@"x1x//\\//\\x2x", test2) ;
      }

      private static void CheckIPEndPoint (Pinger.PingTarget target,
                                           Host host,
                                           int port) {
         Assert.AreEqual (host.Address, target.Host.Address) ;
         Assert.AreEqual (port, target.Port) ;
      }

      [Test]
      public void TestIPEndpointParsing() {
         CheckIPEndPoint (Pinger.ParseAddress (new Host ("www.sysment.hu:2345")), new Host ("www.sysment.hu"), 2345) ;
         CheckIPEndPoint (Pinger.ParseAddress (new Host ("www.sysment.hu")), new Host ("www.sysment.hu"), 0) ;
         CheckIPEndPoint (Pinger.ParseAddress (new Host ("192.168.12.45:2345")), new Host ("192.168.12.45"), 2345) ;
      }

      [Test]
      public void TestUriEndingSlash() {
         var uri = new Uri (@"http://sysment.hu/test/ui") ;
         Assert.AreEqual (@"http://sysment.hu/test/ui/", uri.ToString().EnsureTrailingSlash()) ;

         var uri2 = new Uri (@"http://sysment.hu/test/ui/") ;
         Assert.AreEqual (@"http://sysment.hu/test/ui/", uri2.ToString().EnsureTrailingSlash()) ;
      }

      public class LazyClass1 {
         public static bool ThrowException = false;
         public static int ContructorCallCount = 0 ;
         public LazyClass1() {
            ContructorCallCount++ ;
            if (ThrowException) {
               throw new Exception("Error1.");
            }            
         }

         public string Message1 => "ehune1" ;
         public string Message2 => "ehune2" ;
      }

      [Test]
      public void TestExceptionInLazyConstructor() {
         var lazy1 = new Lazy<LazyClass1>(() => new LazyClass1(), LazyThreadSafetyMode.PublicationOnly) ;
         LazyClass1.ContructorCallCount = 0 ;
         try {
            LazyClass1.ThrowException = true ;
            var x = lazy1.Value.Message1 ;
         } catch (Exception) {

         }

         Assert.AreEqual(1, LazyClass1.ContructorCallCount);

         try {
            LazyClass1.ThrowException = false;
            var x = lazy1.Value.Message2 ;
         } catch (Exception) {

         }
         Assert.AreEqual(2, LazyClass1.ContructorCallCount);
      }
   }
}
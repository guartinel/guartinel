using System ;
using System.Linq ;
using System.Security.Cryptography ;
using System.Text ;
using System.Threading ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   [TestFixture]
   public class TestSemaphor {
      [Test]
      public void CreateSemaphor_Abandon_Check() {
         ThreadSemaphor semaphor = new ThreadSemaphor() ;
         ThreadSemaphor.Flag flag1 = semaphor.GetFlag ;
         semaphor.DisableAndAbandonFlag();
         ThreadSemaphor.Flag flag2 = semaphor.GetFlag ;
         
         Assert.IsFalse (flag1.IsEnabled) ;
         Assert.IsTrue (flag2.IsEnabled) ;         
      }

      [Test]
      public void CreateSemaphor_UseItInManyThreads_Abandon_CheckIfThreadsStopped() {
         const int NUMBER_OF_THREADS = 100 ;

         ThreadSemaphor semaphor1 = new ThreadSemaphor() ;
         ThreadSemaphor.Flag flag1 = semaphor1.GetFlag ;
         ThreadSemaphor semaphor2 = new ThreadSemaphor() ;
         ThreadSemaphor.Flag flag2 = semaphor2.GetFlag ;
         int finishedThreads = 0 ;

         for (int index = 0; index < NUMBER_OF_THREADS; index++) {
            new Thread (() => {
               while (flag1.IsEnabled) {
                  // Just sleep some time
                  Thread.Sleep (new Random().Next (100)) ;
               }
               
               // Indicate that thread is finished
               flag2.RunLocked (() => {
                  finishedThreads++ ;
               }) ;
            }).Start() ;
         }
         semaphor1.DisableAndAbandonFlag() ;

         var endTime = DateTime.UtcNow.AddSeconds (20) ;
         while (DateTime.UtcNow < endTime) {
            if (finishedThreads == NUMBER_OF_THREADS) break ;
            
            Thread.Sleep (100) ;
         }
         
         // Check number of finished threads
         Assert.AreEqual (NUMBER_OF_THREADS, finishedThreads) ;
      }
   }
}

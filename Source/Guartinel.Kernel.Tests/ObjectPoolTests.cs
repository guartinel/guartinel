using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using System.Threading.Tasks ;
using Guartinel.Kernel.Logging ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   [TestFixture]
   internal class ObjectPoolTests {
      public class Test1 {
         public string UID = Guid.NewGuid().ToString() ;
      }

      [SetUp]
      public void Setup() {
         Logger.RegisterLogger<ConsoleLogger> ();
      }

      [Test]
      public void TestNumberOfObjectsWithThreads() {
         const int THREAD_COUNT = 10;
         ObjectPool<Test1> pool = new ObjectPool<Test1>(() => new Test1(), 5);

         int finishedThreads = 0 ;

         void StartThreads () {
            finishedThreads = 0 ;
            for (int threadIndex = 0; threadIndex < THREAD_COUNT; threadIndex++) {
               new Thread (() => {
                  Logger.Debug ($"Thread started, asking for object...") ;
                  Thread.Sleep (200) ;

                  pool.Use (x => {
                     Logger.Debug($"Got object from pool {x.UID}.") ;

                     Thread.Sleep (2000) ;
                     Logger.Debug($"Release object into pool {x.UID}.") ;
                     Interlocked.Increment (ref finishedThreads) ;
                  }) ;
               }).Start() ;
            }
         }

         Assert.AreEqual (0, pool.ObjectCount) ;         

         StartThreads() ;

         new Timeout().WaitFor(() => finishedThreads >= THREAD_COUNT) ;

         Assert.AreEqual (5, pool.ObjectCount) ;

         // Decrease the number of pool
         pool.MaxObjectCount = 3 ;

         StartThreads();

         new Timeout().WaitFor(() => finishedThreads >= THREAD_COUNT);

         Assert.AreEqual(3, pool.ObjectCount);
      }

      [Test]
      public void TestNumberOfObjectsWithTasks () {
         const int TASK_COUNT = 10;
         ObjectPool<Test1> pool = new ObjectPool<Test1>(() => new Test1(), 5);

         int finishedTasks = 0;

         void StartTasks() {
            finishedTasks = 0 ;
            Parallel.For (0, TASK_COUNT, taskIndex => {
                             new Task (() => {
                                Logger.Debug ($"Task started, asking for object...") ;
                                Thread.Sleep (200) ;

                                pool.Use (x => {
                                   Logger.Debug ($"Got object from pool {x.UID}.") ;

                                   Thread.Sleep (2000) ;
                                   Logger.Debug ($"Release object into pool {x.UID}.") ;
                                   Interlocked.Increment (ref finishedTasks) ;
                                }) ;
                             }).Start() ;
                          }
                         ) ;
         }

         Assert.AreEqual(0, pool.ObjectCount);

         StartTasks();

         new Timeout().WaitFor(() => finishedTasks >= TASK_COUNT);

         Assert.AreEqual(5, pool.ObjectCount);

         // Decrease the number of pool
         pool.MaxObjectCount = 3;

         StartTasks();

         new Timeout().WaitFor(() => finishedTasks >= TASK_COUNT);

         Assert.AreEqual(3, pool.ObjectCount);
      }
   }
}

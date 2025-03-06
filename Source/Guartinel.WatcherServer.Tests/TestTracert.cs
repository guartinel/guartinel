using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Guartinel.WatcherServer.Tests {
   [TestFixture]
   class TestTracert {
      [Test]
      public void SimpleTest () {
        string result = GetTrace("index.hu");
        Assert.IsTrue (result.Contains ("Trace complete."),"Trace is not successfull.");
      }
      public string GetTrace (string ipAddressOrHostName) {
         IPHostEntry rootDomainEntry = Dns.GetHostEntry (ipAddressOrHostName) ; // this can throw exception if domain cannot be resolved
         IPAddress ipAddress = rootDomainEntry.AddressList[0];
         StringBuilder traceResults = new StringBuilder();

         using ( Ping pingSender = new Ping() ) {
            PingOptions pingOptions = new PingOptions();
            Stopwatch stopWatch = new Stopwatch();
            pingOptions.DontFragment = true;
            pingOptions.Ttl = 1;// we use TTL to check every hop between us and the final destination of the host. To check the first hop we mus specify TTL 1

            int maxHops = 30;
            traceResults.AppendLine($"Tracing route to {ipAddress} ({rootDomainEntry.HostName}) over a maximum of {maxHops} hops:");
            traceResults.AppendLine();
            for ( int index = 1; index <= maxHops; index++ ) {
               stopWatch.Reset();
               stopWatch.Start();
               PingReply pingReply = pingSender.Send(ipAddress, 5000, new byte[32], pingOptions);
               IPHostEntry domainEntry = null;
               try {
                  domainEntry = Dns.GetHostEntry(pingReply.Address);
               } catch ( Exception e ) {}

               stopWatch.Stop();
               string hostName = domainEntry == null ? "Cannot resolve domain" : domainEntry.HostName;

               if ( pingReply.Status == IPStatus.Success ) { // we have reached the destination 
                  traceResults.AppendLine($"{index}\t{stopWatch.ElapsedMilliseconds} ms\t{pingReply.Address}\t({hostName})");
                  traceResults.AppendLine();
                  traceResults.AppendLine("Trace complete.");
                  break;
               }else if ( pingReply.Status == IPStatus.TtlExpired ) { // current hop is reached so add the results and check the next hop
                  traceResults.AppendLine($"{index}\t{stopWatch.ElapsedMilliseconds} ms\t{pingReply.Address}\t({hostName})");
               }
               else { // something other thing happened add the status to the result
                  traceResults.AppendLine($"{index}\t{stopWatch.ElapsedMilliseconds} ms\t{pingReply.Address}\t({hostName})\tStatus:{pingReply.Status}");
               }
               pingOptions.Ttl++;
            }
         }
          return traceResults.ToString();
      }

   }
}

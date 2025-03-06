using System ;
using System.Diagnostics ;
using System.Net ;
using System.Net.NetworkInformation ;
using System.Net.Sockets ;
using System.Text ;
using System.Threading ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Kernel.Network {
   public enum PingSuccess {
      Success,
      Fail
   }

   public class PingResult {
      private PingResult (PingSuccess success,
                          string host,
                          long roundtripMilliseconds,
                          XString message,
                          XString details) {
         Success = success ;
         Host = host ;
         RoundtripMilliseconds = roundtripMilliseconds ;
         Message = message ;
         Details = details ;
      }

      public PingResult (PingSuccess success,
                         string host) : this (success, host, 0, new XSimpleString(), new XSimpleString()) {}

      public PingResult (string host,
                         long roundtripMilliseconds) : this (PingSuccess.Success, host, roundtripMilliseconds, new XSimpleString(), new XSimpleString()) {}

      public PingResult (string host,
                         long roundtripMilliseconds,
                         XString message) : this (PingSuccess.Success, host, roundtripMilliseconds, message, null) {}

      public PingResult (PingSuccess success,
                         string host,
                         XString message) : this (success, host, 0, message, null) {}

      public PingSuccess Success {get ;}

      public long RoundtripMilliseconds {get ;}

      public XString Message {get ;}
      public XString Details { get; }

      public string Host { get ;}
   }

   public class Pinger {
      public static class Constants {
         public const int NO_PORT_SPECIFIED = 0 ;

         public const char PORT_SEPARATOR = ':' ;
         public const char IP_ADDRESS_SEPARATOR = '.' ;

         public const int TRACE_MAX_HOPS = 10;
         public const int TRACE_TIMEOUT_MILLISECONDS = 100 ;

         public const string TCP_PREFIX = @"tcp://" ;
      }

      public class PingTarget {
         public PingTarget (Host host,
                            int port) {
            Host = host ;
            Port = port ;
         }
         
         public Host Host;
         public int Port ;
         public bool IsPortSpecified => Port > Constants.NO_PORT_SPECIFIED ;

         public override string ToString () {
            var portString = IsPortSpecified ? $"{Constants.PORT_SEPARATOR}{Port}" : string.Empty ;
            return $"{Host.DisplayText}{portString}";

         }
      }

      public static PingTarget ParseAddress (Host host) {
         Uri uri ;

         if (Uri.TryCreate (host.Address, UriKind.Absolute, out uri)) {
            if (!string.IsNullOrEmpty (uri.Host)) {
               return new PingTarget (new Host (uri.Host, host.Caption), Math.Max (uri.Port, Constants.NO_PORT_SPECIFIED)) ;
            }
         }

         if (Uri.TryCreate (string.Concat (Constants.TCP_PREFIX, host.Address), UriKind.Absolute, out uri)) {
            if (!string.IsNullOrEmpty (uri.Host)) {
               return new PingTarget (new Host (uri.Host, host.Caption), Math.Max (uri.Port, Constants.NO_PORT_SPECIFIED)) ;
            }
         }

         if (Uri.TryCreate (string.Concat (Constants.TCP_PREFIX, string.Concat ("[", host.Address, "]")), UriKind.Absolute, out uri)) {
            if (!string.IsNullOrEmpty (uri.Host)) {
               return new PingTarget (new Host(uri.Host, host.Caption), Math.Max (uri.Port, Constants.NO_PORT_SPECIFIED)) ;
            }
         }

         return null ;
      }

      //private static IPEndPoint ParseIpAddress (string value) {
      //   if (string.IsNullOrEmpty (value)) return null ;

      //   // Cut off port number
      //   var ipEndPointParts = value.Split (new[] {Constants.PORT_SEPARATOR}, StringSplitOptions.RemoveEmptyEntries) ;
      //   if ((ipEndPointParts.Length < 1)) return null ;

      //   IPAddress ipAddress ;
      //   if (!IPAddress.TryParse (ipEndPointParts [0], out ipAddress)) return null ;

      //   // Port specified
      //   if (ipEndPointParts.Length == 2) {
      //      int port ;
      //      if (!int.TryParse (ipEndPointParts [1], out port)) return null ;
      //      return new IPEndPoint (ipAddress, port) ;
      //   }

      //   // No port
      //   return new IPEndPoint (ipAddress, Constants.NO_PORT_SPECIFIED) ;
      //}

      private PingResult TestDns() {
         // DNS error?
         return Ping(new Host("sysment.hu", "Sysment"), 1, false, 0, 4) ;
      }

      public PingResult Ping (Host host,
                              int tryCount,
                              bool dummy,
                              int retryWaitSeconds,
                              int timeoutSeconds,
                              Action<string> logError = null) {
         XString message = new XSimpleString() ;

         //32 byte buffer (create empty)
         var buffer = new byte[32] ;

         // Logger.Log ($"Ping initiated. Host: {host}, IP: {ipAddress}. Retry count: {RetryCount}. Wait time {WaitTimeSeconds} seconds.") ;

         // IPAddress instance for holding the returned host
         PingTarget target = ParseAddress (host) ;
         if (target == null) {
            logError?.Invoke ($"Ping: invalid address '{host}'.") ;
            return new PingResult (PingSuccess.Fail,
                                   host.DisplayText,
                                   new XSimpleString ($"Invalid address '{host.Address}' specified.")) ;
         }

         bool noTraceNeeded = false ;

         // Try until the first success or too many
         for (var tryIndex = 1; tryIndex <= tryCount; tryIndex++) {
            noTraceNeeded = false ;
            
            try {
               if (!target.IsPortSpecified) {
                  try {
                     // Use ping
                     // Set the ping options, TTL 128
                     var pingOptions = new PingOptions (128, true) ;

                     // Create a new ping instance
                     using (var ping = new Ping()) {
                        // The Send() method expects 4 items:
                        //1) The IPAddress we are pinging
                        //2) The timeout value
                        //3) A buffer (our byte array)
                        //4) PingOptions
                        var pingReply = ping.Send (target.Host.Address, TimeSpan.FromSeconds (timeoutSeconds).AllMilliseconds(), buffer, pingOptions) ;

                        // make sure we dont have a null reply
                        if (pingReply != null) {
                           switch (pingReply.Status) {
                              case IPStatus.Success:
                                 // string.Format ("Reply from {0}: bytes={1} time={2}ms TTL={3}", pingReply.Address, pingReply.Buffer.Length, pingReply.RoundtripTime, pingReply.Options.Ttl) ;
                                 #warning SzTZ: use XString codes here
                                 message = new XSimpleString ($"The device on address '{host}' is accessible in {pingReply.RoundtripTime} milliseconds.") ;
                                 return new PingResult (target.ToString(),
                                                        pingReply.RoundtripTime,
                                                        message) ;
                              case IPStatus.TimedOut:
                              case IPStatus.DestinationHostUnreachable:
                              case IPStatus.DestinationNetworkUnreachable:
                                 logError?.Invoke($"Error when ping {host}. Status code: '{pingReply.Status}'.");
                                 #warning SzTZ: use XString codes here                                    
                                 message = new XSimpleString ($"The device on address '{host}' is not accessible.") ;
                                 break ;

                              default:
                                 logError?.Invoke($"Error when ping {host}. Status code: '{pingReply.Status}'.");
                                 #warning SzTZ: use XString codes here                                 
                                 message = new XSimpleString ($"The device on address '{host}' is not accessible. Status code: '{pingReply.Status.ToString().SeparateCamelCase()}'.") ;
                                 break ;
                           }
                        }
                     }
                  } catch (PingException e) {
                     if (e.InnerException is SocketException &&
                         (e.InnerException as SocketException).SocketErrorCode == SocketError.HostNotFound) {

                        // DNS error?
                        if (TestDns().Success == PingSuccess.Success) {
                           // Host is unknown
                           logError?.Invoke ($"Host '{host}' not found. Message: {e.Message}") ;
                           message = new XSimpleString ($"Host '{host}' not found.") ;
                           noTraceNeeded = true ;
                        } else {
                           // No DNS
                           logError?.Invoke ($"DNS unavailable '{host}'. Message: {e.Message}") ;
                           message = new XSimpleString ($"DNS service error. {e.Message}") ;
                        }
                     } else {
                        logError?.Invoke($"Exception when trying to ping '{host}'. Message: {e.GetAllMessages()}");
#warning SzTZ: use XString codes here
                        message = new XSimpleString($"Error when trying to ping '{host}'. {e.Message}");
                     }
                  } catch (SocketException e) {
                     logError?.Invoke ($"Socket exception when trying to ping '{host}'. Code: {e.ErrorCode}. Socket error code: '{e.SocketErrorCode.ToString().SeparateCamelCase()}'. Message: {e.GetAllMessages()}") ;
                     #warning SzTZ: use XString codes here
                     message = new XSimpleString ($"Socket error when trying to ping '{host}'. {e.Message}") ;
                  }
               } else {
                  // Use Tcp
                  using (TcpClient tcpClient = new TcpClient()) {
                     try {
                        tcpClient.SendTimeout = TimeSpan.FromSeconds (timeoutSeconds).AllMilliseconds() ;
                        Stopwatch stopwatch = new Stopwatch() ;
                        stopwatch.Start() ;
                        try {
                           tcpClient.Connect (target.Host.Address, target.Port) ;
                        } finally {
                           tcpClient.Close() ;
                        }

                        var elapsed = stopwatch.ElapsedMilliseconds ;

                        message = new XSimpleString ($"The device on address '{host}' is accessible in {elapsed} milliseconds.") ;
                        return new PingResult (target.ToString(), elapsed, message) ;
                     } catch (SocketException e) {
                        if (e.SocketErrorCode == SocketError.HostNotFound) {
                           // DNS error?
                           if (TestDns().Success == PingSuccess.Success) {
                              // Host is unknown
                              logError?.Invoke($"Host '{host}' not found. Message: {e.Message}");
                              message = new XSimpleString($"Host '{host}' not found.");
                              noTraceNeeded = true ;
                           } else {
                              // No DNS
                              logError?.Invoke($"DNS unavailable '{host}'. Message: {e.Message}");
                              message = new XSimpleString($"DNS service error. {e.Message}");
                           }
                        }
                     } catch (Exception e) {
                        logError?.Invoke ($"Exception when trying to Tcp ping '{host}'. Message: {e.Message}") ;
                        message = new XSimpleString ($"Ping failed. {e.Message}") ;
                     }
                  }
               }
               // messages.Add (new XSimpleString (("Ping failed for an unknown reason."))) ;
            } catch (Exception e) {
               logError?.Invoke ($"Generic error when trying to ping {host}. Type: {e.GetType().Name}. Message: {e.Message}") ;
               message = new XSimpleString ($"Generic error when trying to ping '{host}'. {e.Message}") ;
            }

            if (tryIndex < tryCount) {
               Thread.Sleep (TimeSpan.FromSeconds (retryWaitSeconds)) ;
            }
         }

         logError?.Invoke ($"Cannot ping host due: {message}") ;
         // Try to trace
         if (!target.IsPortSpecified && !noTraceNeeded) {
            string trace = GetTrace (host.Address) ;
            message = new XStrings (message, new XSimpleString(" "), new XSimpleString (trace)) ;
         }

         return new PingResult (PingSuccess.Fail, target.ToString(), message) ;
      }

      public static string GetTrace (string ipAddressOrHostName) {
         try {
            IPHostEntry rootDomainEntry = Dns.GetHostEntry (ipAddressOrHostName) ; // this can throw exception if domain cannot be resolved
            IPAddress ipAddress = rootDomainEntry.AddressList [0] ;
            StringBuilder traceResults = new StringBuilder() ;

            using (Ping pingSender = new Ping()) {
               PingOptions pingOptions = new PingOptions() ;
               Stopwatch stopWatch = new Stopwatch() ;
               pingOptions.DontFragment = true ;
               pingOptions.Ttl = 1 ; // we use TTL to check every hop between us and the final destination of the host. To check the first hop we must specify TTL 1

               int maxHops = Constants.TRACE_MAX_HOPS ;
               traceResults.AppendLine ($"Tracing route to {ipAddress} ({rootDomainEntry.HostName}) over a maximum of {maxHops} hops:") ;
               traceResults.AppendLine() ;
               for (int index = 1; index <= maxHops; index++) {
                  stopWatch.Reset() ;
                  stopWatch.Start() ;
                  PingReply pingReply = pingSender.Send (ipAddress, Constants.TRACE_TIMEOUT_MILLISECONDS, new byte[32], pingOptions) ;
                  stopWatch.Stop() ;

                  if (pingReply != null) {
                     if (pingReply.Address != null) {
                        IPHostEntry domainEntry = null ;
                        try {
                           domainEntry = Dns.GetHostEntry (pingReply.Address) ;
                        } catch (Exception) { }

                        string hostName = domainEntry == null ? "Cannot resolve domain" : domainEntry.HostName ;

                        if (pingReply.Status == IPStatus.Success) {
                           // we have reached the destination 
                           traceResults.AppendLine ($"{index}\t{stopWatch.ElapsedMilliseconds} ms\t{pingReply.Address}\t({hostName})") ;
                           traceResults.AppendLine() ;
                           traceResults.AppendLine ("Trace complete.") ;
                           break ;
                        } else if (pingReply.Status == IPStatus.TtlExpired) {
                           // current hop is reached so add the results and check the next hop
                           traceResults.AppendLine ($"{index}\t{stopWatch.ElapsedMilliseconds} ms\t{pingReply.Address}\t({hostName})") ;
                        } else {
                           // something other thing happened add the status to the result
                           traceResults.AppendLine ($"{index}\t{stopWatch.ElapsedMilliseconds} ms\t{pingReply.Address}\t({hostName})\tStatus:{pingReply.Status}") ;
                        }
                     }
                  }

                  pingOptions.Ttl++ ;
               }
            }

            return traceResults.ToString() ;
         } catch (Exception e) {
            return $"Trace error. {e.Message}" ;
         }
      }
   }
}
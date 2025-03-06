using System;
using System.Collections.Generic;
using System.IO ;
using System.Net ;
using System.Net.NetworkInformation ;
using System.Net.Sockets ;
using System.Text;

namespace Guartinel.Kernel.Network {
   public static class Utility {
      //public static List<string> GetLocalIPv4Addresses() {
      //   List<string> result = new List<string>() ;

      //   foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) {
      //      if (item.OperationalState == OperationalStatus.Up) {
      //         foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses) {
      //            if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
      //               result.Add (ip.Address.ToString()) ;
      //            }
      //         }
      //      }
      //   }

      //   return result ;
      //}

      private static string _cachedExternalIPv4Address = string.Empty ;

      private static string GetIPFromIpify() {
         const string URL = "https://api.ipify.org/" ;

         HttpWebRequest request = WebRequest.Create (URL) as HttpWebRequest ;
         if (request == null) { return string.Empty ; }
         using (HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
            Stream responseStream = response.GetResponseStream() ;
            if (responseStream == null) { return string.Empty ; }

            using (StreamReader reader = new StreamReader (responseStream, Encoding.UTF8)) {
               return reader.ReadToEnd() ;
            }
         }
      }

      public static string GetExternalIPv4Address() {
         if (string.IsNullOrEmpty (_cachedExternalIPv4Address)) {
            _cachedExternalIPv4Address = GetIPFromIpify() ;
         }

         return _cachedExternalIPv4Address ;
      }

      public static string GetLocalIPv4Address (NetworkInterfaceType networkInterfaceType,
                                                bool excludeDhcp) {
         foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces()) {
            if ((networkInterface.NetworkInterfaceType == networkInterfaceType) && (networkInterface.OperationalStatus == OperationalStatus.Up)) {
               foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses) {
                  if (excludeDhcp && (ip.PrefixOrigin == PrefixOrigin.Dhcp)) { continue ; }
                  if (ip.Address.AddressFamily != AddressFamily.InterNetwork) { continue ; }
                  return ip.Address.ToString() ;
               }
            }
         }
         return String.Empty ;
      }

      public static string GetLocalFixIPv4Address() {
         return GetLocalIPv4Address (NetworkInterfaceType.Ethernet, true) ;
      }

      public static string GetLocalIPv4Address (NetworkInterfaceType networkInterfaceType) {
         return GetLocalIPv4Address (networkInterfaceType, false) ;
      }

      public static bool IsValidIP4Address (string address) {
         IPAddress ipAddress ;
         if (IPAddress.TryParse (address, out ipAddress)) {
            switch (ipAddress.AddressFamily) {
               case AddressFamily.InterNetwork:
                  // we have IPv4
                  return true ;
               case AddressFamily.InterNetworkV6:
                  // we have IPv6
                  return false ;
            }
         }

         return false ;
      }
   }
}

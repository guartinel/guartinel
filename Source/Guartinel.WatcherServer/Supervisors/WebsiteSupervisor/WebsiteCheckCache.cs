using System;
using System.Collections.Concurrent ;
using System.Linq;
using System.Text;

namespace Guartinel.WatcherServer.Supervisors.WebsiteSupervisor {
   public static class WebsiteCheckCache {
      public class Item {
         public Item (string url,
                      DateTime checkedAt,
                      long roundtripMilliseconds,
                      string content,
                      DateTime? certificateExpiryDate) {

            Url = url ;
            CheckedAt = checkedAt ;
            RoundtripMilliseconds = roundtripMilliseconds ;
            Content = content ;
            CertificateExpiryDate = certificateExpiryDate ;
         }

         public readonly string Url ;
         public readonly DateTime CheckedAt ;
         public readonly long RoundtripMilliseconds ;
         public readonly string Content ;
         public readonly DateTime? CertificateExpiryDate ;
      }

      private static readonly ConcurrentDictionary<string, Item> _cache = new ConcurrentDictionary<string, Item>() ;

      public static void Register (string url,
                                               long roundtripMilliseconds,
                                               string content,
                                               DateTime? certificateExpiryDate) {
         DateTime dateTime = DateTime.Now ;

         _cache.AddOrUpdate (url, key => new Item (key, dateTime, roundtripMilliseconds, content, certificateExpiryDate),
                             (key,
                              oldItem) => new Item (key, dateTime, roundtripMilliseconds, content, certificateExpiryDate)) ;
      }

      public static void Delete (string url) {
         if (!_cache.ContainsKey (url)) return ;

         Item item ;

         _cache.TryRemove (url, out item) ;
      }

      private static bool IsExpired (Item item,
                                     TimeSpan expiry) {
         return item.CheckedAt.Add (expiry) < DateTime.Now ;
      }

      public static Item GetCached (string url,
                                    TimeSpan expiry) {
         if (!_cache.ContainsKey (url)) return null ;

         // Expired?
         if (IsExpired (_cache [url], expiry)) {
            Item item ;
            _cache.TryRemove (url, out item) ;

            return null ;
         }

         return _cache [url] ;
      }

      public static void Clear() {
         _cache.Clear();
      }
   }
}
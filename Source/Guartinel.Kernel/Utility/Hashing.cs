using System ;
using System.IO ;
using System.Linq ;
using System.Security.Cryptography ;
using System.Text ;

// Reference for algorithm:
// http%3A%2F%2Fwww.convertstring.com%2FHash%2FSHA512&h=PAQENf-1r

namespace Guartinel.Kernel.Utility {
   public static class Hashing {
      private static string ByteArrayToString (byte[] source) {
         StringBuilder hexString = new StringBuilder (source.Length * 2) ;
         foreach (byte sourceByte in source) {
            hexString.AppendFormat ("{0:x2}", sourceByte) ;
         }
         return hexString.ToString() ;
      }
        //DTAP if  this algorithm updated the JS equivalent side (MS , Websites) must be updated to!
        public static string GenerateHash (string plainText,
                                         string salt) {
         if (plainText == null) return string.Empty ;

         using (SHA512Managed algorithm = new SHA512Managed()) {
            MemoryStream stream = new MemoryStream() ;

            // Text first, salt second
            stream.Write (Encoding.UTF8.GetBytes (plainText), 0, plainText.Length) ;
            if (!string.IsNullOrEmpty (salt)) {
               stream.Write (Encoding.UTF8.GetBytes (salt), 0, salt.Length) ;
            }            

            stream.Seek (0, SeekOrigin.Begin) ;
            byte[] hash = algorithm.ComputeHash (stream) ;
            
            return ByteArrayToString (hash).ToUpper() ;
         }
      }
   }
}
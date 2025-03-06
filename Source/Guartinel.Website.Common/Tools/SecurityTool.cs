using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Guartinel.Website.Common.Tools {
    public static class SecurityTool {
     /*   public static string GenerateHash(string password) {
            StringBuilder Sb = new StringBuilder();

            using (SHA512 hash = SHA512Managed.Create()) {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(password));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
                }
            return Sb.ToString();
            }

        public static bool IsPasswordValid(string passwordAttempt, string truestedPasswordHash) {
            if (passwordAttempt.Equals(truestedPasswordHash)) { // assume the password is already hashed
                return true;
                }

            passwordAttempt = GenerateHash(passwordAttempt);
            if (passwordAttempt.Equals(truestedPasswordHash)) {
                return true;
                }
            return false;
            }
            
        public static string CreateToken() {
            return Hashing.G(Guid.NewGuid().ToString().Replace("-", string.Empty));
            }*/
        }
    }

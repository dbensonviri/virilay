using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.IO;
namespace Virilay.Security
{
    //CONSIDERATIONS:
    //  do not use string. strings do not get garbage collected

    class Security
    {
        /*
         usage:
         var password=Encoding.UTF8.GetBytes("awesome password");  <--- except do not use system.string?
         ....
             */
        public byte[] GenerateSalt(int length)
        {
            var bytes = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            return bytes;
        }
        public byte[] GenerateHash(byte[] password, byte[] salt, int iterations, int length)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return deriveBytes.GetBytes(length);
            }
        }

    }
}

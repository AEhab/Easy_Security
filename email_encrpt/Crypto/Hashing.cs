using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Crypto
{
    class Hashing
    {
        /// <summary>
        /// Compute Hash using SHA-256 and a random salt of length 15 bytes
        /// </summary>
        /// <param name="text">Text to be hashed</param>
        /// <param name="salt">Optional salt to be used for the hashing</param>
        /// <returns>array of bytes, the hash with the salt appended to it</returns>
        public static byte[] ComputeHash(string text, byte[] salt = null)
        {
            byte[] saltBytes = null;
            if (salt != null)
            {
                saltBytes = salt;
            }
            else
            {
                int SaltLength = 15;
                saltBytes = new byte[SaltLength];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetNonZeroBytes(saltBytes);
                rng.Dispose();
            }
            byte[] plainData = Encoding.UTF8.GetBytes(text);
            byte[] plainDataWithSalt = new byte[plainData.Length + saltBytes.Length];

            for (int x = 0; x < plainData.Length; x++)
                plainDataWithSalt[x] = plainData[x];
            for (int n = 0; n < saltBytes.Length; n++)
                plainDataWithSalt[plainData.Length + n] = saltBytes[n];

            byte[] hashValue = null;

            SHA256Managed sha = new SHA256Managed();
            hashValue = sha.ComputeHash(plainDataWithSalt);
            sha.Dispose();

            byte[] result = new byte[hashValue.Length + saltBytes.Length];
            for (int x = 0; x < hashValue.Length; x++)
                result[x] = hashValue[x];
            for (int n = 0; n < saltBytes.Length; n++)
                result[hashValue.Length + n] = saltBytes[n];

            return (result);
        }
        /// <summary>
        /// Verify if a text hashes to a given hash with the salt appended to it
        /// </summary>
        /// <param name="hash">The given hash to be checked against</param>
        /// <param name="text">Text to be checked</param>
        /// <returns>boolean, true if the message hashes to the same hash</returns>
        public static bool VerifyHash(string hash, string text)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);
            byte[] saltBytes = new byte[hashBytes.Length - 32];
            for (int i = 0; i < saltBytes.Length; ++i)
                saltBytes[i] = hashBytes[32 + i];
            byte[] compuHash = ComputeHash(text, saltBytes);
            string hash1 = Convert.ToBase64String(compuHash);

            return hash1 == hash;
        }
    }
}

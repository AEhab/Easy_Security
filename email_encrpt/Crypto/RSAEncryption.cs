using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Crypto
{
    class RSAEncryption
    {
        /// <summary>
        /// Encrypts a given message with RSA algorithim using a given public key
        /// </summary>
        /// <param name="plaintext">The text to be encrypted</param>
        /// <param name="pubKey">The RSA public key</param>
        /// <returns>array of bytes, the encrypted text</returns>
        public static byte[] EncryptRSA(byte[] plaintext, RSAParameters pubKey)
        {
            byte[] ret;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(pubKey);
            ret = rsa.Encrypt(plaintext, true);
            return ret;
        }
        /// <summary>
        /// Decrypts a given ciphertext with RSA algorithim using a given private key
        /// </summary>
        /// <param name="cipherText">Text to be decrypted</param>
        /// <param name="privKey">RSA private key</param>
        /// <returns>array of bytes, the decrypted text</returns>
        public static byte[] DecryptRSA(byte[] cipherText, RSAParameters privKey)
        {
            byte[] ret;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(privKey);
            ret = rsa.Decrypt(cipherText, true);
            return ret;
        }
    }
}

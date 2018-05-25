using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
namespace Crypto
{
    class DigitalSignature
    {
        /// <summary>
        /// Digitally signs a given message using the private key of the sender
        /// </summary>
        /// <param name="RSAParams">Private key of the sender</param>
        /// <param name="message">Message to be signed</param>
        /// <returns>array of bytes, the digital signature, with the salt generated in the hash appended to it</returns>
        public static byte[] SignMessage(RSAParameters RSAParams, string message)
        {
            byte[] hashedValue = Hashing.ComputeHash(message);
            byte[] signedHashedValue = null;
            byte[] saltBytes = new byte[15];

            for (int i = hashedValue.Length - 15, j = 0; i < hashedValue.Length; i++, j++)
            {
                saltBytes[j] = hashedValue[i];
            }
            byte[] hashedValueWithoutSalt = new byte[hashedValue.Length - 15];
            for (int i = 0; i < hashedValue.Length - 15; i++)
                hashedValueWithoutSalt[i] = hashedValue[i];
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(RSAParams);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
            RSAFormatter.SetHashAlgorithm("SHA256");
            signedHashedValue = RSAFormatter.CreateSignature(hashedValueWithoutSalt);
            byte[] signedWithSalt = new byte[signedHashedValue.Length + saltBytes.Length];
            for (int i = 0; i < signedWithSalt.Length - 15; i++)
                signedWithSalt[i] = signedHashedValue[i];
            for (int i = signedWithSalt.Length - 15, j = 0; i < signedWithSalt.Length; i++, j++)
                signedWithSalt[i] = saltBytes[j];
            rsa.Dispose();
            return signedWithSalt;
        }
        /// <summary>
        /// Verifies if a given signature is indeed generated using a given message and a public key
        /// </summary>
        /// <param name="RSAParams">Signer's public key</param>
        /// <param name="message">Plaintext Message</param>
        /// <param name="signature">Signature to be checked</param>
        /// <returns>boolean, true if the signature is valid</returns>
        public static bool VerifySignature(RSAParameters RSAParams, string message, string signature)
        {
            byte[] signedHashedValueWithSalt = Convert.FromBase64String(signature);
            byte[] signedHashedValue = new byte[signedHashedValueWithSalt.Length - 15];
            byte[] saltBytes = new byte[15];
            
            for (int i = signedHashedValueWithSalt.Length - 15, j = 0; i < signedHashedValueWithSalt.Length; i++, j++)
            {
                saltBytes[j] = signedHashedValueWithSalt[i];
            }
            byte[] hashedValueWithSalt = Hashing.ComputeHash(message, saltBytes);
            byte[] hashedValue = new byte[hashedValueWithSalt.Length - 15];
            for (int i = 0; i < hashedValueWithSalt.Length - 15; ++i)
            {
                hashedValue[i] = hashedValueWithSalt[i];
            }
            for (int i = 0; i < signedHashedValueWithSalt.Length - 15; ++i)
            {
                signedHashedValue[i] = signedHashedValueWithSalt[i];
            }
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(RSAParams);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            RSADeformatter.SetHashAlgorithm("SHA256");
            bool x = RSADeformatter.VerifySignature(hashedValue, signedHashedValue);
            rsa.Dispose();
            return x;
        }
    }
}

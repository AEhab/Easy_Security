using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
namespace Crypto
{
    class Encryption
    {
        /// <summary>
        /// a list of strings used as headers and tails in the format of the encrypted output to be able to retrieve these
        /// values in the decryption process
        /// </summary>
        private readonly static string greeting = "The email has been encrypted using EasySecurity, please use your private key to decrypt this message using the following encrypted key.";
        private readonly static string cryptKeyHeader = "----START ENCRYPTED CRYPTKEY----";
        private readonly static string cryptKeyTail = "----END ENCRYPTED CRYPTKEY----";
        private readonly static string authKeyHeader = "----START ENCRYPTED AUTHKEY----";
        private readonly static string authKeyTail = "----END ENCRYPTED AUTHKEY----";
        private readonly static string messageHeader = "----START MESSAGE----";
        private readonly static string messageTail = "----END MESSAGE----";
        private readonly static string signatureHeader = "----START SIGNATURE----";
        private readonly static string signatureTail = "----END SIGNATURE----";
        private readonly static string pubKeyHeader = "----START PUBLIC KEY----";
        private readonly static string pubKeyTail = "----END PUBLIC KEY----";

        /// <summary>
        /// Encrypts a given message using a randomly generated keys and applying symmetric encryption(AES then HMAC for
        /// authentication) to the message text, then encrypts the random keys using RSA and appends it to the ciphertext
        /// </summary>
        /// <param name="message">Message to be encrypted</param>
        /// <param name="publicKeyString">Public key of the recipient</param>
        /// <returns>The formatted encrypted message with all the non-secure data added to it</returns>
        public static string EncryptMessage(string message, string publicKeyString)
        {
            string cipherMessage;
            string ret;
            byte[] cryptKey = AESThenHMAC.NewKey();
            byte[] authKey = AESThenHMAC.NewKey();
            RSAParameters pubkey = KeyVault.DeserializeRSAParams(publicKeyString);

            byte[] encryptedCryptKey = RSAEncryption.EncryptRSA(cryptKey, pubkey);
            byte[] encryptedAuthKey = RSAEncryption.EncryptRSA(authKey, pubkey);

            cipherMessage = AESThenHMAC.Encrypt(message, cryptKey, authKey);
            using (StringWriter sw = new StringWriter())
            {
                sw.WriteLine(greeting);
                sw.WriteLine(cryptKeyHeader);
                sw.WriteLine(Convert.ToBase64String(encryptedCryptKey));
                sw.WriteLine(cryptKeyTail);
                sw.WriteLine(authKeyHeader);
                sw.WriteLine(Convert.ToBase64String(encryptedAuthKey));
                sw.WriteLine(authKeyTail);
                sw.WriteLine(messageHeader);
                sw.WriteLine(cipherMessage);
                sw.WriteLine(messageTail);
                ret = sw.ToString();

            }
            return ret;
        }
        /// <summary>
        /// Same function as EncryptMessage, but signs the message first
        /// </summary>
        /// <param name="message">Message to be encrypted</param>
        /// <param name="publicKeyString">Public key of the recipient</param>
        /// <param name="password">Password needed to use the user's private key for the signature process</param>
        /// <returns>
        /// The formatted encrypted message with all the non-secure data added to it, in addition to the digital
        /// signature
        /// </returns>
        public static string EncryptAndSignMessage(string message, string publicKeyString, string password)
        {
            string sign;
            RSAParameters key = KeyVault.RetrieveRSAParams(password);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            using (StringWriter sw = new StringWriter())
            {
                sw.WriteLine(signatureHeader);
                sw.WriteLine(Convert.ToBase64String(DigitalSignature.SignMessage(key, message)));
                sw.WriteLine(signatureTail);
                sw.WriteLine(pubKeyHeader);
                sw.WriteLine(KeyVault.SerializeRSAParams(rsa.ExportParameters(false)));
                sw.WriteLine(pubKeyTail);
                sign = sw.ToString();
            }
            return EncryptMessage(message, publicKeyString) + sign;
        }
        /// <summary>
        /// Decrypts a given message using the user's password-protected private key
        /// </summary>
        /// <param name="cipherMessage">The message to be decrypted</param>
        /// <param name="password">The user's password used to retrieve the user's private key</param>
        /// <returns>Decrypted message</returns>
        public static string DecryptMessage(string cipherMessage, string password)
        {
            bool isSigned = false;
            string encryptedCryptKey;
            string encryptedAuthKey;
            string message;
            string signature;
            string pubKey = null;

            using (StringReader sr = new StringReader(cipherMessage))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == greeting) break;
                }
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == cryptKeyHeader) break;
                }
                encryptedCryptKey = sr.ReadLine();

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == authKeyHeader) break;
                }
                encryptedAuthKey = sr.ReadLine();

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == messageHeader) break;
                }
                message = sr.ReadLine();

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == signatureHeader)
                    {
                        isSigned = true;
                        break;
                    }
                }
                signature = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == pubKeyHeader)
                    {
                        while ((line = sr.ReadLine()) != pubKeyTail)
                        {
                            pubKey += line;
                        }
                    }
                }
            }
            while (encryptedAuthKey.Length % 4 != 0)
                encryptedAuthKey += "=";
            while (signature != null && signature.Length % 4 != 0)
                signature += "=";
            while (encryptedCryptKey.Length % 4 != 0)
                encryptedCryptKey += "=";
            while (message.Length % 4 != 0)
                message += "=";
            RSAParameters keys = KeyVault.RetrieveRSAParams(password);
            byte[] encryptedCryptKeyBytes = Convert.FromBase64String(encryptedCryptKey);
            byte[] encryptedAuthKeyBytes = Convert.FromBase64String(encryptedAuthKey);

            byte[] cryptKey = RSAEncryption.DecryptRSA(encryptedCryptKeyBytes, keys);
            byte[] authKey = RSAEncryption.DecryptRSA(encryptedAuthKeyBytes, keys);
            string plainTextMessage = AESThenHMAC.Decrypt(message, cryptKey, authKey);

            if (isSigned)
            {
                RSAParameters pubkey = KeyVault.DeserializeRSAParams(pubKey);
                if (!DigitalSignature.VerifySignature(pubkey, plainTextMessage, signature))
                {
                    string warning = "WARNING: this message appears to have a wrong digital signature.\r\n";
                    plainTextMessage = warning + plainTextMessage;
                }
                else
                {
                    string success = "This message is digitally signed properly.\r\n";
                    plainTextMessage = success + plainTextMessage;
                }
            }
            return plainTextMessage;

        }

    }
}

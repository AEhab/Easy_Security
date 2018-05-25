using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
namespace Crypto
{
    class KeyVault
    {
        /// <summary>
        /// Transforms the RSAParamters object into an XML string for storage
        /// </summary>
        /// <param name="key">The key to be stored</param>
        /// <returns>String in XML format</returns>
        public static string SerializeRSAParams(RSAParameters key)
        {
            var stringWriter = new System.IO.StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(stringWriter, key);
            string xml = stringWriter.ToString();
            return xml;
        }

        /// <summary>
        /// Transforms a key in the form of XML string into an RSAParamaters object
        /// </summary>
        /// <param name="xmlString">String to be transformed</param>
        /// <returns>RSAParamters keys</returns>
        public static RSAParameters DeserializeRSAParams(string xmlString)
        {
            var xmls = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            var stringReader = new System.IO.StringReader(xmlString);
            var ret = (RSAParameters)xmls.Deserialize(stringReader);
            return ret;
        }

        /// <summary>
        /// Retrieves public and private key pair from storage
        /// </summary>
        /// <param name="password">User password needed for retrieval</param>
        /// <exception cref="Exception">Given password doesn't match key password</exception>
        /// <returns>RSAParamters object with the public and private key pairs</returns>
        public static RSAParameters RetrieveRSAParams(string password)
        {
            RSAParameters ret;
            FileStream savedKeys;
            if (!File.Exists("C:/EasySecurity/keys.bmu"))
            {
                RSACryptoServiceProvider rsaa = new RSACryptoServiceProvider(2048);
                ret = rsaa.ExportParameters(true);
                rsaa.Dispose();
                return ret;
            }
            else
            {
                savedKeys = new FileStream("C:/EasySecurity/keys.bmu", FileMode.Open, FileAccess.Read, FileShare.None);
            }
            string encryptedParams;
            string passHash;
            using (StreamReader sr = new StreamReader(savedKeys))
            {
                string line;
                while ((line = sr.ReadLine()) != "----START OF PASS HASH----") ;
                passHash = sr.ReadLine();
                while ((line = sr.ReadLine()) != "----END OF PASS HASH----") ;
                while ((line = sr.ReadLine()) != "----START KEYS----") ;
                encryptedParams = sr.ReadLine();
            }

            if (Hashing.VerifyHash(passHash, password) == false)
            {
                throw new Exception("Wrong password");
            }

            string xmlstring = AESThenHMAC.DecryptWithPassword(encryptedParams, password);
            ret = DeserializeRSAParams(xmlstring);

            return ret;
        }
        /// <summary>
        /// Saves new public and private key pair, along with a salted-SHA-256 hash of the user password to protect these keys
        /// </summary>
        /// <param name="password">Choosen password</param>
       public static void SaveRSAParamaters(string password)
        {

            if (File.Exists("C:/EasySecurity/keys.bmu"))
                File.Delete("C:/EasySecurity/keys.bmu");
            FileStream file = new FileStream("C:/EasySecurity/keys.bmu", FileMode.CreateNew, FileAccess.Write, FileShare.None);
            FileStream file2 = new FileStream("C:/EasySecurity/userPubKey.txt", FileMode.CreateNew, FileAccess.Write, FileShare.None);

            RSAParameters keys, pub;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            keys = rsa.ExportParameters(true);
            pub = rsa.ExportParameters(false);
            string xml = SerializeRSAParams(keys);
            string xml2 = SerializeRSAParams(pub);
            string encryptedxml = AESThenHMAC.EncryptWithPassword(xml, password);

            using (StreamWriter sw = new StreamWriter(file))
            {
                string passhash = Convert.ToBase64String(Hashing.ComputeHash(password));
                sw.WriteLine("----START OF PASS HASH----");
                sw.WriteLine(passhash);
                sw.WriteLine("----END OF PASS HASH----");
                sw.WriteLine("----START KEYS----");
                sw.WriteLine(encryptedxml);
                sw.WriteLine("----END KEYS----");
            }
            using (StreamWriter sw = new StreamWriter(file2))
            {
                sw.WriteLine(xml2);
            }
            rsa.Dispose();
        }
    }
}

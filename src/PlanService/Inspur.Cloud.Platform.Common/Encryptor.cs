using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Inspur.ECP.Rtf.Common
{
    public class Encryptor
    {
        public static string AESEncrypt(string input, string key, string vector = "68e55fc498ea9c38")
        {
            var encryptKey = Encoding.UTF8.GetBytes(key);
            var vKey = Encoding.UTF8.GetBytes(vector);

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.IV = vKey;
                using (var encryptor = aesAlg.CreateEncryptor(encryptKey, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor,
                            CryptoStreamMode.Write))

                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string AESDecrypt(string data, string key, string vector = "68e55fc498ea9c38")
        {
            var encryptKey = Encoding.UTF8.GetBytes(key);
            var vKey = Encoding.UTF8.GetBytes(vector);
            byte[] encryptedData = Convert.FromBase64String(data);


            using (var aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.IV = vKey;

                using (var decryptor = aesAlg.CreateDecryptor(encryptKey, aesAlg.IV))
                {
                    byte[] bStr = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                    return Encoding.UTF8.GetString(bStr);
                }
            }
        }
    }
}

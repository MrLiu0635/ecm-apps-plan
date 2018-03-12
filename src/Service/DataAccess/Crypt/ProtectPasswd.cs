using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    public class ProtectPasswd
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="planPasswd"></param>
        /// <returns></returns>
        public static string EncryptPasswd(string planPasswd)
        {
            string key = GetLegalKey();
            if (key.Length == 0) //如果没有获取到合适的key，不做处理
            {
                return planPasswd;
            }
            SymmCryptoEx symmCrypService = new SymmCryptoEx(SymmProvEnum.DES);
            return symmCrypService.Encrypting(planPasswd, key);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedPasswd"></param>
        /// <returns></returns>
        public static string DecryptPasswd(string encryptedPasswd)
        {
            string key = GetLegalKey();
            if (key.Length == 0)  //如果没有获取到合适的key, 不做处理
            {
                return encryptedPasswd;
            }
            if (string.IsNullOrEmpty(encryptedPasswd))
                return "";
            try
            {
                SymmCryptoEx symmCrypService = new SymmCryptoEx(SymmProvEnum.DES);
                return symmCrypService.Decrypting(encryptedPasswd, key);
            }
            catch
            {
                SymmCrypto symmCrypService = new SymmCrypto(SymmProvEnum.DES);
                return symmCrypService.Decrypting(encryptedPasswd, key);
            }
        }

        private static string GetLegalKey()
        {
            string key = "InspurGenersoft";
            byte[] legalKey = new byte[8];
            if (key.Length != 0)
            {
                byte[] keyBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
                SHA1 sha1 = SHA1CryptoServiceProvider.Create();
                byte[] hashValue = sha1.ComputeHash(keyBytes);
                for (int i = 0; i < 16; i = i + 2)
                {
                    legalKey[i / 2] = hashValue[i];
                }

            }

            return System.Text.ASCIIEncoding.ASCII.GetString(legalKey);
        }
    }
}

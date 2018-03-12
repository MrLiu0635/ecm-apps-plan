﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public enum SymmProvEnum : int
    {
        DES,
        RC2,
        Rijndael,
        //SM4
    }


    /// <summary> 
    /// SymmCrypto 的摘要说明。 
    /// SymmCrypto类实现.NET框架下的加密和解密服务。 
    /// 原作者： Frank Fang : fangfrank@hotmail.com 
    /// </summary> 
    [Obsolete("建议使用SymmCryptoEx类代替此类")]
    public class SymmCrypto
    {
        private SymmetricAlgorithm mobjCryptoService;

        /// <remarks> 
        /// 使用.Net SymmetricAlgorithm 类的构造器. 
        /// </remarks> 
        public SymmCrypto(SymmProvEnum NetSelected)
        {
            switch (NetSelected)
            {
                case SymmProvEnum.DES:
                    mobjCryptoService = new DESCryptoServiceProvider();
                    break;
                case SymmProvEnum.RC2:
                    mobjCryptoService = new RC2CryptoServiceProvider();
                    break;
                case SymmProvEnum.Rijndael:
                    mobjCryptoService = new RijndaelManaged();
                    break;
            }
        }

        /// <remarks> 
        /// 使用自定义SymmetricAlgorithm类的构造器. 
        /// </remarks> 
        public SymmCrypto(SymmetricAlgorithm ServiceProvider)
        {
            mobjCryptoService = ServiceProvider;
        }

        /// <remarks> 
        /// Depending on the legal key size limitations of 
        /// a specific CryptoService provider and length of 
        /// the private key provided, padding the secret key 
        /// with space character to meet the legal size of the algorithm. 
        /// </remarks> 
        private byte[] GetLegalKey(string Key)
        {
            string sTemp;
            if (mobjCryptoService.LegalKeySizes.Length > 0)
            {
                int lessSize = 0, moreSize = mobjCryptoService.LegalKeySizes[0].MinSize;
                // key sizes are in bits 
                while (Key.Length * 8 > moreSize)
                {
                    lessSize = moreSize;
                    moreSize += mobjCryptoService.LegalKeySizes[0].SkipSize;
                }
                sTemp = Key.PadRight(moreSize / 8, ' ');
            }
            else
                sTemp = Key;

            // convert the secret key to byte array 
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        public string Encrypting(string Source, string Key)
        {
            byte[] bytIn = System.Text.ASCIIEncoding.ASCII.GetBytes(Source);
            // create a MemoryStream so that the process can be done without I/O files 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            byte[] bytKey = GetLegalKey(Key);

            // set the private key 
            mobjCryptoService.Key = bytKey;
            mobjCryptoService.IV = bytKey;

            // create an Encryptor from the Provider Service instance 
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();

            // create Crypto Stream that transforms a stream using the encryption 
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

            // write out encrypted content into MemoryStream 
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();

            // get the output and trim the '\0' bytes 
            byte[] bytOut = ms.ToArray();
            //int i = 0;
            //for (i = 0; i < bytOut.Length; i++)
            //    if (bytOut[i] == 0)
            //        break;
            //去掉上面，有这个在加密"password"时加密不正确。
            // convert into Base64 so that the result can be used in xml 
            return System.Convert.ToBase64String(bytOut);
        }

        public string Decrypting(string Source, string Key)
        {
            // convert from Base64 to binary 
            byte[] bytIn = System.Convert.FromBase64String(Source);
            // create a MemoryStream with the input 
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);

            byte[] bytKey = GetLegalKey(Key);

            // set the private key 
            mobjCryptoService.Key = bytKey;
            mobjCryptoService.IV = bytKey;

            // create a Decryptor from the Provider Service instance 
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();

            // create Crypto Stream that transforms a stream using the decryption 
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);

            // read out the result from the Crypto Stream 
            System.IO.StreamReader sr = new System.IO.StreamReader(cs);
            return sr.ReadToEnd();
        }
    }

}
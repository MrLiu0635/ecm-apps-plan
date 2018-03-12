using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    /// <summary> 
    /// SymmCrypto 的摘要说明。 
    /// SymmCrypto类实现.NET框架下的加密和解密服务。 
    /// 原作者： Frank Fang : fangfrank@hotmail.com 
    /// </summary> 
    public class SymmCryptoEx
    {
        private SymmetricAlgorithm mobjCryptoService;

        /// <summary> 
        /// 使用.Net SymmetricAlgorithm 类的构造器. 
        /// </summary> 
        /// <param name="symmSelected">选择的加密算法</param>
        public SymmCryptoEx(SymmProvEnum symmSelected)
        {
            switch (symmSelected)
            {
                case SymmProvEnum.DES:
                    mobjCryptoService = new DESCryptoServiceProvider();
                    break;
                case SymmProvEnum.RC2:
                    mobjCryptoService = new RC2CryptoServiceProvider();
                    break;
                //case SymmProvEnum.SM4:
                //    mobjCryptoService = new SM4CryptoServiceProvider();
                //    break;
                case SymmProvEnum.Rijndael:
                default:
                    mobjCryptoService = new RijndaelManaged();
                    break;
            }
        }

        /// <summary> 
        /// 使用自定义SymmetricAlgorithm类的构造器. 
        /// </summary> 
        /// <param name="serviceProvider">对称加密算法</param>
        public SymmCryptoEx(SymmetricAlgorithm serviceProvider)
        {
            mobjCryptoService = serviceProvider;
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>密文</returns>
        public string Encrypting(string plaintext, string key)
        {
            byte[] bytIn = System.Text.ASCIIEncoding.UTF8.GetBytes(plaintext);
            // create a MemoryStream so that the process can be done without I/O files 
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                // set the private key 
                mobjCryptoService.Key = GetLegalKey(key);
                // IV 
                mobjCryptoService.IV = GetLegalIV(key);
                // create an Encryptor from the Provider Service instance 
                ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();

                //不够分组大小的，强制以'\0'补齐后再加密
                byte[] blankBuffer = this.GetPadding(this.mobjCryptoService.BlockSize, bytIn.Length);

                // create Crypto Stream that transforms a stream using the encryption 
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

                // write out encrypted content into MemoryStream 
                cs.Write(bytIn, 0, bytIn.Length);
                cs.Write(blankBuffer, 0, blankBuffer.Length);
                cs.FlushFinalBlock();
                cs.Close();

                // get the output and trim the '\0' bytes 
                byte[] bytOut = ms.ToArray();
                return System.Convert.ToBase64String(bytOut);
            }
        }
        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="cipher">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public string Decrypting(string cipher, string key)
        {
            // convert from Base64 to binary 
            byte[] bytIn = System.Convert.FromBase64String(cipher);
            // create a MemoryStream with the input 
            using (MemoryStream ms = new System.IO.MemoryStream())
            {
                // set the private key 
                mobjCryptoService.Key = GetLegalKey(key);
                // IV 
                mobjCryptoService.IV = GetLegalIV(key);

                // create a Decryptor from the Provider Service instance 
                ICryptoTransform decrypto = mobjCryptoService.CreateDecryptor();

                // create Crypto Stream that transforms a stream using the decryption 
                using (CryptoStream cs = new CryptoStream(ms, decrypto, CryptoStreamMode.Write))
                {
                    cs.Write(bytIn, 0, bytIn.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                // get the output and trim the '\0' bytes 
                byte[] outputs = ms.ToArray();
                ms.Close();
                return System.Text.ASCIIEncoding.UTF8.GetString(outputs).TrimEnd('\0');
            }
        }

        /// <summary>
        /// 根据分组大小和字节长度，计算得到需要补齐的空字节数组
        /// </summary>
        /// <param name="blockSize">分组大小(按bit)</param>
        /// <param name="byteLength">字节长度</param>
        /// <returns>空字节数组</returns>
        private byte[] GetPadding(int blockSize, int byteLength)
        {
            int blockSizeInByte = blockSize / 8;
            int remainder = (int)(byteLength % blockSizeInByte);
            byte[] buffer = new byte[(blockSizeInByte - remainder) % blockSizeInByte];
            return buffer;
        }

        /// <summary> 
        /// Depending on the legal key size limitations of 
        /// a specific CryptoService provider and length of 
        /// the private key provided, padding the secret key 
        /// with space character to meet the legal size of the algorithm. 
        /// </summary> 
        /// <param name="key">初始密钥</param>
        /// <returns>合法密钥</returns>
        private byte[] GetLegalKey(string key)
        {
            string temp;
            if (mobjCryptoService.LegalKeySizes.Length > 0)
            {
                int minSize = mobjCryptoService.LegalKeySizes[0].MinSize,
                    maxSize = mobjCryptoService.LegalKeySizes[0].MaxSize,
                    skipSize = mobjCryptoService.LegalKeySizes[0].SkipSize;

                int length = key.Length * 8;
                int moreSize = length;
                //如果长度小于最大密钥长度并大于最小密钥长度，则根据可允许的间隔设置密钥长度
                if (length > minSize && length <= maxSize)
                {
                    int currentSize = minSize;//
                    // key sizes are in bits 
                    while (currentSize < moreSize && currentSize < maxSize)
                    {
                        currentSize += skipSize;
                    }
                    //循环出来有两种可能：1、currentSize >= maxSize 按最大算
                    if (currentSize >= maxSize)
                        moreSize = maxSize;
                    else
                        moreSize = currentSize;
                }
                //如果长度大于最大长度，则按最大长度计算
                else if (length > maxSize)
                {
                    moreSize = maxSize;
                    key = key.Substring(0, maxSize / 8);
                }
                else //小于最小长度，则按最小
                {
                    moreSize = minSize;
                }

                temp = key.PadRight(moreSize / 8, ' ');
            }
            else
                temp = key;

            // convert the secret key to byte array 
            return ASCIIEncoding.ASCII.GetBytes(temp);
        }

        /// <summary>
        /// 初始向量的长度需与分组大小一致，而不是与密钥长度一致
        /// 返回初始向量
        /// </summary>
        /// <param name="key">初始密钥</param>
        /// <returns>初始向量</returns>
        private byte[] GetLegalIV(string key)
        {
            int blockSize = mobjCryptoService.BlockSize;
            if (key.Length * 8 > blockSize)
                key = key.Substring(0, blockSize / 8);
            return ASCIIEncoding.ASCII.GetBytes(key.PadRight(blockSize / 8, ' '));
        }
    }
}

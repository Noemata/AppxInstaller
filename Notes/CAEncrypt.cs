using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.IO;

namespace UtilsLib.EncryptUtil
{
    public class CAEncrypt
    {
        /// <summary>
        /// 将普通字符串加密后以base64字符串输出
        /// </summary>
        /// <param name="content"></param>
        /// <param name="CaPubkeyFileName"></param>
        /// <returns></returns>
        public static string CAEncryption(string content, string CaPubkeyFileName)
        {
            X509Certificate2 pubcrt = new X509Certificate2(CaPubkeyFileName);
            return Encrypt(content, pubcrt);
        }
        /// <summary>
        /// 字节输入加密字节输出
        /// </summary>
        /// <param name="content"></param>
        /// <param name="CaPubkeyFileName"></param>
        /// <returns></returns>
        public static byte[] CAEncryption(byte[] content, string CaPubkeyFileName)
        {
            X509Certificate2 pubcrt = new X509Certificate2(CaPubkeyFileName);
            return Encrypt(content, pubcrt);
        }
        public static byte[] CAEncryption(byte[] btData, string CaPubkeyFileName, string CaPwd)
        {
            X509Certificate2 pubcrt = new X509Certificate2(CaPubkeyFileName, CaPwd);
            return Encrypt(btData, pubcrt);
        }
        /// <summary>
        /// 将普通字符串加密后以base64字符串输出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pubcrt"></param>
        /// <returns></returns>
        public static string Encrypt(string data, X509Certificate2 pubcrt)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data), pubcrt), Base64FormattingOptions.None);
        }
        /// <summary>
        /// 字节输入加密字节输出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pubcrt"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, X509Certificate2 pubcrt)
        {
            X509Certificate2 _X509Certificate2 = pubcrt;
            using (RSACryptoServiceProvider RSACryptography = _X509Certificate2.PublicKey.Key as RSACryptoServiceProvider)
            {
                Byte[] PlaintextData = data;
                int MaxBlockSize = RSACryptography.KeySize / 8 - 11;    //加密块最大长度限制  
                if (PlaintextData.Length <= MaxBlockSize)
                    return RSACryptography.Encrypt(PlaintextData, false);
                using (MemoryStream PlaiStream = new MemoryStream(PlaintextData))
                using (MemoryStream CrypStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    while (BlockSize > 0)
                    {
                        Byte[] ToEncrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);
                        Byte[] Cryptograph = RSACryptography.Encrypt(ToEncrypt, false);
                        CrypStream.Write(Cryptograph, 0, Cryptograph.Length);
                        BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    }
                    return CrypStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 将base64字符串解密为普通字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="CaPrvkeyFileName"></param>
        /// <param name="CaPwd"></param>
        /// <returns></returns>
        public static string CADecryption(string content, string CaPrvkeyFileName, string CaPwd)
        {
            X509Certificate2 prvcrt = new X509Certificate2(CaPrvkeyFileName, CaPwd, X509KeyStorageFlags.Exportable);
            return Decrypt(content, prvcrt);
        }
        /// <summary>
        /// 字节输入解密字节输出
        /// </summary>
        /// <param name="content"></param>
        /// <param name="CaPrvkeyFileName"></param>
        /// <param name="CaPwd"></param>
        /// <returns></returns>
        public static byte[] CADecryption(byte[] content, string CaPrvkeyFileName, string CaPwd)
        {
            X509Certificate2 prvcrt = new X509Certificate2(CaPrvkeyFileName, CaPwd, X509KeyStorageFlags.Exportable);
            return Decrypt(content, prvcrt);
        }
        /// <summary>
        /// 将base64字符串解密为普通字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="prvpfx"></param>
        /// <returns></returns>
        public static string Decrypt(string data, X509Certificate2 prvpfx)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(data), prvpfx));
        }
        /// <summary>
        /// 字节输入解密字节输出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="prvpfx"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, X509Certificate2 prvpfx)
        {
            X509Certificate2 _X509Certificate2 = prvpfx;
            using (RSACryptoServiceProvider RSACryptography = _X509Certificate2.PrivateKey as RSACryptoServiceProvider)
            {
                Byte[] CiphertextData = data;
                int MaxBlockSize = RSACryptography.KeySize / 8; //解密块最大长度限制  
                if (CiphertextData.Length <= MaxBlockSize)
                    return RSACryptography.Decrypt(CiphertextData, false);
                using (MemoryStream CrypStream = new MemoryStream(CiphertextData))
                using (MemoryStream PlaiStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                    while (BlockSize > 0)
                    {
                        Byte[] ToDecrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);
                        Byte[] Plaintext = RSACryptography.Decrypt(ToDecrypt, false);
                        PlaiStream.Write(Plaintext, 0, Plaintext.Length);
                        BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                    }
                    return PlaiStream.ToArray();
                }
            }
        }
    }
}

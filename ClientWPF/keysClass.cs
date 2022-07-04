using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF
{
    internal class keysClass
    {


        ECDiffieHellmanCng bob = new ECDiffieHellmanCng();


        public static byte[] encryptedMessage = null;
        public static byte[] iv = null;
        public static byte[] sessKey;
        // bobPublicKey - публичный ключ, bobPrivateKey - приватный ключ.
        public void generate_PublicKey(out byte[] bobPublicKey, out byte[] bobPrivateKey)
        {
            bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            bob.HashAlgorithm = CngAlgorithm.Sha256;
            bobPublicKey = bob.PublicKey.ToByteArray();
            bobPrivateKey = bob.Key.Export(CngKeyBlobFormat.EccPrivateBlob);

        }
        // alicePublicKey - публичный ключ сервера, bobPrivateKey - приватный ключ клиента 
        public byte[] Creating_SessionKey(byte[] alicePublicKey, byte[] bobPrivateKey)
        {
            using (ECDiffieHellmanCng cng = new ECDiffieHellmanCng(CngKey.Import(bobPrivateKey, CngKeyBlobFormat.EccPrivateBlob)))
            {
                cng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                cng.HashAlgorithm = CngAlgorithm.Sha512;
                sessKey = cng.DeriveKeyMaterial(CngKey.Import(alicePublicKey, CngKeyBlobFormat.EccPublicBlob));
                return sessKey;
            }
        }
        public void EncryptMsg(byte[] Sess_key, string secretMessage, out byte[] encryptedMessage, out byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                var md5 = MD5.Create();
                byte[] hash = md5.ComputeHash((Sess_key));
                aes.Key = hash;
                aes.GenerateIV();
                // генерация публичного ключа
                iv = aes.IV;
                aes.Padding = PaddingMode.PKCS7;

                byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    }

                    encryptedMessage = ms.ToArray();
                }
            }
        }
        public void EncryptMsg(byte[] Sess_key, byte[] secretMessage, out byte[] encryptedMessage, out byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                var md5 = MD5.Create();
                byte[] hash = md5.ComputeHash((Sess_key));
                aes.Key = hash;
                aes.GenerateIV();
                iv = aes.IV;
                aes.Padding = PaddingMode.PKCS7;

                byte[] plaintextMessage = secretMessage;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    }

                    encryptedMessage = ms.ToArray();
                }
            }
        }
        public void EncryptMsg_IV(byte[] Sess_key, byte[] secretMessage, out byte[] encryptedMessage, byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                var md5 = MD5.Create();
                byte[] hash = md5.ComputeHash((Sess_key));
                aes.Key = hash;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                byte[] plaintextMessage = secretMessage;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    }

                    encryptedMessage = ms.ToArray();
                }
            }
        }
        public void DecryptMsg(byte[] Sess_key, byte[] encryptedMessage, byte[] iv, out string message)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {

                var md5 = MD5.Create();
                byte[] hash = md5.ComputeHash((Sess_key));
                aes.Key = hash;

                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();
                        message = Encoding.UTF8.GetString(plaintext.ToArray());
                    }
                }
            }
        }

        public void DecryptMsg(byte[] Sess_key, byte[] encryptedMessage, byte[] iv, out byte[] message)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {

                var md5 = MD5.Create();
                byte[] hash = md5.ComputeHash((Sess_key));
                aes.Key = hash;

                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();
                        message = plaintext.ToArray();
                    }
                }
            }
        }

    }
}

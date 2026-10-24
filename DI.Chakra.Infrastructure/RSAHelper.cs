using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace DI.Chakra.Infrastructure
{
    public class RSAHelper
    {
        #region DO NOT change anything here
        //Must not change this password as code will not work.
        private static string Password = "deEp@Identity09!s$->%"; 
        #endregion

        #region Public Method

        public static string DecryptData(string TextToBeDecrypted)
        {
            if (string.IsNullOrEmpty(TextToBeDecrypted))
                return string.Empty;

            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            string DecryptedData;

            try
            {
                byte[] EncryptedData = Convert.FromBase64String(TextToBeDecrypted);
                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream(EncryptedData);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
                byte[] PlainText = new byte[EncryptedData.Length];
                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
                memoryStream.Close();
                cryptoStream.Close();
                DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            }
            catch
            {
                return string.Empty;
            }
            return DecryptedData;
        }

       
        public static string EncryptData(string TextToBeEncrypted)
        {
            string EncryptedData = string.Empty;

            if (string.IsNullOrEmpty(TextToBeEncrypted))
                return string.Empty;

            try
            {
                RijndaelManaged RijndaelCipher = new RijndaelManaged();
                byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(TextToBeEncrypted);
                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
                ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(PlainText, 0, PlainText.Length);
                cryptoStream.FlushFinalBlock();
                byte[] CipherBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                EncryptedData = Convert.ToBase64String(CipherBytes);

            }
            catch (Exception)
            {
                return string.Empty;
            }
           
            return EncryptedData;
        } 

        #endregion

      
    }
}

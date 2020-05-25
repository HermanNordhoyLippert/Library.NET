using LibraryNET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace LibraryNET.DonauApi
{
    /// <summary>
    /// Unfinished attempt on encrypting and decrypting the data in Donau
    /// </summary>
    public class SimpleCryptographic
    {

        private static byte[] IV = { 12, 11, 12, 55, 0, 108, 121, 54 };
        private static string stringKey = "myCryptKey";
        private static BinaryStringEncoding encoding;
        private static byte[] keyByte;
        private static SymmetricKeyAlgorithmProvider objAlg;
        private static CryptographicKey Key;

        private static string Encrypt(String strMsg)
        {
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(strMsg, encoding);
            IBuffer buffEncrypt = CryptographicEngine.Encrypt(Key, buffMsg, IV.AsBuffer());
            return CryptographicBuffer.EncodeToBase64String(buffEncrypt);
        }

        private static string Decrypt(String strMsg)
        {
            Byte[] bb = Convert.FromBase64String(strMsg);
            IBuffer buffEncrypt = CryptographicEngine.Decrypt(Key, bb.AsBuffer(), IV.AsBuffer());
            return CryptographicBuffer.ConvertBinaryToString(encoding, buffEncrypt);
        }
        
        public static User EncryptUser(User user)
        {
            user.Id = SimpleCryptographic.Encrypt(user.Id);
            user.Username = SimpleCryptographic.Encrypt(user.Username);
            user.Password = SimpleCryptographic.Encrypt(user.Password);
            return user;
        }

        public static User DecryptUser(User user)
        {
            user.Id = SimpleCryptographic.Decrypt(user.Id);
            user.Username = SimpleCryptographic.Decrypt(user.Username);
            user.Password = SimpleCryptographic.Decrypt(user.Password);
            return user;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace NAudioDemo.NetworkChatDemo
{
    class AES256
    {
        private Aes aes;
        private ICryptoTransform crypt;
        private ICryptoTransform decrypt;


        public AES256()
        {
            this.aes = Aes.Create();
            //Генерируем соль
            aes.GenerateIV();
            //Генерируем ключ
            aes.GenerateKey();
            crypt = aes.CreateEncryptor(aes.Key, aes.IV);
            decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
        }

        public AES256(Aes aes, ICryptoTransform crypt, ICryptoTransform decrypt)
        {
            this.aes = aes;
            this.crypt = crypt;
            this.decrypt = decrypt;

        }

        public byte[] ToAes256(byte[] encrypted)
        {
            //encrypted - что будет зашифровано
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
                {
                    cs.Write(encrypted, 0, encrypted.Length);
                    cs.Close();
                }
                //Записываем в переменную encrypted зашиврованный поток байтов
                encrypted = ms.ToArray();
            }
            //Возвращаем поток байт + крепим соль
            return encrypted.Concat(aes.IV).ToArray();
        }

        public byte[] FromAes256(byte[] shifr)
        {
            byte[] bytesIv = new byte[16];
            byte[] mess = new byte[shifr.Length - 16];
            //Списываем соль
            for (int i = shifr.Length - 16, j = 0; i < shifr.Length; i++, j++)
                bytesIv[j] = shifr[i];
            //Списываем оставшуюся часть сообщения
            for (int i = 0; i < shifr.Length - 16; i++)
                mess[i] = shifr[i];

            byte[] data = mess;
            byte[] decode;
            
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.Close();
                }

                decode = ms.ToArray();
            }
            return decode;
        }
    }
}

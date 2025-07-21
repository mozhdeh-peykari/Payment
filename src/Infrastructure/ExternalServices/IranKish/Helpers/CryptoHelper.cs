using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish.Helpers
{
    public static class CryptoHelper
    {
        public static (string Data, string IV) GenerateAuthenticationEnvelope(string terminalId, long amount, string passPhrase, string publicKey)
        {
            string baseString = $"{terminalId}{passPhrase}{amount.ToString().PadLeft(12, '0')}00";

            var aes = Aes.Create();
            aes.KeySize = 128;
            aes.GenerateKey();
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] encryptedData;
            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(baseString);
                encryptedData = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            byte[] combined = aes.Key.Concat(encryptedData).ToArray();
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(combined);

            byte[] finalBytes = aes.Key.Concat(hash).ToArray();

            byte[] rsaEncrypted;
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(publicKey.ToCharArray());
                rsaEncrypted = rsa.Encrypt(finalBytes, RSAEncryptionPadding.Pkcs1);
            }

            string dataHex = BitConverter.ToString(rsaEncrypted).Replace("-", "");
            string ivHex = BitConverter.ToString(aes.IV).Replace("-", "");

            return (dataHex, ivHex);
        }
    }
}

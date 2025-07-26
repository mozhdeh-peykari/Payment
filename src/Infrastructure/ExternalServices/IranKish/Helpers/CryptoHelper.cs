using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.ExternalServices.IranKish.Helpers;

public static class CryptoHelper
{
    //public static (string Data, string IV) GenerateAuthenticationEnvelope(string terminalId, long amount, string password, string publicKey)
    //{
    //    var data = string.Empty;
    //    var iv = string.Empty;
    //    string baseString = $"{terminalId}{password}{amount.ToString().PadLeft(12, '0')}00";

    //    using (Aes myAes = Aes.Create())
    //    {
    //        myAes.KeySize = 128;
    //        myAes.GenerateKey();
    //        myAes.GenerateIV();
    //        byte[] keyAes = myAes.Key;
    //        byte[] ivAes = myAes.IV;

    //        byte[] resultCoding = new byte[48];
    //        byte[] baseStringbyte = HexStringToByteArray(baseString);

    //        byte[] encrypted = EncryptAes(baseStringbyte, myAes.Key, myAes.IV);

    //        using var sha256 = SHA256.Create();
    //        byte[] hsahash = sha256.ComputeHash(encrypted);

    //        resultCoding = CombinArray(keyAes, hsahash);

    //        data = ByteArrayToHexString(RSAEncription(resultCoding, publicKey));
    //        iv = ByteArrayToHexString(ivAes);
    //    }
    //    return (data, iv);
    //}

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

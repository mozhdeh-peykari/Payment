using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.ExternalServices.IranKish.Helpers;

public static class CryptoHelper
{
    public static (string Data, string IV) GenerateAuthenticationEnvelope(string terminalId, long amount, string password, string publicKey)
    {
        var data = string.Empty;
        var iv = string.Empty;
        string baseString = $"{terminalId}{password}{amount.ToString().PadLeft(12, '0')}00";

        using (Aes myAes = Aes.Create())
        {
            myAes.KeySize = 128;
            myAes.GenerateKey();
            myAes.GenerateIV();
            byte[] keyAes = myAes.Key;
            byte[] ivAes = myAes.IV;

            byte[] resultCoding = new byte[48];
            byte[] baseStringbyte = HexStringToByteArray(baseString);

            byte[] encrypted = EncryptAes(baseStringbyte, myAes.Key, myAes.IV);

            using var sha256 = SHA256.Create();
            byte[] hsahash = sha256.ComputeHash(encrypted);

            resultCoding = CombineArray(keyAes, hsahash);

            data = ByteArrayToHexString(RSAEncription(resultCoding, publicKey));
            iv = ByteArrayToHexString(ivAes);
        }
        return (data, iv);
    }

    private static byte[] HexStringToByteArray(string hexString)
    {

        return Enumerable.Range(0, hexString.Length)
        .Where(x => x % 2 == 0)
        .Select(x => Convert.ToByte(value: hexString.Substring(startIndex: x, length: 2), fromBase: 16))
        .ToArray();

    }

    private static byte[] EncryptAes(byte[] plainText, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.KeySize = 128;
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
    }

    private static byte[] CombineArray(byte[] first, byte[] second)
    {
        byte[] bytes = new byte[first.Length + second.Length];
        Array.Copy(first, 0, bytes, 0, first.Length);
        Array.Copy(second, 0, bytes, first.Length, second.Length);
        return bytes;
    }

    private static byte[] RSAEncription(byte[] aesCodingResult, string publicKey)
    {
        var encryptEngine = new Pkcs1Encoding(new RsaEngine());

        using (var txtreader = new StringReader(publicKey))
        {
            var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();

            encryptEngine.Init(true, keyParameter);
        }

        return encryptEngine.ProcessBlock(aesCodingResult, 0, aesCodingResult.Length);
    }

    private static string ByteArrayToHexString(byte[] bytes)
    {
        return (bytes.Select(t => t.ToString(format: "X2")).Aggregate((a, b) => $"{a}{b}"));
    }

    //public static (string Data, string IV) GenerateAuthenticationEnvelope(string terminalId, long amount, string passPhrase, string publicKey)
    //{
    //    string baseString = $"{terminalId}{passPhrase}{amount.ToString().PadLeft(12, '0')}00";

    //    var aes = Aes.Create();
    //    aes.KeySize = 128;
    //    aes.GenerateKey();
    //    aes.GenerateIV();
    //    aes.Mode = CipherMode.CBC;
    //    aes.Padding = PaddingMode.PKCS7;

    //    byte[] encryptedData;
    //    using (var encryptor = aes.CreateEncryptor())
    //    {
    //        byte[] plainBytes = Encoding.UTF8.GetBytes(baseString);
    //        encryptedData = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
    //    }

    //    byte[] combined = aes.Key.Concat(encryptedData).ToArray();
    //    using var sha256 = SHA256.Create();
    //    byte[] hash = sha256.ComputeHash(combined);

    //    byte[] finalBytes = aes.Key.Concat(hash).ToArray();

    //    byte[] rsaEncrypted;
    //    using (RSA rsa = RSA.Create())
    //    {
    //        rsa.ImportFromPem(publicKey.ToCharArray());
    //        rsaEncrypted = rsa.Encrypt(finalBytes, RSAEncryptionPadding.Pkcs1);
    //    }

    //    string dataHex = BitConverter.ToString(rsaEncrypted).Replace("-", "");
    //    string ivHex = BitConverter.ToString(aes.IV).Replace("-", "");

    //    return (dataHex, ivHex);
    //}
}

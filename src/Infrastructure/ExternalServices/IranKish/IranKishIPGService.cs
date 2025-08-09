using Application.IPGServices;
using Application.IPGServices.Dtos;
using Application.IPGServices.Interfaces;
using Infrastructure.ExternalServices.IranKish.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System.Net.Http.Json;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text.Json;

namespace Infrastructure.ExternalServices.IranKish;

public class IranKishIPGService : IIranKishIPGService
{
    private readonly HttpClient _httpClient;
    private readonly IranKishIpgServiceSettings _settings;
    private readonly ILogger<IranKishIPGService> _logger;

    public IranKishIPGService(HttpClient httpClient,
        IOptions<IranKishIpgServiceSettings> settings,
        ILogger<IranKishIPGService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _logger = logger;

        _httpClient = ConfigureHttpClientToIgnoreSsl(_httpClient);
    }

    private static HttpClient ConfigureHttpClientToIgnoreSsl(HttpClient httpClient)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        return new HttpClient(handler)
        {
            BaseAddress = httpClient.BaseAddress
        };
    }

    public async Task<TokenResponseDto> GetTokenAsync(TokenRequestDto req)
    {
        TokenResponse response = null;
        TokenResponseDto dto = null;
        string? res = null;

        //req
        var envelope = GenerateAuthenticationEnvelope(_settings.TerminalId, req.Request.Amount, _settings.Password, _settings.PublicKey);

        //var requestId = Guid.NewGuid().ToString("N").Substring(0, 20);

        var tokenRequest2 = new TokenRequestDto
        {
            AuthenticationEnvelope = new AuthenticationEnvelopeDto
            {
                Iv = envelope.IV,
                Data = envelope.Data
            },
            Request = new RequestDto
            {
                TransactionType = _settings.TransactionType,
                TerminalId = _settings.TerminalId,
                AcceptorId = _settings.AcceptorId,
                Amount = req.Request.Amount,
                RevertUri = req.Request.RevertUri,
                RequestId = req.Request.RequestId,
                RequestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };


        

        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(_settings.Tokenization, req);
            res = await httpResponse.Content.ReadAsStringAsync();
            response = JsonSerializer.Deserialize<TokenResponse>(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: $"[IranKishClient].[GetTokenAsync], Request: {req}, Response: {res}");
        }

        dto = new TokenResponseDto
        {
            Description = response.description,
            ResponseCode  =  response.responseCode,
            IsSuccessful = response.status,
            Token = response.result.token
        };

        return dto;
    }

    public async Task<ConfirmResponseDto> ConfirmAsync(ConfirmRequestDto req)
    {
        ConfirmResponseDto dto = null;
        ConfirmResponse response = null;
        string? res = null;
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(_settings.Verify, req);
            res = await httpResponse.Content.ReadAsStringAsync();
            response = JsonSerializer.Deserialize<ConfirmResponse>(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: $"[IranKishClient].[ConfirmAsync], Request: {req}, Response: {res}");
        }

        dto = new ConfirmResponseDto
        {
            Description   =  response.description,
            ResponseCode =  response.responseCode,
            IsSuccessful = response.status,
            Amount = response.result.amount,
            Status = response.status
        };

        return dto;
    }

    public bool IsSuccessful(string responseCode)
    {
        if (responseCode == "00")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public (string Data, string IV) GenerateAuthenticationEnvelope(string terminalId, long amount, string password, string publicKey)
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
}

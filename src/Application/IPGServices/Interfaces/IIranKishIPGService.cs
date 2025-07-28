namespace Application.IPGServices.Interfaces;

public interface IIranKishIPGService : IIPGService
{
    (string Data, string IV) GenerateAuthenticationEnvelope(string terminalId, long amount, string password, string publicKey);

    bool IsSuccessful(string responseCode);
}

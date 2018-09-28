namespace Gigya.Module.Core.Connector.Encryption
{
    public interface IEncryptionService
    {
        bool IsConfigured { get; }
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}

namespace M1.Api.Core
{
    public interface ICryptoOperation
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}

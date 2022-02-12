using System.IO;

namespace System.Security.Cryptography
{
    public interface IEncryptionAlgorithm : IDisposable
    {
        string Password
        {
            get;
        }

        int KeySize
        {
            get;
        }

        Stream CreateEncryptStream(Stream stream);
        Stream CreateDecryptStream(Stream stream);
    }
}

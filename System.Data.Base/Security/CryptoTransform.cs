using System.IO;

namespace System.Security.Cryptography
{
    public abstract class CryptoTransform : IEncryptionAlgorithm
    {
        public CryptoTransform(string password)
        {
            SecureKey = new Rfc2898DeriveBytes(Password = password, new byte[] { 88, 214, 241, 195, 232, 11, 37, 68 });
        }

        public readonly DeriveBytes SecureKey;

        public string Password
        {
            get;
            protected set;
        }

        public abstract bool Reusable
        {
            get;
        }

        public abstract int KeySize
        {
            get;
        }

        public abstract Stream CreateEncryptStream(Stream stream);

        public abstract Stream CreateDecryptStream(Stream stream);

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
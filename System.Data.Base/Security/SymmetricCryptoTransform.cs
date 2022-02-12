using System.IO;

namespace System.Security.Cryptography
{
    public class SymmetricCryptoTransform : CryptoTransform
    {
        protected SymmetricCryptoTransform(SymmetricAlgorithm algorithm, string password)
            : base(password)
        {
            Algorithm = algorithm;
            Algorithm.Key = SecureKey.GetBytes(Algorithm.KeySize / 8);
            Algorithm.IV = SecureKey.GetBytes(Algorithm.BlockSize / 8);

            Encryptor = Algorithm.CreateEncryptor();
            Decryptor = Algorithm.CreateDecryptor();
        }

        protected readonly SymmetricAlgorithm Algorithm;

        protected readonly ICryptoTransform Encryptor;
        protected readonly ICryptoTransform Decryptor;

        public override int KeySize
        {
            get { return Algorithm.KeySize; }
        }

        public override bool Reusable
        {
            get { return false; }
        }

        public override Stream CreateEncryptStream(Stream stream)
        {
            return new CryptoStream(stream, Encryptor, CryptoStreamMode.Write);
        }

        public override Stream CreateDecryptStream(Stream stream)
        {
            return new CryptoStream(stream, Decryptor, CryptoStreamMode.Read);
        }

        public override void Dispose()
        {
            Encryptor.Dispose();
            Decryptor.Dispose();
            Algorithm.DisposeSafely();
            base.Dispose();
        }
    }
}

namespace System.Security.Cryptography
{
    internal class AesCryptoTransform : SymmetricCryptoTransform
    {
        public AesCryptoTransform(string password)
            : base(new AesManaged(), password)
        {
        }
    }
}

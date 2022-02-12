namespace System.Security.Cryptography
{
    internal class RijndaelCryptoTransform : SymmetricCryptoTransform
    {
        public RijndaelCryptoTransform(string password)
            : base(new RijndaelManaged(), password)
        {
        }
    }
}

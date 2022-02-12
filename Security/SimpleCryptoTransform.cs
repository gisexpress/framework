using System.IO;

namespace System.Security.Cryptography
{
    internal class SimpleCryptoTransform : CryptoTransform
    {
        public SimpleCryptoTransform(string password)
            : base(password)
        {
            CryptoStream = new SimpleCryptoStream(SecureKey.GetBytes(KeySize));
        }

        protected readonly SimpleCryptoStream CryptoStream;

        public override int KeySize
        {
            get { return 32; }
        }

        public override bool Reusable
        {
            get { return true; }
        }

        public override Stream CreateEncryptStream(Stream stream)
        {
            CryptoStream.BaseStream = stream;
            return CryptoStream;
        }

        public override Stream CreateDecryptStream(Stream stream)
        {
            CryptoStream.BaseStream = stream;
            return CryptoStream;
        }

        protected class SimpleCryptoStream : Stream
        {
            public SimpleCryptoStream(byte[] key)
            {
                Key = key;
                KeyLength = key.Length;
            }

            public Stream BaseStream;

            protected readonly byte[] Key;
            protected readonly int KeyLength;

            public override bool CanSeek
            {
                get { return BaseStream.CanSeek; }
            }

            public override bool CanRead
            {
                get { return BaseStream.CanRead; }
            }

            public override bool CanWrite
            {
                get { return BaseStream.CanWrite; }
            }

            public override void Flush()
            {
                BaseStream.Flush();
            }

            public override long Length
            {
                get { return BaseStream.Length; }
            }

            public override long Position
            {
                get { return BaseStream.Position; }
                set { BaseStream.Position = value; }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return BaseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                BaseStream.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                long n = Position + offset;
                int numBytes = BaseStream.Read(buffer, offset, count);

                for (int i = offset; i < offset + numBytes; i++)
                {
                    buffer[i] = (byte)(buffer[i] ^ Key[(n + i) % KeyLength]);
                }

                return numBytes;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                long n = Position + offset;

                for (int i = offset; i < offset + count; i++)
                {
                    buffer[i] = (byte)(buffer[i] ^ Key[(n + i) % KeyLength]);
                }

                BaseStream.Write(buffer, offset, count);
            }
        }
    }
}

namespace System.Geometries.Precision
{
    public class CommonBits
    {
        long Bits;
        long SignExp;
        int MantissaBitsCount = 0x35;
        bool First = true;

        public virtual double Common
        {
            get { return BitConverter.Int64BitsToDouble(Bits); }
        }

        public virtual void Add(double value)
        {
            long n = BitConverter.DoubleToInt64Bits(value);

            if (First)
            {
                Bits = n;
                SignExp = SignExpBits(Bits);
                First = false;
            }
            else if (SignExpBits(n) != SignExp)
            {
                Bits = 0L;
            }
            else
            {
                MantissaBitsCount = NumCommonMostSigMantissaBits(Bits, n);
                Bits = ZeroLowerBits(Bits, 0x40 - (12 + MantissaBitsCount));
            }
        }

        public static int GetBit(long bits, int i)
        {
            long n = 1L << i;

            if ((bits & n) == 0L)
            {
                return 0;
            }

            return 1;
        }

        public static int NumCommonMostSigMantissaBits(long a, long b)
        {
            int n = 0;

            for (int i = 0x34; i >= 0; i--)
            {
                if (GetBit(a, i) != GetBit(b, i))
                {
                    return n;
                }

                n++;
            }

            return 0x34;
        }

        public static long SignExpBits(long n)
        {
            return n >> 0x34;
        }

        public static long ZeroLowerBits(long bits, int count)
        {
            return bits & ~((1L << count) - 1L);
        }
    }
}

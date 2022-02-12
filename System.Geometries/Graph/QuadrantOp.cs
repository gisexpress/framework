namespace System.Geometries.Graph
{
    /// <summary> 
    /// Utility functions for working with quadrants, which are numbered as follows:
    /// <para>
    /// 1 | 0
    /// --+--
    /// 2 | 3
    /// </para>
    /// </summary>
    internal class QuadrantOp
    {
        QuadrantOp()
        {
        }

        public static int CommonHalfPlane(int quad1, int quad2)
        {
            if (quad1 == quad2)
            {
                return quad1;
            }

            int num = ((quad1 - quad2) + 4) % 4;

            if (num == 2)
            {
                return -1;
            }

            int num2 = (quad1 < quad2) ? quad1 : quad2;
            int num3 = (quad1 > quad2) ? quad1 : quad2;

            if ((num2 == 0) && (num3 == 3))
            {
                return 3;
            }

            return num2;
        }

        public static bool IsInHalfPlane(int quad, int halfPlane)
        {
            if (halfPlane == 3)
            {
                if (quad != 3)
                {
                    return (quad == 0);
                }

                return true;
            }

            if (quad != halfPlane)
            {
                return (quad == (halfPlane + 1));
            }

            return true;
        }

        public static bool IsNorthern(int quad)
        {
            if (quad != 0)
            {
                return (quad == 1);
            }

            return true;
        }

        public static bool IsOpposite(int quad1, int quad2)
        {
            if (quad1 == quad2)
            {
                return false;
            }

            return ((((quad1 - quad2) + 4) % 4) == 2);
        }

        public static bool TryGetQuadrant(double dx, double dy, out int value)
        {
            if (dx.IsZero() && dy.IsZero())
            {
                value = 0;
                return false;
            }

            if (dx >= 0.0)
            {
                if (dy >= 0.0)
                {
                    value = 0;
                }
                else
                {
                    value = 3;
                }
            }
            else if (dy >= 0.0)
            {
                value = 1;
            }
            else
            {
                value = 2;
            }

            return true;
        }
    }
}

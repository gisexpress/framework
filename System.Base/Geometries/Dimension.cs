using System;

namespace System.Geometries
{
    /// <summary>
    /// Provides constants representing the dimensions of a point, a curve and a surface.
    /// </summary>
    /// <remarks>
    /// Also provides constants representing the dimensions of the empty geometry and
    /// non-empty geometries, and the wildcard constant <see cref="Dontcare"/> meaning "any dimension".
    /// These constants are used as the entries in <see cref="IntersectionMatrix"/>s.
    /// </remarks>
    public enum Dimensions
    {
        /// <summary>
        /// Dimension value of a point (0).
        /// </summary>
        Point = 0,

        /// <summary>
        /// Dimension value of a curve (1).
        /// </summary>
        Curve = 1,

        /// <summary>
        /// Dimension value of a surface (2).
        /// </summary>
        Surface = 2,

        /// <summary>
        /// Dimension value of a empty point (-1).
        /// </summary>
        False = -1,

        /// <summary>
        /// Dimension value of non-empty geometries (= {Point,Curve,A}).
        /// </summary>
        True = -2,

        /// <summary>
        /// Dimension value for any dimension (= {False, True}).
        /// </summary>
        Dontcare = -3
    }

    /// <summary>
    /// Class containing static methods for conversions
    /// between dimension values and characters.
    /// </summary>
    public class DimensionUtility
    {
        /// <summary>
        /// Symbol for the FALSE pattern matrix entry
        /// </summary>
        public const char SymFalse = 'F';
  
        /// <summary>
        /// Symbol for the TRUE pattern matrix entry
        /// </summary>
        public const char SymTrue = 'T';
  
        /// <summary>
        /// Symbol for the DONTCARE pattern matrix entry
        /// </summary>
        public const char SymDontcare = '*';
  
        /// <summary>
        /// Symbol for the P (dimension 0) pattern matrix entry
        /// </summary>
        public const char SymP = '0';
  
        /// <summary>
        /// Symbol for the L (dimension 1) pattern matrix entry
        /// </summary>
        public const char SymL = '1';
  
        /// <summary>
        /// Symbol for the A (dimension 2) pattern matrix entry
        /// </summary>
        public const char SymA = '2';
  
        
        
        /// <summary>
        /// Converts the dimension value to a dimension symbol,
        /// for example, <c>True => 'T'</c>
        /// </summary>
        /// <param name="dimensionValue">Number that can be stored in the <c>IntersectionMatrix</c>.
        /// Possible values are <c>True, False, Dontcare, 0, 1, 2</c>.</param>
        /// <returns>Character for use in the string representation of an <c>IntersectionMatrix</c>.
        /// Possible values are <c>T, F, * , 0, 1, 2</c>.</returns>
        public static char ToDimensionSymbol(Dimensions dimensionValue)
        {
            switch (dimensionValue)
            {
                case Dimensions.False:
                    return SymFalse;
                case Dimensions.True:
                    return SymTrue;
                case Dimensions.Dontcare:
                    return SymDontcare;
                case Dimensions.Point:
                    return SymP;
                case Dimensions.Curve:
                    return SymL;
                case Dimensions.Surface:
                    return SymA;
                default:
                    throw new ArgumentOutOfRangeException
                        ("Unknown dimension value: " + dimensionValue);
            }
        }

        /// <summary>
        /// Converts the dimension symbol to a dimension value,
        /// for example, <c>'*' => Dontcare</c>
        /// </summary>
        /// <param name="dimensionSymbol">Character for use in the string representation of an <c>IntersectionMatrix</c>.
        /// Possible values are <c>T, F, * , 0, 1, 2</c>.</param>
        /// <returns>Number that can be stored in the <c>IntersectionMatrix</c>.
        /// Possible values are <c>True, False, Dontcare, 0, 1, 2</c>.</returns>
        public static Dimensions ToDimensionValue(char dimensionSymbol)
        {
            switch (Char.ToUpper(dimensionSymbol))
            {
                case SymFalse:
                    return Dimensions.False;
                case SymTrue:
                    return Dimensions.True;
                case SymDontcare:
                    return Dimensions.Dontcare;
                case SymP:
                    return Dimensions.Point;
                case SymL:
                    return Dimensions.Curve;
                case SymA:
                    return Dimensions.Surface;
                default:
                    throw new ArgumentOutOfRangeException
                        ("Unknown dimension symbol: " + dimensionSymbol);
            }
        }
    }
}

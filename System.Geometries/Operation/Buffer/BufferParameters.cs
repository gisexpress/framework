namespace System.Geometries.Operation.Buffer
{
    internal class BufferParameters : IBufferParameters
    {
        public static BufferParameters Default = new BufferParameters();

        /// <summary>
        /// The default number of facets into which to divide a fillet of 90 degrees.
        /// A value of 8 gives less than 2% max error in the buffer distance.
        /// For a max error of &lt; 1%, use QS = 12.
        /// For a max error of &lt; 0.1%, use QS = 18.
        /// </summary>
        public const int DefaultQuadrantSegments = 8;

        /// <summary>
        /// The default mitre limit
        /// Allows fairly pointy mitres.
        /// </summary>
        public const double DefaultMitreLimit = 5.0;

        /// <summary>
        /// The default simplify factor.
        /// Provides an accuracy of about 1%, which matches
        /// the accuracy of the <see cref="DefaultQuadrantSegments"/> parameter.
        /// </summary>
        public const double DefaultSimplifyFactor = 0.01;

        public BufferParameters()
            : this(JoinStyle.Round, EndCapStyle.Round, DefaultQuadrantSegments, DefaultMitreLimit)
        {
        }

        public BufferParameters(JoinStyle joinStyle)
            : this(joinStyle, EndCapStyle.Round, DefaultQuadrantSegments, DefaultMitreLimit)
        {
        }

        public BufferParameters(JoinStyle joinStyle, EndCapStyle endCapStyle)
            : this(joinStyle, endCapStyle, DefaultQuadrantSegments, DefaultMitreLimit)
        {
        }

        public BufferParameters(JoinStyle joinStyle, EndCapStyle endCapStyle, int quadrantSegments)
            : this(joinStyle, endCapStyle, quadrantSegments, DefaultMitreLimit)
        {
        }

        public BufferParameters(JoinStyle joinStyle, EndCapStyle endCapStyle, int quadrantSegments, double mitreLimit)
        {
            JoinStyle = joinStyle;
            EndCapStyle = endCapStyle;
            MitreLimit = mitreLimit;
            QuadrantSegments = quadrantSegments;
            SimplifyFactor = DefaultSimplifyFactor;

            if (JoinStyle == JoinStyle.Miter && MitreLimit.IsZero())
            {
                MitreLimit = DefaultMitreLimit;
            }

            if (JoinStyle != JoinStyle.Round)
            {
                QuadrantSegments = DefaultQuadrantSegments;
            }

            if (QuadrantSegments == 0)
            {
                QuadrantSegments = DefaultQuadrantSegments;
            }

            if (QuadrantSegments < 0)
            {
                JoinStyle = JoinStyle.Miter;
                MitreLimit = Math.Abs(QuadrantSegments);
                QuadrantSegments = 1;
            }
        }

        ///<summary>
        /// Gets/sets the number of quadrant segments which will be used
        ///</summary>
        /// <remarks>
        /// QuadrantSegments is the number of line segments used to approximate an angle fillet.
        /// <list type="Table">
        /// <item><c>QuadrantSegments</c> &gt;>= 1</item><description>joins are round, and <c>QuadrantSegments</c> indicates the number of segments to use to approximate a quarter-circle.</description>
        /// <item><c>QuadrantSegments</c> = 0</item><description>joins are beveled</description>
        /// <item><c>QuadrantSegments</c> &lt; 0</item><description>joins are mitred, and the value of qs indicates the mitre ration limit as <c>mitreLimit = |<tt>QuadrantSegments</tt>|</c></description>
        /// </list>
        /// For round joins, <c>QuadrantSegments</c> determines the maximum
        /// error in the approximation to the true buffer curve.
        /// The default value of 8 gives less than 2% max error in the buffer distance.
        /// For a max error of &lt; 1%, use QS = 12.
        /// For a max error of &lt; 0.1%, use QS = 18.
        /// The error is always less than the buffer distance
        /// (in other words, the computed buffer curve is always inside the true
        /// curve).
        /// </remarks>
        public int QuadrantSegments
        {
            get;
            set;
        }

        ///<summary>
        /// Computes the maximum distance error due to a given level of approximation to a true arc.
        ///</summary>
        /// <param name="quadSegs">The number of segments used to approximate a quarter-circle</param>
        /// <returns>The error of approximation</returns>
        public static double BufferDistanceError(int quadSegs)
        {
            double alpha = Math.PI / 2.0 / quadSegs;
            return 1 - Math.Cos(alpha / 2.0);
        }

        ///<summary>
        /// Gets/Sets the end cap style of the generated buffer.
        ///</summary>
        /// <remarks>
        /// <para>
        /// The styles supported are <see cref="GeoAPI.Operations.Buffer.EndCapStyle.Round"/>, 
        /// <see cref="GeoAPI.Operations.Buffer.EndCapStyle.Flat"/>, and 
        /// <see cref="GeoAPI.Operations.Buffer.EndCapStyle.Square"/>.
        /// </para>
        /// <para>The default is <see cref="GeoAPI.Operations.Buffer.EndCapStyle.Round"/>.</para>
        /// </remarks>
        public EndCapStyle EndCapStyle
        {
            get;
            set;
        }

        ///<summary>
        /// Gets/Sets the join style for outside (reflex) corners between line segments.
        ///</summary>
        /// <remarks>
        /// <para>Allowable values are <see cref="GeoAPI.Operations.Buffer.JoinStyle.Round"/> (which is the default), 
        /// <see cref="GeoAPI.Operations.Buffer.JoinStyle.Mitre"/> and <see cref="GeoAPI.Operations.Buffer.JoinStyle.Bevel"/></para>
        /// </remarks>
        public JoinStyle JoinStyle
        {
            get;
            set;
        }

        ///<summary>
        /// Sets the limit on the mitre ratio used for very sharp corners.
        ///</summary>
        /// <remarks>
        /// <para>
        /// The mitre ratio is the ratio of the distance from the corner
        /// to the end of the mitred offset corner.
        /// When two line segments meet at a sharp angle,
        /// a miter join will extend far beyond the original geometry.
        /// (and in the extreme case will be infinitely far.)
        /// To prevent unreasonable geometry, the mitre limit
        /// allows controlling the maximum length of the join corner.
        /// Corners with a ratio which exceed the limit will be beveled.
        /// </para>
        /// </remarks>
        public double MitreLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the computed buffer should be single-sided.
        /// A single-sided buffer is constructed on only one side of each input line.
        /// <para>
        /// The side used is determined by the sign of the buffer distance:
        /// <list type="Bullet">
        /// <item>a positive distance indicates the left-hand side</item>
        /// <item>a negative distance indicates the right-hand side</item>
        /// </list>
        /// The single-sided buffer of point geometries is  the same as the regular buffer.
        /// </para><para>
        /// The End Cap Style for single-sided buffers is always ignored,
        /// and forced to the equivalent of <see cref="GeoAPI.Operations.Buffer.EndCapStyle.Flat"/>.
        /// </para>
        /// </summary>
        public bool SingleSide
        {
            get;
            set;
        }

        /// <summary>
        /// Factor used to determine the simplify distance tolerance
        /// for input simplification.
        /// Simplifying can increase the performance of computing buffers.
        /// Generally the simplify factor should be greater than 0.
        /// Values between 0.01 and .1 produce relatively good accuracy for the generate buffer.
        /// Larger values sacrifice accuracy in return for performance.
        /// </summary>
        public double SimplifyFactor
        {
            get;
            set;
        }
    }
}
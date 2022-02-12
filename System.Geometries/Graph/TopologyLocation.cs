using System.Text;

namespace System.Geometries.Graph
{
    /// <summary> 
    /// A TopologyLocation is the labelling of a
    /// GraphComponent's topological relationship to a single Geometry.
    /// </summary>
    /// <remarks>
    /// If the parent component is an area edge, each side and the edge itself
    /// have a topological location.  These locations are named:
    /// <list type="Table">
    /// <item>On</item><description>on the edge</description> 
    /// <item>Left</item><description>left-hand side of the edge</description>
    /// <item>Right</item><description>right-hand side</description>
    /// </list>
    /// <para>
    /// If the parent component is a line edge or node, there is a single
    /// topological relationship attribute, On.</para>
    /// <para>
    /// The possible values of a topological location are
    /// { <see cref="Locations.Null"/>, <see cref="Locations.Exterior"/>, <see cref="Locations.Boundary"/>, <see cref="Locations.Interior"/> }</para>
    /// <para>
    /// The labelling is stored in an array _location[j] where
    /// where j has the values On, Left, Right.
    /// </para>
    /// </remarks>
    public class TopologyLocation 
    {
        private Locations[] _location;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        public TopologyLocation(Locations[] location)
        {
            Init(location.Length);
        }

        /// <summary> 
        /// Constructs a TopologyLocation specifying how points on, to the left of, and to the
        /// right of some GraphComponent relate to some Geometry. Possible values for the
        /// parameters are Location.Null, Location.Exterior, Location.Boundary, 
        /// and Location.Interior.
        /// </summary>        
        /// <param name="on"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public TopologyLocation(Locations on, Locations left, Locations right) 
        {
            Init(3);
            _location[(int) Positions.On] = on;
            _location[(int) Positions.Left] = left;
            _location[(int) Positions.Right] = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="on"></param>
        public TopologyLocation(Locations on) 
        {
            Init(1);
            _location[(int) Positions.On] = on;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gl"></param>
        public TopologyLocation(TopologyLocation gl) 
        {
            if (gl == null)
                throw new ArgumentNullException("gl", "null topology location specified");

            Init(gl._location.Length);
            for (int i = 0; i < _location.Length; i++) 
                _location[i] = gl._location[i];                            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        private void Init(int size)
        {
            _location = new Locations[size];
            SetAllLocations(Locations.Null);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="posIndex"></param>
        /// <returns></returns>
        public  Locations Get(Positions posIndex)
        {
            int index = (int)posIndex;
            if (index < _location.Length)
                return _location[index];
            return Locations.Null;
        }

        /// <summary>
        /// Get calls Get(Positions posIndex),
        /// Set calls SetLocation(Positions locIndex, Location locValue)
        /// </summary>
        /// <param name="posIndex"></param>
        /// <returns></returns>
        public  Locations this[Positions posIndex]
        {
            get
            {
                return Get(posIndex);
            }
            set
            {
                SetLocation(posIndex, value);
            }
        }

        /// <returns>
        /// <c>true</c> if all locations are Null.
        /// </returns>
        public  bool IsNull
        {
            get
            {
                for (int i = 0; i < _location.Length; i++)
                    if (_location[i] != Locations.Null) 
                        return false;
                return true;
            }
        }

        /// <returns> 
        /// <c>true</c> if any locations are Null.
        /// </returns>
        public  bool IsAnyNull
        {
            get
            {
                for (int i = 0; i < _location.Length; i++)
                    if (_location[i] == Locations.Null) 
                        return true;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="le"></param>
        /// <param name="locIndex"></param>
        /// <returns></returns>
        public  bool IsEqualOnSide(TopologyLocation le, int locIndex)
        {
            return _location[locIndex] == le._location[locIndex];
        }

        /// <summary>
        /// 
        /// </summary>
        public  bool IsArea
        {
            get
            {
                return _location.Length > 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public  bool IsLine
        {
            get
            {
                return _location.Length == 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public  void Flip()
        {
            if (_location.Length <= 1) 
                return;
            Locations temp = _location[(int)Positions.Left];
            _location[(int)Positions.Left] = _location[(int)Positions.Right];
            _location[(int)Positions.Right] = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locValue"></param>
        public  void SetAllLocations(Locations locValue)
        {
            for (int i = 0; i < _location.Length; i++) 
                _location[i] = locValue;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locValue"></param>
        public  void SetAllLocationsIfNull(Locations locValue)
        {
            for (int i = 0; i < _location.Length; i++) 
                if (_location[i] == Locations.Null) 
                    _location[i] = locValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locIndex"></param>
        /// <param name="locValue"></param>
        public  void SetLocation(Positions locIndex, Locations locValue)
        {
            _location[(int)locIndex] = locValue;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locValue"></param>
        public  void SetLocation(Locations locValue)
        {
            SetLocation(Positions.On, locValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  Locations[] GetLocations() 
        {
            return _location; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="on"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public  void SetLocations(Locations on, Locations left, Locations right) 
        {
            _location[(int)Positions.On] = on;
            _location[(int)Positions.Left] = left;
            _location[(int)Positions.Right] = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gl"></param>
        public  void SetLocations(TopologyLocation gl) 
        {
            for (int i = 0; i < gl._location.Length; i++) 
                _location[i] = gl._location[i];            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public  bool AllPositionsEqual(Locations loc)
        {
            for (int i = 0; i < _location.Length; i++) 
                if (_location[i] != loc) 
                    return false;
            return true;
        }

        /// <summary>
        /// Merge updates only the Null attributes of this object
        /// with the attributes of another.
        /// </summary>
        public  void Merge(TopologyLocation gl)
        {
            // if the src is an Area label & and the dest is not, increase the dest to be an Area
            if (gl._location.Length > _location.Length) 
            {
                Locations[] newLoc = new Locations[3];
                newLoc[(int)Positions.On] = _location[(int)Positions.On];
                newLoc[(int)Positions.Left] = Locations.Null;
                newLoc[(int)Positions.Right] = Locations.Null;
                _location = newLoc;
            }
            for (int i = 0; i < _location.Length; i++) 
                if (_location[i] == Locations.Null && i < gl._location.Length)
                    _location[i] = gl._location[i];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_location.Length > 1)
                sb.Append(LocationUtility.ToLocationSymbol(_location[(int)Positions.Left]));
            sb.Append(LocationUtility.ToLocationSymbol(_location[(int)Positions.On]));
            if (_location.Length > 1)
                sb.Append(LocationUtility.ToLocationSymbol(_location[(int)Positions.Right]));
            return sb.ToString();
        }
    }
}

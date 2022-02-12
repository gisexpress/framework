using System.Collections.Generic;
using System.ComponentModel;

namespace System.Geometries
{
    public interface IGeometryFactory : ITypeFactory
    {
        event PointVisitedEventHandler PointVisited;

        double NorthAngle
        {
            get; set;
        }

        MeasuringSystems MeasuringSystem
        {
            get; set;
        }

        double GetAzimuth(ICoordinate point, ICoordinate other);

        IGeometry BuildGeometry(IEnumerable<IGeometry> geometries);
    }
}

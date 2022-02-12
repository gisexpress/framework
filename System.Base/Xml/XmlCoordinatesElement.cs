using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace System.Geometries
{
    public class XmlCoordinatesElement : XmlElementBase
    {
        protected internal XmlCoordinatesElement(XmlDocument doc) : base(string.Empty, Constants.Xml.Coordinates, Constants.Xml.NamespaceURI, doc)
        {
        }

        protected byte Flag;
        protected IEnvelope Bounds;
        protected double Length, Clockwise, SignedArea;

        double BaseX, BaseY, BaseZ;
        public double CenterX, CenterY, CenterZ, AreaSum = double.NaN;

        protected List<ICoordinate> List;

        public virtual new bool IsEmpty()
        {
            return Count == 0 || StartPoint.IsEmpty();
        }

        public void Clear()
        {
            ClearItems();
        }

        public virtual int Count
        {
            get { return Items.Count; }
        }

        public virtual ICoordinate StartPoint
        {
            get { return Items.FirstOrDefault(); }
        }

        public virtual ICoordinate EndPoint
        {
            get { return Items.LastOrDefault(); }
        }

        public virtual bool IsRing()
        {
            return ParentNode is ILinearRing;
        }

        public virtual bool IsClosed
        {
            get { return Items.Count > 2 && StartPoint.IsEquivalent(EndPoint); }
            set
            {
                if (Items.Count > 2 && IsClosed != value)
                {
                    if (value)
                    {
                        Close();
                    }
                    else
                    {
                        Flag++;
                        RemoveItem(Count - 1);
                        Flag--;
                    }
                }
            }
        }

        public bool Remove(ICoordinate c)
        {
            int i = IndexOf(c);

            if (i >= 0)
            {
                RemoveAt(i);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            RemoveItem(index);
        }

        public int IndexOf(ICoordinate c)
        {
            return Items.IndexOf(c);
        }

        protected List<ICoordinate> Items
        {
            get
            {
                if (List == null)
                {
                    List = new List<ICoordinate>();
                    LoadCoordinates();
                }

                return List;
            }
        }

        protected virtual void ClearItems()
        {
            Length = 0.0;
            Clockwise = 0.0;
            SignedArea = 0.0;
            CenterX = 0.0;
            CenterY = 0.0;
            CenterZ = 0.0;
            BaseX = 0.0;
            BaseY = 0.0;
            AreaSum = double.NaN;
            Bounds = null;

            List.Clear();
        }

        public void Set(int index, params double[] values)
        {
            Set(Items[index], values);
        }

        public void Set(ICoordinate item, params double[] values)
        {
            if (Flag == 0 && item.IsEmpty() == false)
            {
                int index = Items.IndexOf(item);

                Debug.Assert(index >= 0, "InvalidOperationException");

                if (index >= 0)
                {
                    Debug.Assert(ReferenceEquals(item, List[index]), "InvalidOperationException");

                    bool wasClosed = index == 0 && IsClosed;

                    ICoordinate previous = index > 0 ? List[index - 1] : null;
                    ICoordinate next = index < Count - 1 ? List[index + 1] : null;

                    if (index >= 0)
                    {
                        Flag++;

                        if (Count > 2)
                        {
                            if (previous.HasValue() && next.HasValue())
                            {
                                Length -= previous.Distance(item);
                                Length -= item.Distance(next);

                                Clockwise -= ItemClockwise(previous, item);
                                Clockwise -= ItemClockwise(item, next);

                                SignedArea -= ItemSignedArea(previous, item);
                                SignedArea -= ItemSignedArea(item, next);

                                ItemCentroid(previous, item, false);
                                ItemCentroid(item, next, false);
                            }
                            else if (next.HasValue())
                            {
                                Length -= item.Distance(next);
                                Clockwise -= ItemClockwise(item, next);
                                SignedArea -= ItemSignedArea(item, next);

                                ItemCentroid(item, next, false);
                            }
                            else if (previous.HasValue())
                            {
                                Length -= previous.Distance(item);
                                Clockwise -= ItemClockwise(previous, item);
                                SignedArea -= ItemSignedArea(previous, item);

                                ItemCentroid(previous, item, false);
                            }

                            item.SetValues(values);

                            if (previous.HasValue() && next.HasValue())
                            {
                                Length += previous.Distance(item);
                                Length += item.Distance(next);

                                Clockwise += ItemClockwise(previous, item);
                                Clockwise += ItemClockwise(item, next);

                                SignedArea += ItemSignedArea(previous, item);
                                SignedArea += ItemSignedArea(item, next);

                                ItemCentroid(previous, item, true);
                                ItemCentroid(item, next, true);
                            }
                            else if (next.HasValue())
                            {
                                Length += item.Distance(next);
                                Clockwise += ItemClockwise(item, next);
                                SignedArea += ItemSignedArea(item, next);

                                ItemCentroid(item, next, true);
                            }
                            else if (previous.HasValue())
                            {
                                Length += previous.Distance(item);
                                Clockwise += ItemClockwise(previous, item);
                                SignedArea += ItemSignedArea(previous, item);

                                ItemCentroid(previous, item, true);
                            }
                        }
                        else if (Count == 2)
                        {
                            AreaSum = double.NaN;
                            item.SetValues(values);
                            Length = StartPoint.Distance(EndPoint);
                            Clockwise = ItemClockwise(StartPoint, EndPoint);
                            SignedArea = ItemSignedArea(StartPoint, EndPoint);
                            ItemCentroid(StartPoint, EndPoint, true);
                        }

                        Flag--;

                        if (wasClosed && ReferenceEquals(item, EndPoint) == false)
                        {
                            EndPoint.SetValues(values);
                        }

                        OnChanged();
                    }
                }
            }
        }

        protected virtual void InsertItem(int index, ICoordinate c)
        {
            if (c.IsEquivalent(EndPoint))
            {
                return;
            }

            if (index > 0)
            {
                ICoordinate previous = List[index - 1];

                Length += previous.Distance(c);
                Clockwise += ItemClockwise(previous, c);
                SignedArea += ItemSignedArea(previous, c);
                ItemCentroid(previous, c, true);
            }

            List.Add(c);
        }

        protected void RemoveItem(int index)
        {
            ICoordinate c = List[index];
            ICoordinate p = index > 0 ? List[index - 1] : null;
            ICoordinate n = index < Count - 1 ? List[index + 1] : null;

            if (Flag == 0 && IsClosed)
            {
                Flag++;

                if (p == null)
                {
                    ICoordinate c0 = List[Count - 2];
                    ICoordinate c1 = List[Count - 1];

                    Length -= c0.Distance(c1);
                    Clockwise -= ItemClockwise(c0, c1);
                    SignedArea -= ItemSignedArea(c0, c1);
                    ItemCentroid(c0, c1, false);

                    List[1].CopyTo(c1);

                    Length += c0.Distance(c1);
                    Clockwise += ItemClockwise(c0, c1);
                    SignedArea += ItemSignedArea(c0, c1);
                    ItemCentroid(c0, c1, true);
                }
                else if (n == null)
                {
                    ICoordinate c0 = List[0];
                    ICoordinate c1 = List[1];

                    Length -= c0.Distance(c1);
                    Clockwise -= ItemClockwise(c0, c1);
                    SignedArea -= ItemSignedArea(c0, c1);
                    ItemCentroid(c0, c1, false);

                    List[Count - 2].CopyTo(c0);

                    Length += c0.Distance(c1);
                    Clockwise += ItemClockwise(c0, c1);
                    SignedArea += ItemSignedArea(c0, c1);
                    ItemCentroid(c0, c1, true);
                }

                Flag--;
            }

            if (p.HasValue() && n.HasValue())
            {
                Length -= p.Distance(c);
                Length -= c.Distance(n);
                Length += p.Distance(n);

                Clockwise -= ItemClockwise(p, c);
                Clockwise -= ItemClockwise(c, n);
                Clockwise += ItemClockwise(p, n);

                SignedArea -= ItemSignedArea(p, c);
                SignedArea -= ItemSignedArea(c, n);
                SignedArea += ItemSignedArea(p, n);

                ItemCentroid(p, c, false);
                ItemCentroid(c, n, false);
                ItemCentroid(p, n, true);
            }
            else if (n.HasValue())
            {
                Length -= c.Distance(n);
                Clockwise -= ItemClockwise(c, n);
                SignedArea -= ItemSignedArea(c, n);
                ItemCentroid(c, n, false);
            }
            else if (p.HasValue())
            {
                Length -= p.Distance(c);
                Clockwise -= ItemClockwise(p, c);
                SignedArea -= ItemSignedArea(p, c);
                ItemCentroid(p, c, false);
            }

            List.RemoveAt(index);
        }

        protected virtual void OnChanged()
        {
            Bounds = default;
        }

        void Close()
        {
            if (IsClosed)
            {
                return;
            }

            InsertItem(Count, StartPoint.Clone());
        }

        void LoadCoordinates()
        {
            int n = 0;
            ICoordinate c;

            foreach (string coorText in InnerText.SplitWithoutEmptyEntries(' ', '\n', '\r', '\t'))
            {
                c = OwnerDocument.Factory.Create<ICoordinate>();
                c.SetValues(coorText.SplitWithoutEmptyEntries(',').Select(value => XmlConvert.ToDouble(value)).ToArray());
                InsertItem(n++, c);
            }
        }

        protected double ItemClockwise(ICoordinate c, ICoordinate other)
        {
            if (c.IsEmpty() || other.IsEmpty())
            {
                return 0.0;
            }

            return (other.X - c.X) * (other.Y + c.Y);
        }

        protected double ItemSignedArea(ICoordinate c, ICoordinate other)
        {
            if (c.IsNull() || other.IsNull())
            {
                return 0.0;
            }

            if (c.IsEmpty() || other.IsEmpty())
            {
                return 0.0;
            }

            return (c.X + other.X) * (other.Y - c.Y);
        }

        protected void ItemCentroid(ICoordinate c, ICoordinate other, bool append)
        {
            if (c.IsEmpty() || other.IsEmpty())
            {
                return;
            }

            if (IsRing())
            {
                if (double.IsNaN(AreaSum))
                {
                    CenterX = 0.0;
                    CenterY = 0.0;
                    CenterZ = 0.0;
                    BaseX = StartPoint.X;
                    BaseY = StartPoint.Y;
                    BaseZ = StartPoint.Z;
                    AreaSum = 0.0;
                }

                double x = BaseX + c.X + other.X;
                double y = BaseY + c.Y + other.Y;
                double z = BaseZ + c.Z + other.Z;
                double area = TriangleArea(BaseX, BaseY, c, other);

                if (append)
                {
                    AreaSum += area;
                    CenterX += x * area;
                    CenterY += y * area;
                    CenterZ += z * area;
                }
                else
                {
                    AreaSum -= area;
                    CenterX -= x * area;
                    CenterY -= y * area;
                    CenterZ -= z * area;
                }
            }
        }

        static double TriangleArea(double x, double y, ICoordinate p2, ICoordinate p3)
        {
            return (((p2.X - x) * (p3.Y - y)) - ((p3.X - x) * (p2.Y - y)));
        }
    }
}
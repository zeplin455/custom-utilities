using System;
using System.Collections.Generic;
using System.Text;

namespace ZUtils.Utilities
{
    public class VertexDistance : IComparable
    {
        public int Side1
        {
            get
            {
                if (Source > Destination)
                {
                    return Destination;
                }
                else
                {
                    return Source;
                }
            }
        }

        public int Side2
        {
            get
            {
                if (Source > Destination)
                {
                    return Source;
                }
                else
                {
                    return Destination;
                }
            }
        }

        public ulong Id
        {
            get
            {
                return (((ulong)Side1) << 32) | ((ulong)Side2 & 0xffffffffL);
            }
        }

        public int Source;
        public int Destination;
        public double Distance;

        public List<VertexDistance> ConnectedSide1 = new List<VertexDistance>();
        public List<VertexDistance> ConnectedSide2 = new List<VertexDistance>();

        public int CompareTo(object obj)
        {
            VertexDistance Casted = (VertexDistance)obj;

            if (Side1 == Casted.Side1)
            {
                if (Side2 == Casted.Side2)
                {
                    return 0;
                }
                else
                {
                    return Side2.CompareTo(Casted.Side2);
                }
            }
            else
            {
                return Side1.CompareTo(Casted.Side1);
            }
        }

        public override string ToString()
        {
            if (Source < Destination)
            {
                return $"{Source}|{Destination}|{Distance}";
            }
            else
            {
                return $"{Destination}|{Source}|{Distance}";
            }
        }
    }
}

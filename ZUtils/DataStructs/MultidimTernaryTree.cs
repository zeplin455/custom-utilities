using System;
using System.Collections.Generic;
using System.Text;

namespace ZUtils.DataStructs
{
    public class MultidimTernaryTree<T>
    {

        public class Node
        {
            public T Value = default(T);
            public Node[] Children = null;
        }

        public Node Root;
        private int DimDepth;
        private int Dims;
        private long CellSize;
        private long Center;

        public MultidimTernaryTree(int dimensions)
        {
            DimDepth = 0;
            Dims = dimensions;

            CellSize = IntPow(3, Dims);
            
            for (int i = 0; i < Dims - 1; ++i)
            {
                Center += 1 * 3;
            }

            Center += 1;
        }

        public long GetSize()
        {
            //To save on one level
            return IntPow(3, DimDepth + 1);
        }

        private bool CheckSize(long[] coords)
        {
            long Bounds = GetSize() / 2;

            for (int i = 0; i < coords.Length; ++i)
            {
                if(coords[i] > Bounds || coords[i] < -Bounds)
                {
                    return false;
                }
            }
            return true;
        }

        private void CheckSizeResize(long[] coords)
        {
            bool ValidBounds = false;

            do
            {
                long Bounds = GetSize() / 2;

                ValidBounds = true;

                for (int i = 0; i < coords.Length; ++i)
                {
                    if (coords[i] > Bounds || coords[i] < -Bounds)
                    {
                        //Resize by creating a new node, making it the new parent and setting the old parent as its center child.
                        Node newParent = new Node();
                        newParent.Children = new Node[CellSize];
                        newParent.Children[Center] = Root;
                        Root = newParent;
                        DimDepth++;
                        ValidBounds = false;
                        break;
                    }
                }
            }
            while (!ValidBounds);

        }

        private static long IntPow(long x, long pow)
        {
            long ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        public void Add(T _Value, long[] coords)
        {
            CheckSizeResize(coords);
            InternalAdd(_Value, coords, Root, DimDepth);
        }

        public T Get(long[] coords)
        {
            if (CheckSize(coords))
            {
                return InternalGet(coords, Root, DimDepth);
            }
            else
            {
                return default(T);
            }
        }

        private void InternalAdd(T _Value, long[] coords, Node _Node, int _Depth = 1)
        {
            if (_Depth > 0)
            {
                if (_Node.Children == null)
                {
                    _Node.Children = new Node[CellSize];
                }

                long Block = IntPow(3, _Depth);
                long Dim = (Block - 1) / 2;

                long[] divs = new long[Dims];

                if (Dim > 1)
                {
                    for(int i = 0; i < Dims; ++i)
                    {
                        divs[i] = coords[i] == 0 ? 0 : coords[i] > 0 ? (coords[i] - 1) / Dim : (coords[i] + 1) / Dim;
                    }
                }
                else
                {
                    for (int i = 0; i < Dims; ++i)
                    {
                        divs[i] = coords[i] < 0 ? coords[i] + 1 : coords[i] > 0 ? coords[i] - 1 : 0;
                    }
                }

                long[] dirs = new long[Dims];
                long offset = 0;

                for (int i = 0; i < Dims; ++i)
                {
                    dirs[i] = divs[i] > 0 ? 2 : divs[i] < 0 ? 0 : 1;                    
                }

                for( int i = 0; i < Dims - 1; ++i)
                {
                    offset += dirs[i] * 3;
                }

                offset += dirs[dirs.Length - 1];
                
                if (_Node.Children[offset] == null)
                {
                    _Node.Children[offset] = new Node();
                }

                long[] newCoords = new long[Dims];

                for(int i = 0; i < Dims; ++i)
                {
                    newCoords[i] = dirs[i] > 1 ? coords[i] - (int)Block : dirs[i] < 1 ? coords[i] + (int)Block : coords[i];
                }

                InternalAdd(_Value,
                    newCoords,
                    _Node.Children[offset],
                    _Depth - 1);
            }
            else
            {
                if (_Node.Children == null)
                {
                    _Node.Children = new Node[CellSize];
                }

                long[] c = new long[Dims];
                long offset = 0;

                for ( int i = 0; i < Dims; ++i)
                {
                    c[i] = coords[i] + 1;
                }

                for (int i = 0; i < Dims - 1; ++i)
                {
                    offset += c[i] * 3;
                }

                offset += c[c.Length - 1];

                if (_Node.Children[offset] == null)
                {
                    _Node.Children[offset] = new Node();
                }
                _Node.Children[offset].Value = _Value;
            }
        }

        private T InternalGet(long[] coords, Node _Node, int _Depth = 3)
        {
            if (_Depth > 0)
            {
                if (_Node.Children == null)
                {
                    return default(T);
                }

                long Block = IntPow(3, _Depth);
                long Dim = (Block - 1) / 2;

                long[] divs = new long[Dims];

                if (Dim > 1)
                {
                    for (int i = 0; i < Dims; ++i)
                    {
                        divs[i] = coords[i] == 0 ? 0 : coords[i] > 0 ? (coords[i] - 1) / Dim : (coords[i] + 1) / Dim;
                    }
                }
                else
                {
                    for (int i = 0; i < Dims; ++i)
                    {
                        divs[i] = coords[i] < 0 ? coords[i] + 1 : coords[i] > 0 ? coords[i] - 1 : 0;
                    }
                }

                long[] dirs = new long[Dims];
                long offset = 0;

                for (int i = 0; i < Dims; ++i)
                {
                    dirs[i] = divs[i] > 0 ? 2 : divs[i] < 0 ? 0 : 1;
                }

                for (int i = 0; i < Dims - 1; ++i)
                {
                    offset += dirs[i] * 3;
                }

                offset += dirs[dirs.Length - 1];

                if (_Node.Children[offset] == null)
                {
                    return default(T);
                }

                long[] newCoords = new long[Dims];

                for (int i = 0; i < Dims; ++i)
                {
                    newCoords[i] = dirs[i] > 1 ? coords[i] - (int)Block : dirs[i] < 1 ? coords[i] + (int)Block : coords[i];
                }

                return InternalGet(
                    newCoords,
                    _Node.Children[offset],
                    _Depth - 1);
            }
            else
            {
                if (_Node.Children == null)
                {
                    return default(T);
                }

                long[] c = new long[Dims];
                long offset = 0;

                for (int i = 0; i < Dims; ++i)
                {
                    c[i] = coords[i] + 1;
                }

                for (int i = 0; i < Dims - 1; ++i)
                {
                    offset += (c[i]) * 3;
                }

                offset += c[c.Length - 1];

                if (_Node.Children[offset] == null)
                {
                    return default(T);
                }

                if (_Node.Children[offset].Value == null)
                {
                    return default(T);
                }
                else
                {
                    return _Node.Children[offset].Value;
                }
            }
        }
    }
}

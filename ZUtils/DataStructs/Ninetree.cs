namespace ZUtils.DataStructs
{
    /// <summary>
    /// Like an Octree but different
    /// Instead of dividing space into 8 blocks this takes the approach of 
    /// dividing space into 27 blocks having cells of 3x3x3 representing a center point and 
    /// the 26 possible directions from that point using integer coordinates. (Right,Left,Up, Down, Up-Right, Up-Front-Right, Down-Back-Left etc..)
    /// It will automatically resize if given a coordinate outside of its current supported bounds
    /// It uses recursion but its guaranteed to never stack overflow because the max values of integers will only produce 21 levels of recursion.
    /// Even though it supports a very large space it is most efficient with data that is clustered together as coordinates far from each other will result in deeper recursion and branches with memory usage then becoming the main concern.
    /// Also it is called Ninetree because it sound better than twenty seven tree
    /// </summary>
    public class Ninetree<T>
    {
        public class Container
        {
            public T Value;
        }
        public class Node
        {
            public Container Container = null;
            public Node[,,] Children = null;
        }
        public Node Root = new Node();
        private int DimDepth;

        public Ninetree()
        {
            DimDepth = 0;
        }

        public long GetSize()
        {
            //To save on one level
            return IntPow(3, DimDepth+1);
        }

        private void CheckSizeResize(int _X, int _Y, int _Z)
        {
            bool ValidBounds = false;

            do
            {
                long Bounds = GetSize() / 2;

                if (_X > Bounds || _X < -Bounds || _Y > Bounds || _Y < -Bounds || _Z > Bounds || _Z < -Bounds)
                {
                    //Resize by creating a new node, making it the new parent and setting the old parent as its center child.
                    Node newParent = new Node();
                    newParent.Children = new Node[3, 3, 3];
                    newParent.Children[1, 1, 1] = Root;
                    Root = newParent;
                    DimDepth++;
                }
                else
                {
                    ValidBounds = true;
                }
            }
            while (!ValidBounds);

        }

        private bool CheckSize(int _X, int _Y, int _Z)
        {
            long Bounds = GetSize() / 2;

            if (_X > Bounds || _X < -Bounds || _Y > Bounds || _Y < -Bounds || _Z > Bounds || _Z < -Bounds)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Add(T _Value, int _X, int _Y, int _Z)
        {
            CheckSizeResize(_X, _Y, _Z);
            InternalAdd(_Value, _X, _Y, _Z, Root, DimDepth);
        }

        public T Get(int _X, int _Y, int _Z)
        {
            if (CheckSize(_X, _Y, _Z))
            {
                return InternalGet(_X, _Y, _Z, Root, DimDepth);
            }else
            {
                return default(T);
            }
        }

        private void InternalAdd(T _Value, int _X, int _Y, int _Z, Node _Node, int _Depth = 1)
        {
            if (_Depth > 0)
            {
                if (_Node.Children == null)
                {
                    _Node.Children = new Node[3, 3, 3];
                }

                long Block = IntPow(3, _Depth);
                long Dim = (Block - 1) / 2;

                long XDiv;
                long YDiv;
                long ZDiv;

                if (Dim > 1)
                {
                    XDiv = _X == 0 ? 0 : _X > 0 ? (_X - 1) / Dim : (_X + 1) / Dim;
                    YDiv = _Y == 0 ? 0 : _Y > 0 ? (_Y - 1) / Dim : (_Y + 1) / Dim;
                    ZDiv = _Z == 0 ? 0 : _Z > 0 ? (_Z - 1) / Dim : (_Z + 1) / Dim;
                }
                else
                {
                    XDiv = _X < 0 ? _X + 1 : _X > 0 ? _X - 1 : 0;
                    YDiv = _Y < 0 ? _Y + 1 : _Y > 0 ? _Y - 1 : 0;
                    ZDiv = _Z < 0 ? _Z + 1 : _Z > 0 ? _Z - 1 : 0;
                }

                long DirX = XDiv > 0 ? 2 : XDiv < 0 ? 0 : 1;
                long DirY = YDiv > 0 ? 2 : YDiv < 0 ? 0 : 1;
                long DirZ = ZDiv > 0 ? 2 : ZDiv < 0 ? 0 : 1;

                if (_Node.Children[DirX, DirY, DirZ] == null)
                {
                    _Node.Children[DirX, DirY, DirZ] = new Node();
                }

                InternalAdd(_Value,
                    DirX > 1 ? _X - (int)Block : DirX < 1 ? _X + (int)Block : _X,
                    DirY > 1 ? _Y - (int)Block : DirY < 1 ? _Y + (int)Block : _Y,
                    DirZ > 1 ? _Z - (int)Block : DirZ < 1 ? _Z + (int)Block : _Z,
                    _Node.Children[DirX, DirY, DirZ],
                    _Depth - 1);
            }
            else
            {
                if (_Node.Children == null)
                {
                    _Node.Children = new Node[3, 3, 3];
                }

                int CX = _X + 1;
                int CY = _Y + 1;
                int CZ = _Z + 1;

                if (_Node.Children[CX, CY, CZ] == null)
                {
                    _Node.Children[CX, CY, CZ] = new Node();
                }
                _Node.Children[CX, CY, CZ].Container = new Container()
                {
                    Value = _Value
                };
            }
        }

        private T InternalGet(int _X, int _Y, int _Z, Node _Node, int _Depth = 3)
        {
            if (_Depth > 0)
            {
                if (_Node.Children == null)
                {
                    return default(T);
                }

                long Block = IntPow(3, _Depth);
                long Dim = (Block - 1) / 2;

                long XDiv;
                long YDiv;
                long ZDiv;

                if (Dim > 1)
                {
                    XDiv = _X == 0 ? 0 : _X > 0 ? (_X - 1) / Dim : (_X + 1) / Dim;
                    YDiv = _Y == 0 ? 0 : _Y > 0 ? (_Y - 1) / Dim : (_Y + 1) / Dim;
                    ZDiv = _Z == 0 ? 0 : _Z > 0 ? (_Z - 1) / Dim : (_Z + 1) / Dim;
                }
                else
                {
                    XDiv = _X < 0 ? _X + 1 : _X > 0 ? _X - 1 : 0;
                    YDiv = _Y < 0 ? _Y + 1 : _Y > 0 ? _Y - 1 : 0;
                    ZDiv = _Z < 0 ? _Z + 1 : _Z > 0 ? _Z - 1 : 0;
                }

                long DirX = XDiv > 0 ? 2 : XDiv < 0 ? 0 : 1;
                long DirY = YDiv > 0 ? 2 : YDiv < 0 ? 0 : 1;
                long DirZ = ZDiv > 0 ? 2 : ZDiv < 0 ? 0 : 1;

                if (_Node.Children[DirX, DirY, DirZ] == null)
                {
                    return default(T);
                }

                return InternalGet(
                    DirX > 1 ? _X - (int)Block : DirX < 1 ? _X + (int)Block : _X,
                    DirY > 1 ? _Y - (int)Block : DirY < 1 ? _Y + (int)Block : _Y,
                    DirZ > 1 ? _Z - (int)Block : DirZ < 1 ? _Z + (int)Block : _Z,
                    _Node.Children[DirX, DirY, DirZ],
                    _Depth - 1);
            }
            else
            {
                if (_Node.Children == null)
                {
                    return default(T);
                }

                int CX = _X + 1;
                int CY = _Y + 1;
                int CZ = _Z + 1;

                if (_Node.Children[CX, CY, CZ] == null)
                {
                    return default(T);
                }

                if (_Node.Children[CX, CY, CZ].Container == null)
                {
                    return default(T);
                }
                else
                {
                    return _Node.Children[CX, CY, CZ].Container.Value;
                }
            }
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
    }
}

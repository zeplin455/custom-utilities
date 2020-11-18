using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZUtils.Utilities
{
    /// <summary>
    /// Simple helper to make casts look prettier
    /// </summary>
    public static class TypeHelper
    {
        public static T As<T>(this object obj)
        {
            return (T)obj;
        }
    }
}

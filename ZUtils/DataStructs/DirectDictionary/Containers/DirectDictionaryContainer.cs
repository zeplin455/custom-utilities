using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZUtils.DataStructs.DirectDictionary.Containers
{
    /// <summary>
    /// You can derive from this class to make your own containers types
    /// </summary>
    /// <typeparam name="V">Type of value this container will hold</typeparam>
    public abstract class DirectDictionaryContainer<V>
    {
        public DirectDictionaryContainer(V val)
        {
            Value = val;
        }

        public DirectDictionaryContainer()
        {
            Value = default(V);
        }

        public abstract V Value { get; set; }
    }
}

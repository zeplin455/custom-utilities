using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZUtils.DataStructs.DirectDictionary.Containers
{
    /// <summary>
    /// Simplest implementation of a container holding a value
    /// </summary>
    /// <typeparam name="V">Type of value to hold</typeparam>
    public class SimpleContainer<V> : DirectDictionaryContainer<V>
    {
        private V _value;

        public SimpleContainer(V val) : base(val)
        {

        }

        public SimpleContainer():base()
        {

        }

        public override V Value { get => _value; set => _value = value; }
    }
}

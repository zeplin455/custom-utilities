using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZUtils.DataStructs.DirectDictionary.Containers
{
    /// <summary>
    /// Simple example of an expirable container that will get rid of its value when it has expired.
    /// </summary>
    /// <typeparam name="V">Type of value to hold</typeparam>
    public class ExpirableContainer<V> : DirectDictionaryContainer<V>
    {
        public ExpirableContainer(V val, DateTime expiresWhen) :base(val)
        {
            _expiryDateTime = expiresWhen;
        }

        public ExpirableContainer(DateTime expiresWhen) :base()
        {
            _expiryDateTime = expiresWhen;
        }

        private DateTime _expiryDateTime;
        private V _value;

        public bool HasExpired()
        {
            return DateTime.Now >= _expiryDateTime;
        }

        public override V Value 
        {
            get 
            { 
                if(HasExpired())
                {
                    _value = default(V);
                    return _value;
                }else
                {
                    return _value;
                }
            }
            set
            {
                _value = value;
            }
        }
    }
}

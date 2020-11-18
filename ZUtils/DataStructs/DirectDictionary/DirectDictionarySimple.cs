using ZUtils.DataStructs.DirectDictionary.Containers;

namespace ZUtils.DataStructs.DirectDictionary
{
    /// <summary>
    /// An version that simplifies the class signature by using SimpleContainer as a default container
    /// </summary>
    /// <typeparam name="T">Type of data you want to store</typeparam>
    public class DirectDictionaryLayerSimple<T> :DirectDictionaryLayer<T, SimpleContainer<T>>
    {
    }
}

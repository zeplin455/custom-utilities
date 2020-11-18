using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ZUtils.DataStructs.DirectDictionary.Containers;
using ZUtils.Utilities;

namespace ZUtils.DataStructs.DirectDictionary
{
    /// <summary>
    /// A Recursive binary tree like structure with every node having 16 branches that makes for a nice dictionary where the keys build a direct path to the data.
    /// Keys are bytes that gets split into nibbles and then used as indexes on each level of the tree.
    /// The longer and more different keys are from each other the more memory it will use.
    /// Its slower than a normal .net dictionary and uses quite a bit more memory. The only interesting quality is its ability to loop on itself to form interesting keys and structures. 
    /// It is thread safe so you can read and write from multiple threads at the same time. It will lock per accessed layer when writing to it.
    /// </summary>
    /// <typeparam name="T">Type of item you want to store</typeparam>
    /// <typeparam name="T">Type of container you want to use</typeparam>
    public class DirectDictionaryLayer<T, Container> : IEnumerable<T>
        where Container : DirectDictionaryContainer<T>, new()
    {
        #region Constructors
        public DirectDictionaryLayer()
        {
            //16 is a good balance between performance and memory cost.
            Items = new DirectDictionaryLayer<T, Container>[16];
        }
        #endregion

        #region Indexers
        /// <summary>
        /// Indexer for byte keys
        /// </summary>
        /// <param name="key">your key in byte array form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[byte[] key]
        {
            get
            {
                byte[] byteKey = key.SplitNibbles();
                return GetLayer(byteKey);
            }
            set
            {
                byte[] byteKey = key.SplitNibbles();
                SetLayer(byteKey, value, true);
            }
        }

        /// <summary>
        /// Indexer for single byte as part of a key when you want to partition keys
        /// </summary>
        /// <param name="index">byte part of a key</param>
        /// <returns>layer under this key part</returns>
        public DirectDictionaryLayer<T, Container> this[byte index]
        {
            get
            {
                byte[] byteKey = index.SplitNibbles();
                return GetLayer(byteKey);
            }
            set
            {
                byte[] byteKey = index.SplitNibbles();
                SetLayer(byteKey, value, true);
            }
        }

        /// <summary>
        /// Indexer for a string key
        /// </summary>
        /// <param name="key">Your key in string form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[string key]
        {
            get
            {
                byte[] byteKey = KeyConvert.ConvertToNibbles(key);
                return GetLayer(byteKey);
            }
            set
            {
                byte[] byteKey = KeyConvert.ConvertToNibbles(key);
                SetLayer(byteKey, value, true);
            }
        }

        /// <summary>
        /// Indexer for a char key
        /// </summary>
        /// <param name="key">Your key in char form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[char key]
        {
            get
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                return GetLayer(bKey);
            }
            set
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                SetLayer(bKey, value, true);
            }
        }

        /// <summary>
        /// Indexer for a int key
        /// </summary>
        /// <param name="key">Your key in int form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[int key]
        {
            get
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                return GetLayer(bKey);
            }
            set
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                SetLayer(bKey, value, true);
            }
        }
        /// <summary>
        /// Indexer for a uint key
        /// </summary>
        /// <param name="key">Your key in uint form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[uint key]
        {
            get
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                return GetLayer(bKey);
            }
            set
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                SetLayer(bKey, value, true);
            }
        }

        /// <summary>
        /// Indexer for a long key
        /// </summary>
        /// <param name="key">Your key in long form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[long key]
        {
            get
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                return GetLayer(bKey);
            }
            set
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                SetLayer(bKey, value, true);
            }
        }

        /// <summary>
        /// Indexer for a ulong key
        /// </summary>
        /// <param name="key">Your key in ulong form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[ulong key]
        {
            get
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                return GetLayer(bKey);
            }
            set
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                SetLayer(bKey, value, true);
            }
        }

        /// <summary>
        /// Indexer for a short key
        /// </summary>
        /// <param name="key">Your key in short form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[short key]
        {
            get
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                return GetLayer(bKey);
            }
            set
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                SetLayer(bKey, value, true);
            }
        }

        /// <summary>
        /// Indexer for a ushort key
        /// </summary>
        /// <param name="key">Your key in ushort form</param>
        /// <returns>layer stored at this location</returns>
        public DirectDictionaryLayer<T, Container> this[ushort key]
        {
            get
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                return GetLayer(bKey);
            }
            set
            {
                byte[] bKey = BitConverter.GetBytes(key).SplitNibbles();
                SetLayer(bKey, value, true);
            }
        }

        #endregion

        #region Properties and variables

        /// <summary>
        /// Layers under this layer
        /// </summary>
        public readonly DirectDictionaryLayer<T, Container>[] Items;

        /// <summary>
        /// Value container at this layer
        /// I'm using a container so that it is decernable whether a location stores a null value or the location does not exist as well as the ability to have special containers such as an expirable container when doing time sensitive caching 
        /// </summary>
        private Container _value = null;

        /// <summary>
        /// Accesor for value stored in layer container
        /// </summary>
        public T Value
        {
            get
            {
                if (_value != null)
                {
                    return _value.Value;
                }
                else
                {
                    return default(T);
                }
            }
            set => _value = new Container() { Value = value };
        }

        /// <summary>
        /// private accessor for the container at this layer
        /// </summary>
        private Container ValueContainer
        {
            get
            {
                if (_value != null)
                {
                    return _value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _value = value;
            }
        }

        #endregion

        #region Public Structure Operations
        /// <summary>
        /// Chect if a location exists
        /// </summary>
        /// <param name="key">location key (Must be nibbles)</param>
        /// <returns>Exists?</returns>
        public bool Contains(byte[] key)
        {
            DirectDictionaryLayer<T, Container> current = this;

            for (int i = 0; i < key.Length; ++i)
            {
                if (current.Items[key[i]] != null)
                {
                    current = current.Items[key[i]];
                }
                else
                {
                    return false;
                }
            }

            if (current.ValueContainer != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Contains<K>(K key) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            return Contains(bytes);
        }

        public bool Contains(string key)
        {
            byte[] bytes = key.ConvertToNibbles();
            return Contains(bytes);
        }



        /// <summary>
        /// Get value stored at location
        /// </summary>
        /// <param name="key">location key (Must be nibbles)</param>
        /// <returns>Value stored at location</returns>
        public T Get(byte[] key)
        {
            DirectDictionaryLayer<T, Container> current = this;

            //Any recursion can be done in a while loop and while loops can become for loops when you know how deep you need to go.
            for (int i = 0; i < key.Length; ++i)
            {
                if (current.Items[key[i]] != null)
                {
                    current = current.Items[key[i]];
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }

            if (current.ValueContainer != null)
            {
                return current.ValueContainer.Value;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public T Get<K>(K key) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            return Get(bytes);
        }

        public T Get(string key)
        {
            byte[] bytes = key.ConvertToNibbles();
            return Get(bytes);
        }

        /// <summary>
        /// Set a specified location with a value
        /// </summary>
        /// <param name="key">location key (Must be nibbles)</param>
        /// <param name="data">item to store at this location</param>
        public void Set(byte[] key, T data)
        {
            DirectDictionaryLayer<T, Container> current = this;
            if (key != null)
            {
                for (int i = 0; i < key.Length; ++i)
                {
                    if (current.Items[key[i]] != null)
                    {
                        current = current.Items[key[i]];
                    }
                    else
                    {
                        lock (current)
                        {
                            //Check again to make sure you are not overwriting another thread's work
                            if (current.Items[key[i]] == null)
                            {
                                current.Items[key[i]] = new DirectDictionaryLayer<T, Container>();
                            }
                        }
                        current = current.Items[key[i]];
                    }
                }
            }

            current.ValueContainer = new Container() { Value = data };
        }

        public void Set<K>(K key, T data) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            Set(bytes, data);
        }

        public void Set(string key, T data)
        {
            byte[] bytes = key.ConvertToNibbles();
            Set(bytes, data);
        }

        /// <summary>
        /// Get the layer object at the specified location
        /// </summary>
        /// <param name="key">location key (Must be nibbles)</param>
        /// <returns>Layer at location</returns>
        public DirectDictionaryLayer<T, Container> GetLayer(byte[] key)
        {
            DirectDictionaryLayer<T, Container> current = this;

            for (int i = 0; i < key.Length; ++i)
            {
                if (current.Items[key[i]] != null)
                {
                    current = current.Items[key[i]];
                }
                else
                {
                    lock (current)
                    {
                        //Check again to make sure you are not overwriting another thread's work
                        if (current.Items[key[i]] == null)
                        {
                            current.Items[key[i]] = new DirectDictionaryLayer<T, Container>();
                        }
                    }
                    current = current.Items[key[i]];
                }
            }

            return current;
        }

        public DirectDictionaryLayer<T,Container> GetLayer<K>(K key) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            return GetLayer(bytes);
        }
        public DirectDictionaryLayer<T, Container> GetLayer(string key)
        {
            byte[] bytes = key.ConvertToNibbles();
            return GetLayer(bytes);
        }

        /// <summary>
        /// Set the layer object a the specified location.
        /// And yes this data structure can loop back on itself if you set layers to refer back to layers higher up in the tree.
        /// </summary>
        /// <param name="key">location to set layer (Must be nibbles)</param>
        /// <param name="layer">layer object to set at location</param>
        /// <param name="swap">Whether to take move existing layer's children over to the newly set one.</param>
        public void SetLayer(byte[] key, DirectDictionaryLayer<T, Container> layer, bool swap = false)
        {
            DirectDictionaryLayer<T, Container> current = this;

            for (int i = 0; i < key.Length - 1; ++i)
            {
                if (current.Items[key[i]] != null)
                {
                    current = current.Items[key[i]];
                }
                else
                {
                    lock (current)
                    {
                        //Check again to make sure you are not overwriting another thread's work
                        if (current.Items[key[i]] == null)
                        {
                            current.Items[key[i]] = new DirectDictionaryLayer<T, Container>();
                        }
                    }
                    current = current.Items[key[i]];
                }
            }
            if (key.Length > 0)
            {
                lock (current)
                {
                    byte last = key[key.Length - 1];
                    if (swap && current.Items[last] != null)
                    {
                        DirectDictionaryLayer<T, Container> swapTarget = current.Items[last];
                        current.Items[last] = layer;
                        Array.Copy(swapTarget.Items, layer.Items, 16);
                    }
                    else
                    {
                        current.Items[last] = layer;
                    }

                }
            }
        }

        public void SetLayer<K>(K key, DirectDictionaryLayer<T, Container> layer, bool swap = false) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            SetLayer(bytes, layer, swap);
        }

        public void SetLayer(string key, DirectDictionaryLayer<T, Container> layer, bool swap = false)
        {
            byte[] bytes = key.ConvertToNibbles();
            SetLayer(bytes, layer, swap);
        }


        /// <summary>
        /// Check if something exists at a specified location and return it.
        /// </summary>
        /// <param name="key">location key (Must be nibbles)</param>
        /// <param name="value">value if found at specified location</param>
        /// <returns>bool specifying whether something was found or not</returns>
        public bool IfContainsGet(byte[] key, out T value)
        {
            DirectDictionaryLayer<T, Container> current = this;

            for (int i = 0; i < key.Length; ++i)
            {
                if (current.Items[key[i]] != null)
                {
                    current = current.Items[key[i]];
                }
                else
                {
                    value = default(T);
                    return false;
                }
            }

            if (current.ValueContainer != null)
            {
                value = current.ValueContainer.Value;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public bool IfContainsGet<K>(K key, out T value) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            return IfContainsGet(bytes, out value);
        }

        public bool IfContainsGet<K>(string key, out T value)
        {
            byte[] bytes = key.ConvertToNibbles();
            return IfContainsGet(bytes, out value);
        }

        /// <summary>
        /// Get a value at a specified location or set it if it's empty
        /// </summary>
        /// <param name="key">location key (Must be nibbles)</param>
        /// <param name="getNewFunc">function used to set location if it's empty</param>
        /// <returns>Value that was get or set</returns>
        public T GetOrSet(byte[] key, Func<T> getNewFunc)
        {
            DirectDictionaryLayer<T, Container> current = this;
            bool found = true;

            for (int i = 0; i < key.Length; ++i)
            {
                if (current.Items[key[i]] != null)
                {
                    current = current.Items[key[i]];
                }
                else
                {
                    found = false;
                    lock (current)
                    {
                        if (current.Items[key[i]] == null)
                        {
                            current.Items[key[i]] = new DirectDictionaryLayer<T, Container>();
                        }
                    }
                    current = current.Items[key[i]];
                }
            }
            if (!found)
            {
                lock (current)
                {
                    if (current.ValueContainer == null)
                    {
                        current.ValueContainer = new Container() { Value = getNewFunc() };
                    }
                    else
                    {
                        return current.ValueContainer.Value;
                    }
                }
            }
            if (current.ValueContainer == null)
            {
                lock (current)
                {
                    current.ValueContainer = new Container() { Value = getNewFunc() };
                }
            }

            return current.ValueContainer.Value;
        }

        public T GetOrSet<K>(K key, Func<T> getNewFunc) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            return GetOrSet(bytes, getNewFunc);
        }

        public T GetOrSet(string key, Func<T> getNewFunc)
        {
            byte[] bytes = key.ConvertToNibbles();
            return GetOrSet(bytes, getNewFunc);
        }

        /// <summary>
        /// Delete a value stored at specified location without cleaning up the structure. (Faster but leaves memory in use)
        /// </summary>
        /// <param name="key">location key (Must be nibbles)</param>
        public void DeleteWithoutShrink(byte[] key)
        {
            int level = 0;

            DirectDictionaryLayer<T, Container> current = this;
            while (level < key.Length)
            {
                if (current.Items[key[level]] == null)
                {
                    return;
                }
                else
                {
                    current = current.Items[key[level]];
                }
                ++level;
            }
            lock (current)
            {
                current.ValueContainer = null;
            }
        }

        public void DeleteWithoutShrink<K>(K key) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            DeleteWithoutShrink(bytes);
        }

        public void DeleteWithoutShrink(string key)
        {
            byte[] bytes = key.ConvertToNibbles();
            DeleteWithoutShrink(bytes);
        }

        /// <summary>
        /// Merges two structures together. Passed layer will overwrite overlapping locations existing in both.
        /// </summary>
        /// <param name="mergeLayer">layer to merge into this one</param>
        public void Merge(DirectDictionaryLayer<T, Container> mergeLayer)
        {
            mergeLayer.ExecuteOnAllItems((o, k) =>
            {
                Set(k, o);
            });
        }

        /// <summary>
        /// Filter out layers under a specified partial key
        /// </summary>
        /// <param name="beginsWith">Part of a key you want to filter on</param>
        /// <returns>Layers under partial key</returns>
        public DirectDictionaryLayer<T, Container> Filter(byte[] beginsWith)
        {
            return GetKeyFragmentRoot(beginsWith);
        }

        public DirectDictionaryLayer<T,Container> Filter<K>(K key) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            return Filter(bytes);
        }
        public DirectDictionaryLayer<T, Container> Filter(string key)
        {
            byte[] bytes = key.ConvertToNibbles();
            return Filter(bytes);
        }

        /// <summary>
        /// Delete a value stored at a specified location
        /// </summary>
        /// <param name="key">location key (Must be nibbles)</param>
        public void Delete(byte[] key)
        {
            DirectDictionaryLayer<T, Container> current = this;
            bool found = true;

            for (int i = 0; i < key.Length; ++i)
            {
                if (current.Items[key[i]] != null)
                {
                    current = current.Items[key[i]];
                }
                else
                {
                    found = false;
                    break;
                }
            }

            if (found)
            {
                lock (current)
                {
                    //don't check again because deleting is deleting
                    current.ValueContainer = null;
                }
            }

            //reclaim memory if possible
            for (int i = key.Length - 1; i >= 0; --i)
            {
                byte[] newKey = new byte[i + 1];
                Array.Copy(key, newKey, i + 1);

                DirectDictionaryLayer<T, Container> layerCheck = GetLayer(key);

                if (ShouldDelete(layerCheck))
                {
                    if (newKey.Length > 1)
                    {
                        byte[] deleteKey = new byte[newKey.Length - 1];
                        Array.Copy(newKey, deleteKey, deleteKey.Length);
                        DirectDictionaryLayer<T, Container> deleteFromLayer = GetLayer(deleteKey);
                        deleteFromLayer.Items[newKey[newKey.Length - 1]] = null;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public void Delete<K>(K key)where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            Delete(bytes);
        }
        public void Delete(string key)
        {
            byte[] bytes = key.ConvertToNibbles();
            Delete(bytes);
        }


        /// <summary>
        /// Custom method to execute on all values.
        /// </summary>
        /// <param name="method">Method to execute on all values.</param>
        public void ExecuteOnAllItems(Action<T> method)
        {
            List<int> IndexTracker = new List<int>();
            List<DirectDictionaryLayer<T, Container>> RefTracker = new List<DirectDictionaryLayer<T, Container>>();
            int currentDepth = 0;

            IndexTracker.Add(0);
            RefTracker.Add(this);

            while (currentDepth >= 0)
            {
                if (IndexTracker[currentDepth] < 16 && RefTracker[currentDepth].Items[IndexTracker[currentDepth]] != null)
                {
                    IndexTracker.Add(0);
                    RefTracker.Add(RefTracker[currentDepth].Items[IndexTracker[currentDepth]]);
                    ++currentDepth;
                }
                else
                {
                    ++IndexTracker[currentDepth];
                }

                if (IndexTracker[currentDepth] >= 16)
                {
                    if (RefTracker[currentDepth].ValueContainer != null)
                    {
                        method(RefTracker[currentDepth].ValueContainer.Value);
                    }

                    IndexTracker.RemoveAt(currentDepth);
                    RefTracker.RemoveAt(currentDepth);

                    --currentDepth;
                    if (currentDepth >= 0)
                    {
                        ++IndexTracker[currentDepth];
                    }
                }
            }
        }

        /// <summary>
        /// Custom method to execute on all values in the structure.
        /// </summary>
        /// <param name="method">Method to execute on each value. Key and value will be passed to the method.</param>
        public void ExecuteOnAllItems(Action<T, byte[]> method)
        {
            List<int> IndexTracker = new List<int>();
            List<int> KeyTracker = new List<int>();
            List<DirectDictionaryLayer<T, Container>> RefTracker = new List<DirectDictionaryLayer<T, Container>>();
            int currentDepth = 0;

            IndexTracker.Add(0);
            RefTracker.Add(this);

            while (currentDepth >= 0)
            {
                if (IndexTracker[currentDepth] < 16 && RefTracker[currentDepth].Items[IndexTracker[currentDepth]] != null)
                {
                    IndexTracker.Add(0);
                    RefTracker.Add(RefTracker[currentDepth].Items[IndexTracker[currentDepth]]);
                    KeyTracker.Add(IndexTracker[currentDepth]);
                    ++currentDepth;
                }
                else
                {
                    ++IndexTracker[currentDepth];
                }

                if (IndexTracker[currentDepth] >= 16)
                {
                    if (RefTracker[currentDepth].ValueContainer != null)
                    {
                        method(RefTracker[currentDepth].ValueContainer.Value, KeyTracker.Select(x => (byte)x).ToArray());
                    }
                    IndexTracker.RemoveAt(currentDepth);
                    RefTracker.RemoveAt(currentDepth);
                    --currentDepth;

                    if (currentDepth >= 0)
                    {
                        KeyTracker.RemoveAt(currentDepth);
                        ++IndexTracker[currentDepth];
                    }
                }
            }
        }

        /// <summary>
        /// Execute a custom method on each layer object in the structure.
        /// </summary>
        /// <param name="method">Method to execute. Current key and layer object will be passed to method</param>
        public void ExecuteOnAllLayers(Action<DirectDictionaryLayer<T, Container>, byte[]> method)
        {
            List<int> IndexTracker = new List<int>();
            List<int> KeyTracker = new List<int>();
            List<DirectDictionaryLayer<T, Container>> RefTracker = new List<DirectDictionaryLayer<T, Container>>();
            int currentDepth = 0;

            IndexTracker.Add(0);
            RefTracker.Add(this);

            while (currentDepth >= 0)
            {
                if (IndexTracker[currentDepth] < 16 && RefTracker[currentDepth].Items[IndexTracker[currentDepth]] != null)
                {
                    IndexTracker.Add(0);
                    RefTracker.Add(RefTracker[currentDepth].Items[IndexTracker[currentDepth]]);
                    KeyTracker.Add(IndexTracker[currentDepth]);
                    ++currentDepth;
                }
                else
                {
                    ++IndexTracker[currentDepth];
                }

                if (IndexTracker[currentDepth] >= 16)
                {
                    method(RefTracker[currentDepth], KeyTracker.Select(x => (byte)x).ToArray());
                    IndexTracker.RemoveAt(currentDepth);
                    RefTracker.RemoveAt(currentDepth);
                    --currentDepth;

                    if (currentDepth >= 0)
                    {
                        KeyTracker.RemoveAt(currentDepth);
                        ++IndexTracker[currentDepth];
                    }
                }
            }
        }

        /// <summary>
        /// Delete structure branches that don't contain any values.
        /// This will go through the entire structure and can be an expensive task.
        /// </summary>
        public void Shrink()
        {
            ExecuteOnAllLayers((o, k) =>
            {
                if (o.ValueContainer == null)
                {
                    if (ShouldDelete(o))
                    {
                        SetLayer(k, null);
                    }
                }
            });
        }

        public IEnumerable<DirectDictionaryLayer<T, Container>> GetLayersContaining(byte[] searchPattern)
        {
            List<DirectDictionaryLayer<T, Container>> result = new List<DirectDictionaryLayer<T, Container>>();
            ExecuteOnAllLayers((o, k) =>
            {
                if (Search(k, searchPattern) != -1)
                {
                    result.Add(o);
                }
            });
            return result;
        }

        public IEnumerable<DirectDictionaryLayer<T, Container>> GetLayersContaining<K>(K key) where K : struct
        {
            byte[] bytes = GetBytes<K>(key).SplitNibbles();
            return GetLayersContaining(bytes);
        }
        public IEnumerable<DirectDictionaryLayer<T, Container>> GetLayersContaining(string key)
        {
            byte[] bytes = key.ConvertToNibbles();
            return GetLayersContaining(bytes);
        }

        #endregion

        #region Private Methods
        private bool ShouldDelete(DirectDictionaryLayer<T, Container> layer)
        {
            if (layer.ValueContainer != null)
            {
                return false;
            }

            bool shouldDelete = true;

            for (int i = 0; i < layer.Items.Length; ++i)
            {
                if (layer.Items[i] != null)
                {
                    shouldDelete = false;
                    break;
                }
            }

            return shouldDelete;
        }



        private bool CompareByteArrays(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length == arr2.Length)
            {
                for (int i = 0; i < arr1.Length; ++i)
                {
                    if (arr1[i] != arr2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private byte[] GetBytes<I>(I input) where I : struct
        {
            int size = Marshal.SizeOf(typeof(I));
            var result = new byte[size];
            var gcHandle = GCHandle.Alloc(input, GCHandleType.Pinned);
            Marshal.Copy(gcHandle.AddrOfPinnedObject(), result, 0, size);
            gcHandle.Free();
            return result;
        }

        private int Search(byte[] src, byte[] pattern)
        {
            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = 0; i < c; i++)
            {
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return i;
            }
            return -1;
        }

        private DirectDictionaryLayer<T, Container> GetKeyFragmentRoot(byte[] keyFragment)
        {
            int level = 0;

            DirectDictionaryLayer<T, Container> current = this;
            while (level < keyFragment.Length)
            {
                if (current.Items[keyFragment[level]] == null)
                {
                    return null;
                }
                else
                {
                    current = current.Items[keyFragment[level]];
                }
                ++level;
            }
            return current;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Operator to get value directly from layer
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator T(DirectDictionaryLayer<T, Container> d)
        {
            return d.Value;
        }

        /// <summary>
        /// Operator to set contained value directly on layer
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator DirectDictionaryLayer<T, Container>(T d)
        {
            DirectDictionaryLayer<T, Container> s = new DirectDictionaryLayer<T, Container>();
            s.Value = d;
            return s;
        }
        #endregion

        #region Enumerator
        public IEnumerator<T> GetEnumerator()
        {
            List<int> IndexTracker = new List<int>();
            List<DirectDictionaryLayer<T, Container>> RefTracker = new List<DirectDictionaryLayer<T, Container>>();
            int currentDepth = 0;

            IndexTracker.Add(0);
            RefTracker.Add(this);

            while (currentDepth >= 0)
            {
                if (IndexTracker[currentDepth] < 16 && RefTracker[currentDepth].Items[IndexTracker[currentDepth]] != null)
                {
                    IndexTracker.Add(0);
                    RefTracker.Add(RefTracker[currentDepth].Items[IndexTracker[currentDepth]]);
                    ++currentDepth;
                }
                else
                {
                    ++IndexTracker[currentDepth];
                }

                if (IndexTracker[currentDepth] >= 16)
                {
                    if (RefTracker[currentDepth].ValueContainer != null)
                    {
                        yield return RefTracker[currentDepth].ValueContainer.Value;
                    }
                    IndexTracker.RemoveAt(currentDepth);
                    RefTracker.RemoveAt(currentDepth);
                    --currentDepth;
                    if (currentDepth >= 0)
                    {
                        ++IndexTracker[currentDepth];
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

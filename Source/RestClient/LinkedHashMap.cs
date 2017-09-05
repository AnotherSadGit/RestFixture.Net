using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NLog.Time;

namespace RestClient
{
    /// <summary>
    /// A .NET implementation of a Java LinkedHashMap.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the LinkedHashMap.</typeparam>
    /// <typeparam name="TValue">The type of the values in the LinkedHashMap.</typeparam>
    /// <remarks>Loosely based on the implementation in an answer to Stackoverflow question 
    /// "C# equivalent of LinkedHashMap" by user N0thing, 
    /// http://stackoverflow.com/questions/29205934/c-sharp-equivalent-of-linkedhashmap
    /// 
    /// A LinkedHashMap in Java is a combination of a HashMap (ie Dictionary) and a 
    /// doubly-linked list.  It's like a Dictionary with a predictable iteration order.
    /// 
    /// A Java LinkedHashMap can have one of two iteration orders, depending on which 
    /// constructor is used:
    /// 
    /// 1) Insertion Order (default): The order in which the keys were inserted into the 
    /// map (dictionary).  Updating an element will not affect the iteration order;
    /// 
    /// 2) Access Order: The order in which the keys were last accessed, from least-recently 
    /// accessed to most-recently.  "Access" is defined as get or set (actually "put" rather 
    /// than get in Java).  In addition, Java has a putAll method that accesses all the keys.
    /// No other operation counts as access to a key.
    /// 
    /// This class implements Insertion Order iteration order.</remarks>
    public class LinkedHashMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, KeyValuePair<TKey, TValue>> _dictionary
            = new Dictionary<TKey, KeyValuePair<TKey, TValue>>();

        // Use a List<> rather than a LinkedList<> as it's simpler for the GetEnumerator method 
        //  and Lists are more efficient if elements are only going to be added or removed from 
        //  the end.
        private List<KeyValuePair<TKey, TValue>> _list = new List<KeyValuePair<TKey, TValue>>();

        public TValue this[TKey key]
        {
            get
            {
                if (!_dictionary.ContainsKey(key))
                {
                    return default(TValue);
                }

                KeyValuePair<TKey, TValue> commonNode = _dictionary[key];
                return commonNode.Value;
            }

            set
            {
                KeyValuePair<TKey, TValue> newNode = new KeyValuePair<TKey, TValue>(key, value);
                _dictionary[key] = newNode;

                int existingIndex = _list.FindIndex(kvp => kvp.Key.Equals(key));
                if (existingIndex == -1)
                {
                    _list.Add(newNode);
                    return;
                }
                _list[existingIndex] = newNode;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, TValue> commonNode in _list)
            {
                yield return commonNode;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _dictionary.Clear();
            _list.Clear();
        }

        public IList<TKey> Keys
        {
            // Return list keys rather than dictionary keys to ensure they're in the correct order.
            get { return _list.Select(it => it.Key).ToList();  }
        }

        public void PutAll(LinkedHashMap<TKey, TValue> sourceMap)
        {
            if (sourceMap == null || sourceMap.Count == 0)
            {
                return;
            }

            foreach (TKey key in sourceMap.Keys)
            {
                this[key] = sourceMap[key];
            }
        }

        public Dictionary<TKey, TValue> GetDictionary()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> kvp in this._list)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }

            return dictionary;
        }

        public List<KeyValuePair<TKey, TValue>> GetList()
        {
            return this._list;
        }

        public bool IsEmpty()
        {
            return _list.Count == 0;
        }
    }
}

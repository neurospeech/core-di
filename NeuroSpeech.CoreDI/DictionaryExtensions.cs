using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSpeech.CoreDI
{

    /// <summary>
    /// 
    /// </summary>
    public static class DictionaryExtensions
    {


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="d"></param>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, Func<TKey, TValue> factory)
        {
            TValue v;
            if (d.TryGetValue(key, out v))
                return v;
            v = factory(key);
            d[key] = v;
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="d"></param>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static TValue GetOrAddLocked<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> d, TKey key, Func<TKey, TValue> factory)
        {
            return d.GetOrAdd(key, k => {
                TValue v;
                lock (d)
                {
                    if (!d.TryGetValue(k, out v))
                    {
                        v = factory(k);
                        d[k] = v;
                    }
                }
                return v;
            });
        }

    }

}
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace NeuroSpeech.CoreDI
{

    public class ThreadSafeList<T> : IList<T>
    {
        private List<T> items = new List<T>();

        public T this[int index] { get {
                lock (items) {
                    return items[index];
                }
            } set {
                lock (items) {
                    items[index] = value;
                }
            }
        }

        public int Count {
            get {
                lock (items) {
                    return items.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            lock (items) {
                items.Add(item);
            }
        }

        public void Clear()
        {
            lock (items) {
                items.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (items){
                return items.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (items) {
                items.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (items) {
                return items.ToList<T>().GetEnumerator();
            }
        }

        public int IndexOf(T item)
        {
            lock (items) {
                return items.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (items) {
                items.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            lock (items) {
                return items.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (items) {
                items.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)this).GetEnumerator();
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class DIScope : IDisposable
    {

        /// <summary>
        /// You can set name to identify scope
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }


        private ConcurrentDictionary<Type, object> items = new ConcurrentDictionary<Type, object>();

        private ThreadSafeList<IDisposable> disposables;

        public DIScope Parent { get; private set; }

        private ThreadSafeList<DIScope> children = new ThreadSafeList<DIScope>();

        public DIScope(DIScope parent = null)
        {
            this.Parent = parent;
            parent?.children?.Add(this);
        }

        public object Get(Type type)
        {
            return items.GetOrAddLocked(type, t => DI.New(this, type));
        }

        public object this[Type type] {
            get {
                return Get(type);
            }
            set {
                items[type] = value;
            }
        }

        public void Clear() {
            foreach (var child in children)
            {
                child.Dispose();
            }
            children.Clear();
            items.Clear();
            if (disposables != null)
            {
                foreach (var d in disposables)
                {
                    try
                    {
                        d.Dispose();
                    }
                    catch (Exception ex){
                        System.Diagnostics.Debug.WriteLine("Object Dispose Warning !! " + ex.ToString());
                    }
                }
                disposables.Clear();
                disposables = null;
            }
        }

        public void Dispose()
        {
            Clear();
        }

        internal bool ContainsKey(Type typeImpl)
        {
            return items.ContainsKey(typeImpl);
        }

        internal object RegisterDisposable(object v)
        {
            if (v is IDisposable d) {
                var dl = (disposables ?? (disposables = new ThreadSafeList<IDisposable>()));
                dl.Add(d);
            }
            return v;
        }
    }

}
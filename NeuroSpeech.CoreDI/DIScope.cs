using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSpeech.CoreDI
{

    public class DIScope : IDisposable
    {

        private ConcurrentDictionary<Type, object> items = new ConcurrentDictionary<Type, object>();

        public DIScope Parent { get; private set; }

        private List<DIScope> children = new List<DIScope>();

        public DIScope(DIScope parent = null)
        {
            this.Parent = parent;
            parent?.children?.Add(this);
        }

        public object Get(Type type)
        {
            return items.GetOrAddLocked(type, t => DI.New(this, type));
        }

        public void Dispose()
        {
            foreach (var child in children)
            {
                child.Dispose();
            }
            children.Clear();
            foreach (var value in items.Values)
            {
                var id = value as IDisposable;
                id?.Dispose();
            }
            items.Clear();
        }
    }

}
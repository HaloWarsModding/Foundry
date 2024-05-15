using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    public class RefManager<T>
    {
        public T Item { get; private set; }
        public int RefCount { get; internal set; }

        public RefManager(T item)
        {
            RefCount = 0;
            Item = item;
        }

        public RefWrapper<T> GetRef()
        {
            return new RefWrapper<T>(this, Item);
        }
    }

    public class RefWrapper<T>
    {
        public T Value { get; private set; }
        private RefManager<T> Owner { get; set; }

        internal RefWrapper(RefManager<T> owner, T value)
        {
            Owner = owner;
            Value = value;
            Owner.RefCount++;
        }
        ~RefWrapper()
        {
            Owner.RefCount--;
        }

    }
}

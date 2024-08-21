//using KSoft.Phoenix.Phx;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Chef
//{
//    public class RefWrapper<T> where T : class
//    {
//        public operator= (RefWrapper<T> rhs)
//    }


//    public class RefItem<T> where T : class
//    {
//        public RefItem(T item)
//        {
//            Count = 0;
//            Item = item;
//        }

//        public event EventHandler ItemDestroyed;

//        public T Item { get; private set; }
//        public long Count { get; set; }
//    }
//}

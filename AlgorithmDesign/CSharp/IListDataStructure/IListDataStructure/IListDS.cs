using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IListDataStructure
{
    //DS : DataStructure
    public interface IListDS<T>
    {
        int Count { get; }

        T this[int index] { get; set; }

        void Clear();

        bool IsEmpty();

        //是否为空
        void Append(T item);

        //在位置index前插入元素item
        //就是在指定位置插入元素
        void InsertBefore(T item, int index);

        //在位置index后插入元素item
        void InsertAfter(T item, int index);

        bool RemoveAt(int index);

        bool Remove(T item);

        T GetItemAt(int index);

        int IndexOf(T value);

        bool Contains(T value);

        //反转所有元素
        void Reverse();
    }
}

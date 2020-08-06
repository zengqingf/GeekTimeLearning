using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IListDataStructure;


namespace Array_CSharp_Learning
{
    /// <summary>
    /// 线性表 - 顺序表
    /// 内置List是一个顺序表  Cs SourceCode : https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs
    /// https://www.cnblogs.com/yjmyzz/archive/2010/10/17/1853376.html
    /// </summary>
    public class SequenceList<T> : IListDS<T>
    {
        private const int _defaultCapacity = 4;
        static readonly T[] _emptyArray = new T[0];
        private T[] data;
        //数量
        private int size;
        public int Count
        {
            get { return size; }
        }
        //容量
        public int Capacity
        {
            get { return data.Length; }
            set
            {
                //扩容
                //if (value < size)
                //{
                //    Console.WriteLine(string.Format("[SequenceList] - Capacity set failed, list size {0} > set value {1}", size , value));
                //    return;
                //}
                //if (value != data.Length)
                //{
                //    if (value > 0)
                //    {
                //        T[] newData = new T[value];
                //        if (size > 0)
                //        {
                //            Array.Copy(data, 0, newData, 0, size);
                //        }
                //        data = newData;
                //    }
                //    else
                //    {
                //        data = _emptyArray;
                //    }
                //}

                //_ExpandCapacity_1(value);
                _ExpandCapacity_2(value);
                //_ExpandCapacity_3(value);
            }
        }
                
        private void _ExpandCapacity_1(int newCapacity)
        {
            if (newCapacity < size || newCapacity == data.Length)
            {
                return;
            }
            if (newCapacity > 0)
            {
                T[] newArray = new T[newCapacity];
                if (size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        newArray[i] = data[i];
                    }
                }
                data = newArray;
            }
            else
            {
                data = _emptyArray;
            }
        }

        private void _ExpandCapacity_2(int newCapacity)
        {
            if (newCapacity < size || newCapacity == data.Length)
            {
                return;
            }
            if (newCapacity > 0)
            {
                T[] newArray = new T[newCapacity];
                if (size > 0)
                {
                    Array.Copy(data, 0, newArray, 0, size);
                }
                data = newArray;
            }
            else
            {
                data = _emptyArray;
            }
        }

        private void _ExpandCapacity_3(int newCapacity)
        {
            if (newCapacity < size || newCapacity == data.Length)
            {
                return;
            }
            if (newCapacity > 0)
            {
                Array.Resize<T>(ref data, newCapacity);
            }
            else
            {
                data = _emptyArray;
            }
        }

        private void _EnsureCapacity(int min)
        {
            if (data.Length < min)
            {
                int newCapacity = data.Length == 0 ? _defaultCapacity : data.Length * 2;
                if (newCapacity < min)
                {
                    newCapacity = min;
                }
                Capacity = newCapacity;
            }
        }

        public SequenceList()
        {
            data = _emptyArray;
        }

        public SequenceList(int capacity)
        {
            if (capacity < 0)
            {
                Console.WriteLine("[SequenceList] - create failed, capacity < 0");
                return;
            }
            if (capacity == 0)
            {
                data = _emptyArray;
            }
            else
            {
                data = new T[capacity];
            }
        }

        //表索引器
        public T this[int index]
        {
            get
            {
                return this.GetItemAt(index);
            }
            set
            {
                if (index < 0 || index >= size)
                {
                    throw new IndexOutOfRangeException();
                }
                data[index] = value;
            }
        }

        public void Append(T item)
        {
            if (size == data.Length)
            {
                _EnsureCapacity(size + 1);
            }
            data[size++] = item;
        }

        public void Clear()
        {
            if (size > 0)
            {
                Array.Clear(data, 0, size);
                size = 0;
            }
        }

        public T GetItemAt(int index)
        {
            if (index >= size)
            {
                return default(T);
            }
            return data[index];
        }

        public int IndexOf(T value)
        {
            return Array.IndexOf(data, value, 0, size);
        }

        public void InsertAfter(T item, int index)
        {
            if (index > size)
            {
                Console.WriteLine(string.Format("[SequenceList] - InsertAfter failed, insert index : {0}, size : {1}", index, size));
                return;
            }
            if (size == data.Length)
            {
                _EnsureCapacity(size + 1);
            }
            if (index + 1 < size)
            {
                //Array.Copy(data, index + 1, data, index + 2, size - index - 1);
                for (int j = size - 1; j > index; j--)
                {
                    data[j + 1] = data[j];
                }
            }
            if (index == size)
            {
                data[index] = item;
            }
            else
            {
                data[index + 1] = item;
            }
            size++;
        }

        public void InsertBefore(T item, int index)
        {
            if (index > size)
            {
                Console.WriteLine(string.Format("[SequenceList] - InsertAfter failed, insert index : {0}, size : {1}", index, size));
                return;
            }
            if (size == data.Length)
            {
                _EnsureCapacity(size + 1);
            }
            if (index < size)
            {
                //Array.Copy(data, index, data, index + 1, size - index);
                for (int j = size - 1; j >= index; j--)
                {
                    data[j + 1] = data[j];
                }
            }
            data[index] = item;
            size++;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                return RemoveAt(index);
            }
            return false;
        }

        public bool RemoveAt(int index)
        {
            if (index >= size)
            {
                Console.WriteLine(string.Format("[SequenceList] - RemoveAt failed, remove index : {0}, size : {1}", index, size));
                return false;
            }
            size--;
            if (index < size)
            {
                Array.Copy(data, index + 1, data, index, size - index);
            }
            data[size] = default(T);
            return true;
        }

        public void Reverse()
        {
            //Array.Reverse(data);
            T temp = default(T);
            for (int i = 0; i < size / 2; i++)
            {
                temp = data[i];
                data[i] = data[size - 1 - i];
                data[size - 1 - i] = temp;
            }
        }

        bool IListDS<T>.Contains(T value)
        {
            if ((Object)value == null)
            {
                for (int i = 0; i < size; i++)
                {
                    if ((Object)data[i] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                EqualityComparer<T> eqComparer = EqualityComparer<T>.Default;
                for (int i = 0; i < size; i++)
                {
                    if (eqComparer.Equals(data[i], value))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                sb.Append(data[i].ToString() + ",");
            }
            return sb.ToString().TrimEnd(',');
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IListDataStructure;


namespace LinkedList_CSharp_Learning
{
    /// <summary>
    /// 循环链表(尾部结点指向头结点)
    /// 
    /// 参考 ： https://blog.csdn.net/luxin10/article/details/6113936
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularLinkedList<T> : IListDS<T>
    {
        private Node<T> head;
        public Node<T> Head
        {
            get { return head; }
            set { head = value; }
        }

        public CircularLinkedList()
        {
            this.head = null;
        }


        public T this[int index]
        {
            get
            {
                return this.GetItemAt(index);
            }
            set
            {
                Node<T> tempNode = GetNodeAt(index);
                if (tempNode != null)
                {
                    tempNode.Data = value;
                }
            }
        }



        public int Count => throw new NotImplementedException();

        public void Append(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T value)
        {
            throw new NotImplementedException();
        }

        public T GetItemAt(int index)
        {
            throw new NotImplementedException();
        }

        public Node<T> GetNodeAt(int index)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T value)
        {
            throw new NotImplementedException();
        }

        public void InsertAfter(T item, int index)
        {
            throw new NotImplementedException();
        }

        public void InsertBefore(T item, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Reverse()
        {
            throw new NotImplementedException();
        }
    }
}

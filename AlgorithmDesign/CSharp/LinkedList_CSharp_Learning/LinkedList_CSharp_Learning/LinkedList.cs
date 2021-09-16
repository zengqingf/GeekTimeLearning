using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IListDataStructure;

namespace LinkedList_CSharp_Learning
{
    /// <summary>
    /// 单链表
    /// 
    /// 参考：https://www.cnblogs.com/yjmyzz/archive/2010/10/17/1853562.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedList<T> : IListDS<T>
    {
        private Node<T> head;
        public Node<T> Head
        {
            get { return head; }
            set { head = value; }
        }

        public LinkedList()
        {
            head = null;
        }

        public T this[int index]
        {
            get {
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

        public int Count
        {
            get
            {
                Node<T> p = head;
                int length = 0;
                while (p != null)
                {
                    length++;
                    p = p.Next;
                }
                return length;
            }
        }

        public void Clear()
        {
            //GC
            head = null;
        }

        public bool IsEmpty()
        {
            return head == null;
        }

        public void Append(T item)
        {
            Node<T> newNode = new Node<T>(item);
            if (IsEmpty())
            {
                head = newNode;
                return;
            }
            Node<T> currNode = head;
            while (currNode.Next != null)
            {
                currNode = currNode.Next;
            }
            currNode.Next = newNode;
        }

        public void InsertBefore(T item, int index)
        {
            if (IsEmpty() || index < 0)
            {
                Console.WriteLine("[LinkedList] - InsertBefore failed, index : {0}, linkedList {1} empty", index, IsEmpty() ? "is" : "is not");
                return;
            }

            Node<T> newNode = new Node<T>(item);
            // O(n)
            if (index == 0)
            {
                newNode.Next = head;
                head = newNode;
                return;
            }

            Node<T> currNode = head;
            Node<T> preNode = null;                     //前一个元素
            int j = 0;
            while (currNode.Next != null && j < index)
            {
                preNode = currNode;
                currNode = currNode.Next;
                j++;
            }

            //如果当前指向结点为尾结点，并且指定序号大于链表长度，则表明是append结点
            if (currNode.Next == null && j < index)
            {
                currNode.Next = newNode;
                newNode.Next = null;
            }
            else
            {
                if (j == index)
                {
                    preNode.Next = newNode;
                    newNode.Next = currNode;
                }
            }
        }

        public void InsertAfter(T item, int index)
        {
            if (IsEmpty() || index < 0)
            {
                Console.WriteLine("[LinkedList] - InsertAfter failed, index : {0}, linkedList {1} empty", index, IsEmpty() ? "is" : "is not");
                return;
            }

            Node<T> newNode = new Node<T>(item);

            if (index == 0)
            {
                newNode.Next = head.Next;
                head.Next = newNode;
                return;
            }

            Node<T> currNode = head;
            int j = 0;
            while (currNode.Next != null && j < index)
            {
                currNode = currNode.Next;
                j++;
            }

            //如果当前指向结点为尾结点，并且指定序号大于链表长度，则表明是append结点
            if (currNode.Next == null && j < index)
            {
                currNode.Next = newNode;
                newNode.Next = null;
            }
            else
            {
                if (j == index)
                {
                    newNode.Next = currNode.Next;
                    currNode.Next = newNode;
                }
            }
        }

        public bool RemoveAt(int index)
        {
            if (IsEmpty() || index < 0)
            {
                Console.WriteLine("[LinkedList] - RemoveAt failed, index : {0}, linkedList {1} empty", index, IsEmpty() ? "is" : "is not");
                return false;
            }
            Node<T> currNode = head;
            if (index == 0)
            {
                head = head.Next;
                return true;
            }

            Node<T> preNode = null;
            int j = 0;
            while (currNode.Next != null && j < index)
            {
                preNode = currNode;
                currNode = currNode.Next;
                j++;
            }

            if (j == index)
            {
                preNode.Next = currNode.Next;
                return true;
            }
            else
            {
                Console.WriteLine("[LinkedList] - RemoveAt failed, index : {0} error", index);
                return false;
            }
        }

        public bool Remove(T item)
        {
            if (IsEmpty())
            {
                Console.WriteLine("[LinkedList] - Remove failed, linkedList {1} empty", IsEmpty() ? "is" : "is not");
                return false;
            }
            Node<T> currNode = head;
            Node<T> preNode = null;
            while (currNode.Next != null && !currNode.Data.Equals(item))
            {
                preNode = currNode;
                currNode = currNode.Next;
            }
            if (currNode.Next != null && currNode.Data.Equals(item))
            {
                preNode.Next = currNode.Next;
                return true;
            }
            else
            {
                Console.WriteLine("[LinkedList] - Remove failed, not found {0}", item);
                return false;
            }            
        }

        public T GetItemAt(int index)
        {
            if (IsEmpty())
            {
                Console.WriteLine("[LinkedList] - GetItemAt failed, index : {0}, linkedList {1} empty", index, IsEmpty() ? "is" : "is not");
                return default(T);
            }

            Node<T> currNode = head;

            // O(1)
            if (index == 0)
            {
                return currNode.Data;
            }

            // O(n)
            int j = 0;
            //找到index位置的元素
            while (currNode.Next != null && j < index)
            {
                j++;
                currNode = currNode.Next;
            }
            if (j == index)
            {
                return currNode.Data;
            }

            return default(T);
        }

        public Node<T> GetNodeAt(int index)
        {
            if (IsEmpty())
            {
                Console.WriteLine("[LinkedList] - find this failed, index : {0}, linkedList {1} empty", index, IsEmpty() ? "is" : "is not");
                return null;
            }

            Node<T> currNode = head;

            // O(1)
            if (index == 0)
            {
                return currNode;
            }

            // O(n)
            int j = 0;
            //找到index位置的元素
            while (currNode.Next != null && j < index)
            {
                j++;
                currNode = currNode.Next;
            }
            if (j == index)
            {
                return currNode;
            }
            return null;
        }

        public int IndexOf(T value)
        {
            if (IsEmpty())
            {
                Console.WriteLine("[LinkedList] - IndexOf failed, linkedList {1} empty",IsEmpty() ? "is" : "is not");
                return -1;
            }
            Node<T> currNode = head;
            int j = 0;
            while (currNode.Next != null && !currNode.Data.Equals(value))
            {
                currNode = currNode.Next;
                j++;
            }
            return j;
        }

        public bool Contains(T value)
        {
            if (IsEmpty())
            {
                Console.WriteLine("[LinkedList] - IndexOf failed, linkedList {1} empty", IsEmpty() ? "is" : "is not");
                return false;
            }
            Node<T> currNode = head;
            while (currNode.Next != null && !currNode.Data.Equals(value))
            {
                currNode = currNode.Next;
            }
            if (currNode.Next == null && !currNode.Data.Equals(value))
            {
                return false;
            }
            return true;
        }

        //反转
        public void Reverse()
        {
            //递归方式
            if (IsEmpty())
            {
                Console.WriteLine("[LinkedList] - Reverse failed, linkedList {1} empty", IsEmpty() ? "is" : "is not");
                return;
            }
            head = _ReverseNode(head);
        }

        //递归
        //private Node<T> _ReverseNode(Node<T> head)
        //{
        //    if (head == null || head.Next == null)
        //    {
        //        return head;
        //    }
        //    Node<T> newHead = _ReverseNode(head.Next);
        //    head.Next.Next = head;
        //    head.Next = null;  //断链
        //    return newHead;
        //}

        private Node<T> _ReverseNode(Node<T> head)
        {
            Node<T> preNode = null;
            Node<T> currNode = head;
            Node<T> tempNode = null;
            while (currNode != null)
            {
                tempNode = currNode.Next;
                currNode.Next = preNode;
                preNode = currNode;
                currNode = tempNode;
            }
            return preNode;
        }

        //迭代
        //private Node<T> _ReverseNode(Node<T> head)
        //{
        //    if (head == null || head.Next == null)
        //    {
        //        return head;
        //    }
        //    Node<T> preNode = head;
        //    Node<T> currNode = preNode.Next;
        //    Node<T> tempNode = currNode.Next;

        //    while (currNode != null)
        //    {
        //        tempNode = currNode.Next;
        //        currNode.Next = preNode;
        //        preNode = currNode;
        //        currNode = tempNode;
        //    }
        //    head.Next = null;  //反转后最后一个结点，即反转前第一个结点，位置域需要置空
        //    return preNode;
        //}

        //获取链表中间值
        // 两个指针从头结点开始 第一个指针每次向后走一步 第二个指针每次向后走两步 当第二个指针走到链表尾部时 第一个指针指向链表中间位置
        public T GetMiddleValue(bool preferPrev = true)
        {
            if (IsEmpty())
            {
                Console.WriteLine("[LinkedList] - GetMiddleValue failed, linkedList {1} empty", IsEmpty() ? "is" : "is not");
                return default(T);
            }
            if (head.Next == null)
            {
                return head.Data;
            }
            Node<T> preNode = head;
            Node<T> quickNode = head;
            while (quickNode != null && quickNode.Next != null)
            {
                preNode = preNode.Next;
                quickNode = quickNode.Next.Next;
            }
            //奇数
            if (quickNode != null)
            {
                return preNode.Data;
            }
            //偶数
            else
            {
                return preferPrev ? preNode.Data : preNode.Next.Data;
            }
        }
 
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Node<T> currNode = head;
            sb.Append(currNode.Data.ToString() + ",");
            while (currNode.Next != null)
            {
                sb.Append(currNode.Next.Data.ToString() + ",");
                currNode = currNode.Next;
            }
            return sb.ToString().TrimEnd(',');
        }
    }


    public class Node<T>
    {
        private T data;                         //当前结点数据域
        public T Data
        {
            get { return data; }
            set { data = value; }
        }

        private Node<T> next;                   //下一个结点位置域
        public Node<T> Next
        {
            get { return next; }
            set { next = value; }
        }       

        public Node()
        {
            this.data = default(T);
            this.next = null;
        }

        public Node(T val)
        {
            this.data = val;
            this.next = null;
        }        
    }
}

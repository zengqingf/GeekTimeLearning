

using System;

namespace Tenmove.Runtime
{
    /// <summary>
    /// 将值类型转化为引用类型，该类型的分配会在堆上产生GC，应尽量少用
    /// </summary>
    /// <typeparam name="TValue">Value type</typeparam>
    public class ValueObject<TValue>
    {
        public ValueObject(TValue value)
        {
            m_Value = value;
        }

        private TValue m_Value;

        public TValue Value
        {
            set { m_Value = value; }
            get { return m_Value; }
        }
        
        public static implicit operator ValueObject<TValue>(TValue value)
        {
            return new ValueObject<TValue>(value);
        }
    }
}
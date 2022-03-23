

using System;

namespace Tenmove.Runtime
{
    /// <summary>
    /// ��ֵ����ת��Ϊ�������ͣ������͵ķ�����ڶ��ϲ���GC��Ӧ��������
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
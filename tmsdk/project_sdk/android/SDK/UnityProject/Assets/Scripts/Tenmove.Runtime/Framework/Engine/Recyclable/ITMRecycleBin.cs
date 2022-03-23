
using System;

namespace Tenmove.Runtime
{
    /// <summary>
    /// �������վ(�ɻ��ն���)
    /// </summary>
    public interface ITMRecycleBin
    {
        /// <summary>
        /// ����ָ�����͵Ŀɻ��ն���������
        /// </summary>
        /// <param name="type">ָ������</param>
        /// <param name="reserveCount">��������</param>
        /// <returns></returns>
        void SetReserveCountOfType(Type type, int reserveCount);

        /// <summary>
        /// ��ȡָ�����͵Ŀɻ��ն���������
        /// </summary>
        /// <param name="type">ָ������</param>
        /// <returns>ָ�����͵Ķ���������</returns>
        int GetResserveCountOfType(Type type);

        /// <summary>
        /// �������
        /// </summary>
        /// <returns>��������Ķ���</returns>
        T Acquire<T>() where T: Recyclable,new ();

        /// <summary>
        /// ���ն���
        /// </summary>
        /// <param name="obj">Ҫ�����յĶ���</param>
        void Recycle<T>(T obj) where T : Recyclable, new();

        /// <summary>
        /// ��������ָ�����͵Ķ���
        /// </summary>
        /// <param name="type">Ҫ������������</param>
        void ClearAllObjectOfType<T>() where T : Recyclable, new();

        /// <summary>
        /// ��ϴ����
        /// </summary>
        /// <param name="clearAll">�Ƿ��������ж�����������������У����Ϊ������Reserve��ָ���Ķ������</param>
        void Purge(bool clearAll);        
    }
}


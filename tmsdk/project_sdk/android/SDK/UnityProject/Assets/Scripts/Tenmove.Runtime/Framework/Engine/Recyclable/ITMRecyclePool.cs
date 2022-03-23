
namespace Tenmove.Runtime
{
    /// <summary>
    /// �ɻ��ն���أ�Ƶ�����������ٵ�ʵ����
    /// </summary>
    public interface ITMRecyclePool<T> where T : Recyclable, new()
    {
        /// <summary>
        /// ����������
        /// </summary>
        int ReserveCount
        {
            get;
        }

        /// <summary>
        /// δʹ�õĶ������
        /// </summary>
        int UnusedObjectCount
        {
            get;
        }

        /// <summary>
        /// ���ڱ�ʹ�õĶ������
        /// </summary>
        int UsingObjectCount
        {
            get;
        }

        /// <summary>
        /// �������Ĵ���
        /// </summary>
        int AcquireCount
        {
            get;
        }

        /// <summary>
        /// ���ն���Ĵ���
        /// </summary>
        int RecycleCount
        {
            get;
        }

        /// <summary>
        /// �����Ķ������
        /// </summary>
        int CreateCount
        {
            get;
        }

        /// <summary>
        /// �����ͷŶ���ĸ���
        /// </summary>
        int ReleaseCount
        {
            get;
        }

        /// <summary>
        /// ���ó��ӵ�Ԥ������������
        /// </summary>
        /// <param name="reserveCount">Ԥ������������</param>
        void SetReserveCount(int reserveCount);

        /// <summary>
        /// ���ó��ӵ�Ԥ������������
        /// </summary>
        /// <param name="reserveCount">Ԥ������������</param>
        void SetExpireTime(float expireTime);

        /// <summary>
        /// ���ó��ӵ�Ԥ������������
        /// </summary>
        /// <param name="reserveCount">Ԥ������������</param>
        void SetPriority(int priority);

        /// <summary>
        /// �������
        /// </summary>
        /// <returns>��������Ķ���</returns>
        T Acquire();

        /// <summary>
        /// ���ն���
        /// </summary>
        /// <param name="obj">Ҫ�����յĶ���</param>
        void Recycle(T obj);

        /// <summary>
        /// ��ϴ����
        /// </summary>
        /// <param name="clearAll">�Ƿ��������ж�����������������У����Ϊ������Reserve��ָ���Ķ������</param>
        void PurgePool(bool clearAll);
    }
}


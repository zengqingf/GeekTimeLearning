
namespace Tenmove.Runtime
{
    /// <summary>
    /// �ɻ��ն���Ƶ�����������ٵ�ʵ����
    /// </summary>
    public abstract class Recyclable
    {
        private bool m_IsRecycled = false;

        public abstract string Name
        {
            get;
        }

        public bool IsRecycled
        {
            get { return m_IsRecycled; }
        }

        public abstract bool IsValid
        {
            get;
        }

        public virtual void OnCreate()
        {
        }

        /// <summary>
        /// �����ٴ�ʹ�ô���
        /// </summary>
        public virtual void OnReuse()
        {
            m_IsRecycled = false;
        }

        /// <summary>
        /// ������մ���
        /// </summary>
        public virtual void OnRecycle()
        {
            m_IsRecycled = true;
        }

        /// <summary>
        /// ���ٴ���
        /// </summary>
        public abstract void OnRelease();
    }
}


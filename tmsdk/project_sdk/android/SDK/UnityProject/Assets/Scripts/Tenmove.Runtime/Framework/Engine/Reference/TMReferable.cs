

using System;

namespace Tenmove.Runtime
{
    public abstract class Referable
    {
        public abstract void OnSpawn();

        public abstract void OnUnspawn();

        public abstract void OnRelease();

        public abstract void Lock(bool bLock);

        public abstract string Name
        {
            get;
        }

        public abstract int NameHashCode
        {
            get;
        }

        public abstract bool IsInUse
        {
            get;
        }

        public abstract bool IsLocked
        {
            get;
        }

        public abstract long LastUseTime
        {
            get;
        }

        public abstract int SpawnCount
        {
            get;
        }
    }

    public sealed class Referable<T> : Referable
    {
        private readonly string m_Name;
        private readonly int    m_NameHashCode;
        private readonly T      m_Target;
        private bool            m_Locked;
        private long            m_LastUseTime;
        private int             m_SpawnCount;
        private Action<T>       NotifyOnRelease;

        public Referable(T t, string name, Action<T> notifyOnRelease = null)
        {
            m_Name = name;
            m_NameHashCode = m_Name.GetHashCode();
            m_Target = t;
            NotifyOnRelease = notifyOnRelease;

            m_SpawnCount = 0;
            m_LastUseTime = Utility.Time.GetTicksNow();

            m_Locked = false;
        }

        public override string Name
        {
            get { return m_Name; }
        }

        public override int NameHashCode
        {
            get { return m_NameHashCode; }
        }

        public override bool IsInUse
        {
            get { return SpawnCount > 0; }
        }

        public override bool IsLocked
        {
            get { return m_Locked; }
        }

        public override long LastUseTime
        {
            get { return m_LastUseTime; }
        }

        public override int SpawnCount
        {
            get { return m_SpawnCount; }
        }

        public T Target
        {
            get { return m_Target; }
        }

        public override void OnRelease()
        {
            if (NotifyOnRelease != null)
                NotifyOnRelease(m_Target);
        }

        public override void OnSpawn()
        {
            m_SpawnCount++;
            m_LastUseTime = Utility.Time.GetTicksNow();
        }

        public override void OnUnspawn()
        {
            m_SpawnCount--;
            m_LastUseTime = Utility.Time.GetTicksNow();
        }

        public override void Lock(bool bLock) { }
    }

}
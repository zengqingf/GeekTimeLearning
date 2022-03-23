
using System;

namespace Tenmove.Runtime
{
    public abstract class ByteBuffer
    {
        public enum SeekOrigin
        {
            Begin,
            Current,
            End,
        }

        private static readonly int DEFAULT_CAPACITY = 256;

        private readonly bool m_IsLittleEndian;
        private readonly bool m_IsFixedSize;

        protected byte[] m_BufferBytes;
        protected long m_Position;
        protected long m_Length;

        protected ByteBuffer(int capacity,bool fixedSize)
        {
            if (capacity <= 0)
            {
                Debugger.LogWarning("Capacity must be a positive value,Auto set to {0} bytes.", DEFAULT_CAPACITY);
                capacity = DEFAULT_CAPACITY;
            }
            m_IsFixedSize = fixedSize;
            m_IsLittleEndian = BitConverter.IsLittleEndian;
            m_BufferBytes = new byte[capacity];
            m_Position = 0;
            m_Length = 0;
        }

        public long Position
        {
            get { return m_Position; }
        }

        public long Length
        {
            get { return m_Length; }
        }

        public byte[] Buffer
        {
            get
            {
                byte[] buf = new byte[Length];
                System.Buffer.BlockCopy(m_BufferBytes, 0, buf, 0, buf.Length);
                return buf;
            }
        }

        public bool IsLittleEndian
        {
            get { return m_IsLittleEndian; }
        }

        public void Reset()
        {
            m_Position = 0;
        }

        public abstract byte ReadByte();
        public abstract ushort ReadShort();
        public abstract uint ReadInt();
        public abstract ulong ReadLong();
        public abstract float ReadFloat();
        public abstract double ReadDouble();

        public abstract void WriteByte(byte data);
        public abstract void WriteShort(ushort data);
        public abstract void WriteInt(uint data);
        public abstract void WriteLong(ulong data);
        public abstract void WriteFloat(float data);
        public abstract void WriteDouble(double data);

        public void Seek(SeekOrigin origin, long offset)
        {
            long pos = 0;
            switch (origin)
            {
                case SeekOrigin.Begin: pos = offset; break;
                case SeekOrigin.Current: pos = m_Position + offset; break;
                case SeekOrigin.End: pos = m_Length + offset; break;
            }

            if (pos < 0)
                pos = 0;
            if (pos > m_Length)
                pos = Length;

            m_Position = pos;
        }

        protected void _CheckRange(long offset,int size)
        {
            if (offset >= 0 && offset + size < m_BufferBytes.Length)
                return;

            Debugger.AssertFailed("Out of byte buffer range!");
        }

        protected void _EnsureSize(long offset, int size)
        {
            if (m_IsFixedSize)
                return;

            if(offset + size >= m_BufferBytes.Length)
            {
                long newBufSize = m_BufferBytes.Length + Utility.Math.Max((m_BufferBytes.Length >> 1), DEFAULT_CAPACITY);
                byte[] newBuffer = new byte[newBufSize];
                System.Buffer.BlockCopy(m_BufferBytes, 0, newBuffer, 0, m_BufferBytes.Length);
                m_BufferBytes = newBuffer;
            }
        }
    }
}
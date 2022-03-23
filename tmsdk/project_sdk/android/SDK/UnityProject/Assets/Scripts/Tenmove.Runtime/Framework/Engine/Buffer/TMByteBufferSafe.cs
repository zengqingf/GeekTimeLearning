


using System;

namespace Tenmove.Runtime
{
    public class ByteBufferSafe : ByteBuffer
    {
        private readonly uint[] m_IntArrayCache;
        private readonly float[] m_FloatArrayCache;
        private readonly ulong[] m_LongArrayCache;
        private readonly double[] m_DoubleArrayCache;

        public ByteBufferSafe(int capacity,bool fixedSize)
            : base(capacity, fixedSize)
        {
            m_IntArrayCache = new uint[1] { 0u };
            m_FloatArrayCache = new float[1] { 0.0f };
            m_LongArrayCache = new ulong[1] { 0ul };
            m_DoubleArrayCache = new double[1] { 0.0 };
        }

        public sealed override byte ReadByte()
        {
            _CheckRange(m_Position, sizeof(byte));
            byte res = m_BufferBytes[m_Position];
            m_Position += sizeof(byte);
            return res;
        }

        public sealed override ushort ReadShort()
        {
            return (ushort)_ReadWithLittleEndian(sizeof(ushort));
        }

        public sealed override uint ReadInt()
        {
            return (uint)_ReadWithLittleEndian(sizeof(uint));
        }

        public sealed override ulong ReadLong()
        {
            return (ulong)_ReadWithLittleEndian(sizeof(ulong));
        }

        public sealed override float ReadFloat()
        {
            uint i = (uint)_ReadWithLittleEndian(sizeof(float));
            m_IntArrayCache[0] = i;
            System.Buffer.BlockCopy(m_IntArrayCache, 0, m_FloatArrayCache, 0, sizeof(float));
            return m_FloatArrayCache[0];
        }

        public sealed override double ReadDouble()
        {
            ulong i = _ReadWithLittleEndian( sizeof(double));
            m_LongArrayCache[0] = i;
            System.Buffer.BlockCopy(m_LongArrayCache, 0, m_DoubleArrayCache, 0, sizeof(double));
            return m_DoubleArrayCache[0];
        }

        public sealed override void WriteByte(byte data)
        {
            _EnsureSize(m_Position, sizeof(byte));
            _CheckRange(m_Position, sizeof(byte));
            m_BufferBytes[m_Position] = data;
            m_Position += sizeof(byte);
        }

        public sealed override void WriteShort(ushort data)
        {
            _WriteWithLittleEndian(sizeof(ushort),data);
        }

        public sealed override void WriteInt( uint data)
        {
            _WriteWithLittleEndian(sizeof(uint), data);
        }

        public sealed override void WriteLong( ulong data)
        {
            _WriteWithLittleEndian(sizeof(ulong), data);
        }

        public sealed override void WriteFloat(float data)
        {
            m_FloatArrayCache[0] = data;
            System.Buffer.BlockCopy(m_FloatArrayCache, 0, m_IntArrayCache, 0, sizeof(float));
            _WriteWithLittleEndian(sizeof(float), m_IntArrayCache[0]);
        }

        public sealed override void WriteDouble( double data)
        {
            m_DoubleArrayCache[0] = data;
            System.Buffer.BlockCopy(m_DoubleArrayCache, 0, m_LongArrayCache, 0, sizeof(double));
            _WriteWithLittleEndian(sizeof(double), m_LongArrayCache[0]);
        }

        private ulong _ReadWithLittleEndian(int bytesCount)
        {
            _CheckRange(m_Position, bytesCount);
            ulong r = 0;
            if (IsLittleEndian)
            {
                for (int i = 0; i < bytesCount; i++)
                    r |= (ulong)m_BufferBytes[m_Position + i] << i * 8;
            }
            else
            {
                for (int i = 0; i < bytesCount; i++)
                    r |= (ulong)m_BufferBytes[m_Position + bytesCount - 1 - i] << i * 8;
            }

            m_Position += bytesCount;
            return r;
        }

        private void _WriteWithLittleEndian( int bytesCount, ulong data)
        {
            _EnsureSize(m_Position, bytesCount);
            _CheckRange(m_Position, bytesCount);
            if (IsLittleEndian)
            {
                for (int i = 0; i < bytesCount; i++)
                    m_BufferBytes[m_Position + i] = (byte)(data >> i * 8);
            }
            else
            {
                for (int i = 0; i < bytesCount; i++)
                    m_BufferBytes[m_Position + bytesCount - 1 - i] = (byte)(data >> i * 8);
            }

            m_Position += bytesCount;
            if (m_Position > m_Length)
                m_Length = m_Position;
        }
    }
}
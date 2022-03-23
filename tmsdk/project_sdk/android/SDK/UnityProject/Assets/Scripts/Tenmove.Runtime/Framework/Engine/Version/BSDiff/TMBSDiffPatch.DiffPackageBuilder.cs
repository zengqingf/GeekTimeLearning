
using System;
using System.IO;

namespace Tenmove.Runtime
{
    public partial class BSDiffPatch 
    {
        private class DiffPackageBuilder
        {
            public byte[] Build(byte[] oldbuf, byte[] newbuf, Stream zipStream)
            {
                using (MemoryStream diffDataStream = Utility.Memory.OpenStream(CONST_DEFAULT_CACHE_SIZE) as MemoryStream)
                {
                    int oldSize = oldbuf.Length;
                    int newSize = newbuf.Length;
                    int[] arrayI = new int[oldSize + 1];
                    _QSuffixSort(arrayI, new int[oldSize + 1], oldbuf, oldSize);

                    /// diff block
                    long diffBlockLen = 0;
                    byte[] diffBlockBuf = new byte[newSize];

                    /// extra block
                    long extraBlockLen = 0;
                    byte[] extraBlockBuf = new byte[newSize];

                    byte[] longTypeCache = new byte[8];
                    _EncodeLongType(0, longTypeCache);

                    diffDataStream.Seek(0, SeekOrigin.Begin);
                    diffDataStream.Write(CONST_DIFF_FILE_HEAD_TAG, 0, CONST_DIFF_FILE_HEAD_TAG.Length);
                    long ctrlBlockLenPos = diffDataStream.Position;
                    diffDataStream.Write(longTypeCache, 0, longTypeCache.Length);
                    long diff_block_len_pos = diffDataStream.Position;
                    diffDataStream.Write(longTypeCache, 0, longTypeCache.Length);
                    _EncodeLongType(newSize, longTypeCache);
                    diffDataStream.Write(longTypeCache, 0, longTypeCache.Length);
                    diffDataStream.Flush();

                    long diffDataHeaderSize = diffDataStream.Length;

                    GZip.GZipStream ctrlDataZipStream = new GZip.GZipStream(diffDataStream, GZip.CompressionMode.Compress, true);
                    BinaryWriter ctrlDataZipWriter = new BinaryWriter(ctrlDataZipStream);

                    //BinaryWriter ctrlDataZipWriter = new BinaryWriter(diffDataStream,System.Text.Encoding.ASCII);

                    //ctrlDataZipWriter.Write(new byte[] { (byte)'C', (byte)'T', (byte)'R', (byte)'L', (byte)'B', (byte)'L', (byte)'K', (byte)' ' });
                    //diffDataStream.Write(new byte[] { (byte)'C', (byte)'T', (byte)'R', (byte)'L', (byte)'B', (byte)'L', (byte)'K', (byte)' ' },0,8);

                    ByteBufferSafe ctrlDataByteBuf = new ByteBufferSafe(256,false);

                    int oldScore, scsc;
                    int overlap, ss, lens;
                    int scan = 0;
                    int matchLen = 0;
                    int lastScan = 0;
                    int lastPos = 0;
                    int lastOffset = 0;
                    int pos = 0;

                    while (scan < newSize)
                    {
                        oldScore = 0;

                        for (scsc = scan += matchLen; scan < newSize; scan++)
                        {
                            ///  oldbuf[0...oldsize] newbuf[scan...newSize]. pos.value£¬scan
                            matchLen = _Search(arrayI, oldbuf, oldSize, newbuf, newSize, scan, 0, oldSize, ref pos);

                            for (; scsc < scan + matchLen; scsc++)
                            {
                                if ((scsc + lastOffset < oldSize) && (oldbuf[scsc + lastOffset] == newbuf[scsc]))
                                {
                                    ++oldScore;
                                }
                            }

                            if (((matchLen == oldScore) && (matchLen != 0)) || (matchLen > oldScore + 8))
                            {
                                break;
                            }

                            if ((scan + lastOffset < oldSize) && (oldbuf[scan + lastOffset] == newbuf[scan]))
                            {
                                --oldScore;
                            }
                        }

                        if ((matchLen != oldScore) || (scan == newSize))
                        {

                            int equalNum = 0;
                            int sf = 0;
                            int lenFromOld = 0;
                            for (int i = 0; (lastScan + i < scan) && (lastPos + i < oldSize);)
                            {
                                if (oldbuf[lastPos + i] == newbuf[lastScan + i])
                                {
                                    ++equalNum;
                                }
                                ++i;
                                if (equalNum * 2 - i > sf * 2 - lenFromOld)
                                {
                                    sf = equalNum;
                                    lenFromOld = i;
                                }
                            }

                            int lenb = 0;
                            if (scan < newSize)
                            {
                                equalNum = 0;
                                int sb = 0;
                                for (int i = 1; (scan >= lastScan + i) && (pos >= i); i++)
                                {
                                    if (oldbuf[pos - i] == newbuf[scan - i])
                                    {
                                        ++equalNum;
                                    }
                                    if (equalNum * 2 - i > sb * 2 - lenb)
                                    {
                                        sb = equalNum;
                                        lenb = i;
                                    }
                                }
                            }

                            if (lastScan + lenFromOld > scan - lenb)
                            {
                                overlap = (lastScan + lenFromOld) - (scan - lenb);
                                equalNum = 0;
                                ss = 0;
                                lens = 0;
                                for (int i = 0; i < overlap; i++)
                                {
                                    if (newbuf[lastScan + lenFromOld - overlap + i] == oldbuf[lastPos + lenFromOld - overlap + i])
                                    {
                                        ++equalNum;
                                    }
                                    if (newbuf[scan - lenb + i] == oldbuf[pos - lenb + i])
                                    {
                                        --equalNum;
                                    }
                                    if (equalNum > ss)
                                    {
                                        ss = equalNum;
                                        lens = i + 1;
                                    }
                                }

                                lenFromOld += lens - overlap;
                                lenb -= lens;
                            }

                            /// ? byte casting introduced here -- might affect things
                            for (int i = 0; i < lenFromOld; i++)
                            {
                                diffBlockBuf[diffBlockLen + i] = (byte)(newbuf[lastScan + i] - oldbuf[lastPos + i]);
                            }

                            for (int i = 0; i < (scan - lenb) - (lastScan + lenFromOld); i++)
                            {
                                extraBlockBuf[extraBlockLen + i] = newbuf[lastScan + lenFromOld + i];
                            }

                            diffBlockLen += lenFromOld;
                            extraBlockLen += (scan - lenb) - (lastScan + lenFromOld);

                            /// Write control block entry (3 x int)                        
                            /// ctrlDataZipWriter.Write(lenFromOld);  /// oldbuf
                            /// ctrlDataZipWriter.Write((scan - lenb) - (lastScan + lenFromOld));/// diffBufextraBlock
                            /// ctrlDataZipWriter.Write((pos - lenb) - (lastPos + lenFromOld));  /// oldbuf
                            //ctrlDataByteBuf.WriteLong((ulong)_DecodeLongType(new byte[] { (byte)'C', (byte)'T', (byte)'R', (byte)'L', (byte)'D', (byte)'A', (byte)'T', (byte)'A' }));
                            ctrlDataByteBuf.WriteInt((uint)(lenFromOld));  /// oldbuf
                            ctrlDataByteBuf.WriteInt((uint)((scan - lenb) - (lastScan + lenFromOld)));/// diffBufextraBlock
                            ctrlDataByteBuf.WriteInt((uint)((pos - lenb) - (lastPos + lenFromOld)));  /// oldbuf

                            lastScan = scan - lenb;
                            lastPos = pos - lenb;
                            lastOffset = pos - scan;
                        } /// end if
                    } /// end while loop

                    byte[] data = ctrlDataByteBuf.Buffer;
                    /// UnityEngine.Debug.LogError("len:" + data.Length);
                    ctrlDataZipWriter.Write(data);
                    ctrlDataZipWriter.Flush();
                    ctrlDataZipStream.Flush();
                    ctrlDataZipStream.Close();

                    /// now compressed ctrlBlockLen
                    /// 
                     
                    /// UnityEngine.Debug.LogError("diffDataStream.Length:" + diffDataStream.Length);
                    /// UnityEngine.Debug.LogError("diffDataHeaderSize:" + diffDataHeaderSize);
                    long ctrlDataBlockLen = diffDataStream.Length - diffDataHeaderSize;

                    /// Write diff block
                    GZip.GZipStream diffBlockZipStream = new GZip.GZipStream(diffDataStream, GZip.CompressionMode.Compress, true);
                    //BinaryWriter diffBlockZipStream = new BinaryWriter(diffDataStream);

                    //diffBlockZipStream.Write(new byte[] { (byte)'D', (byte)'I', (byte)'F', (byte)'F', (byte)'B', (byte)'L', (byte)'K', (byte)' ' });
                    //diffDataStream.Write(new byte[] { (byte)'D', (byte)'I', (byte)'F', (byte)'F', (byte)'B', (byte)'L', (byte)'K', (byte)' ' },0,8);
                    diffBlockZipStream.Write(diffBlockBuf, 0, (int)diffBlockLen);
                    diffBlockZipStream.Flush();
                    diffBlockZipStream.Close();

                    diffDataStream.Flush();

                    long diffDataBlockLen = diffDataStream.Length - ctrlDataBlockLen - diffDataHeaderSize;

                    /// Write extra block
                    GZip.GZipStream extraBlockZipStream = new GZip.GZipStream(diffDataStream, GZip.CompressionMode.Compress, true);
                    //BinaryWriter extraBlockZipStream = new BinaryWriter(diffDataStream);

                    //diffDataStream.Write(new byte[] { (byte)'E', (byte)'X', (byte)'T', (byte)'R', (byte)'B', (byte)'L', (byte)'K', (byte)' ' },0,8);
                    //extraBlockZipStream.Write(new byte[] { (byte)'E', (byte)'X', (byte)'T', (byte)'R', (byte)'B', (byte)'L', (byte)'K', (byte)' ' });
                    extraBlockZipStream.Write(extraBlockBuf, 0, (int)extraBlockLen);
                    extraBlockZipStream.Flush();
                    extraBlockZipStream.Close();

                    diffDataStream.Flush();

                    ///UnityEngine.Debug.LogError("ctrlDataBlockLen:" + ctrlDataBlockLen);
                    diffDataStream.Seek((int)ctrlBlockLenPos, SeekOrigin.Begin);
                    _EncodeLongType(ctrlDataBlockLen, longTypeCache);

                    ///UnityEngine.Debug.LogErrorFormat("longTypeCache:'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'." ,longTypeCache[0], longTypeCache[1], longTypeCache[2], longTypeCache[3], longTypeCache[4], longTypeCache[5], longTypeCache[6],longTypeCache[7]);
                    diffDataStream.Write(longTypeCache, 0, longTypeCache.Length);

                    diffDataStream.Seek((int)diff_block_len_pos, SeekOrigin.Begin);
                    _EncodeLongType(diffDataBlockLen, longTypeCache);
                    diffDataStream.Write(longTypeCache, 0, longTypeCache.Length);

                    diffDataStream.Flush();

                    byte[] databuf = new byte[diffDataStream.Length];
                    Buffer.BlockCopy(diffDataStream.GetBuffer(), 0, databuf, 0, databuf.Length);                   
                    return databuf;
                }
            }

            protected void _Split(int[] I, int[] V, int start, int len, int h)
            {
                int i, j, k, x, tmp, jj, kk;

                if (len < 16)
                {
                    for (k = start; k < start + len; k += j)
                    {
                        j = 1;
                        x = V[I[k] + h];
                        for (i = 1; k + i < start + len; i++)
                        {
                            if (V[I[k + i] + h] < x)
                            {
                                x = V[I[k + i] + h];
                                j = 0;
                            }

                            if (V[I[k + i] + h] == x)
                            {
                                tmp = I[k + j];
                                I[k + j] = I[k + i];
                                I[k + i] = tmp;
                                j++;
                            }

                        }

                        for (i = 0; i < j; i++)
                        {
                            V[I[k + i]] = k + j - 1;
                        }
                        if (j == 1)
                        {
                            I[k] = -1;
                        }
                    }

                    return;
                }

                x = V[I[start + len / 2] + h];
                jj = 0;
                kk = 0;
                for (i = start; i < start + len; i++)
                {
                    if (V[I[i] + h] < x)
                    {
                        jj++;
                    }
                    if (V[I[i] + h] == x)
                    {
                        kk++;
                    }
                }

                jj += start;
                kk += jj;

                i = start;
                j = 0;
                k = 0;
                while (i < jj)
                {
                    if (V[I[i] + h] < x)
                    {
                        i++;
                    }
                    else if (V[I[i] + h] == x)
                    {
                        tmp = I[i];
                        I[i] = I[jj + j];
                        I[jj + j] = tmp;
                        j++;
                    }
                    else
                    {
                        tmp = I[i];
                        I[i] = I[kk + k];
                        I[kk + k] = tmp;
                        k++;
                    }

                }

                while (jj + j < kk)
                {
                    if (V[I[jj + j] + h] == x)
                    {
                        j++;
                    }
                    else
                    {
                        tmp = I[jj + j];
                        I[jj + j] = I[kk + k];
                        I[kk + k] = tmp;
                        k++;
                    }

                }

                if (jj > start)
                {
                    _Split(I, V, start, jj - start, h);
                }

                for (i = 0; i < kk - jj; i++)
                {
                    V[I[jj + i]] = kk - 1;
                }

                if (jj == kk - 1)
                {
                    I[jj] = -1;
                }

                if (start + len > kk)
                {
                    _Split(I, V, kk, start + len - kk, h);
                }
            }

            protected void _QSuffixSort(int[] I, int[] V, byte[] old, int oldsize)
            {
                /// int oldsize = old.length;
                int[] buckets = new int[256];

                for (int i = 0; i < oldsize; i++)
                {
                    buckets[old[i] & 0xff]++;
                }

                for (int i = 1; i < 256; i++)
                {
                    buckets[i] += buckets[i - 1];
                }

                for (int i = 255; i > 0; i--)
                {
                    buckets[i] = buckets[i - 1];
                }

                buckets[0] = 0;

                for (int i = 0; i < oldsize; i++)
                {
                    I[++buckets[old[i] & 0xff]] = i;
                }

                I[0] = oldsize;
                for (int i = 0; i < oldsize; i++)
                {
                    V[i] = buckets[old[i] & 0xff];
                }
                V[oldsize] = 0;

                for (int i = 1; i < 256; i++)
                {
                    if (buckets[i] == buckets[i - 1] + 1)
                    {
                        I[buckets[i]] = -1;
                    }
                }

                I[0] = -1;

                for (int h = 1; I[0] != -(oldsize + 1); h += h)
                {
                    int len = 0;
                    int i;
                    for (i = 0; i < oldsize + 1;)
                    {
                        if (I[i] < 0)
                        {
                            len -= I[i];
                            i -= I[i];
                        }
                        else
                        {
                            /// if(len) I[i-len]=-len;
                            if (len != 0)
                            {
                                I[i - len] = -len;
                            }
                            len = V[I[i]] + 1 - i;
                            _Split(I, V, i, len, h);
                            i += len;
                            len = 0;
                        }

                    }

                    if (len != 0)
                    {
                        I[i - len] = -len;
                    }
                }

                for (int i = 0; i < oldsize + 1; i++)
                {
                    I[V[i]] = i;
                }
            }

            protected int _MatchLen(byte[] _old, int _oldsize, int _oldoffset, byte[] _new, int _newsize, int _newoffset)
            {
                int end = _GetMin(_oldsize - _oldoffset, _newsize - _newoffset);
                for (int i = 0; i < end; ++i)
                {
                    if (_old[_oldoffset + i] != _new[_newoffset + i])
                        return i;
                }

                return end;
            }

            protected int _Search(int[] I, byte[] _old, int _oldsize, byte[] _new,
                int _newsize, int _newoffset, int start, int end, ref int pos)
            {
                int x, y;

                if (end - start < 2)
                {
                    x = _MatchLen(_old, _oldsize, I[start], _new, _newsize, _newoffset);
                    y = _MatchLen(_old, _oldsize, I[end], _new, _newsize, _newoffset);

                    if (x > y)
                    {
                        pos = I[start];
                        return x;
                    }
                    else
                    {
                        pos = I[end];
                        return y;
                    }
                }

                x = start + (end - start) / 2;

                if (_MemoryCompare(_old, _oldsize, I[x], _new, _newsize, _newoffset) < 0)
                {
                    return _Search(I, _old, _oldsize, _new, _newsize, _newoffset, x, end, ref pos);
                }
                else
                {
                    return _Search(I, _old, _oldsize, _new, _newsize, _newoffset, start, x, ref pos);
                }
            }

            protected int _MemoryCompare(byte[] _leftbuf, int _leftbuflen, int _leftoffset, byte[] _rightbuf, int _rightbuflen, int _rightoffset)
            {
                int _count = _GetMin(_leftbuflen - _leftoffset, _rightbuflen - _rightoffset);
                for (int i = 0; i < _count; ++i)
                {
                    int _left = _leftbuf[i + _leftoffset];
                    int _right = _rightbuf[i + _rightoffset];
                    if (_left != _right)
                        return _left < _right ? -1 : 1;
                }

                return 0;
            }
        }
    }
}
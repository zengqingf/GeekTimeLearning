

using System.IO;

namespace Tenmove.Runtime
{
    public partial class BSDiffPatch 
    {
        private class DiffPackagePatcher
        {
            public bool Patch(string oldfilepath, string newfilepath, byte[] diffbuf, int extlen)
            {
                using (Stream old_file_stream = Utility.File.OpenRead(oldfilepath))
                {
                    long oldsize = old_file_stream.Length;

                    using (Stream new_file_stream = Utility.File.OpenWrite(newfilepath, true))
                    {
                        using (MemoryStream diff_data_stream = Utility.Memory.OpenStream(diffbuf) as MemoryStream)
                        {
                            byte[] head_data_cache = new byte[8];
                            diff_data_stream.Read(head_data_cache, 0, head_data_cache.Length);
                            if (_IsBSDiffFile(head_data_cache))
                            {
                                diff_data_stream.Read(head_data_cache, 0, head_data_cache.Length);
                                long ctrl_data_block_len = _DecodeLongType(head_data_cache);
                                diff_data_stream.Read(head_data_cache, 0, head_data_cache.Length);
                                long diff_data_block_len = _DecodeLongType(head_data_cache);
                                diff_data_stream.Read(head_data_cache, 0, head_data_cache.Length);
                                long newsize = _DecodeLongType(head_data_cache);

                                long diff_file_header_len = diff_data_stream.Position;
                                int stream_offset = (int)diff_file_header_len;
                                int stream_length = (int)ctrl_data_block_len;
                                using (MemoryStream ctrl_block_data_stream = new MemoryStream(diffbuf, stream_offset, stream_length))
                                {
                                    GZip.GZipStream ctrl_block_unzip_stream = new GZip.GZipStream(ctrl_block_data_stream, GZip.CompressionMode.Decompress);
                                    BinaryReader ctrl_block_unzip_reader = new BinaryReader(ctrl_block_unzip_stream);

                                    stream_offset = (int)(diff_file_header_len + ctrl_data_block_len);
                                    stream_length = (int)diff_data_block_len;
                                    using (MemoryStream diff_block_data_stream = new MemoryStream(diffbuf, stream_offset, stream_length))
                                    {
                                        GZip.GZipStream diff_block_unzip_stream = new GZip.GZipStream(diff_block_data_stream, GZip.CompressionMode.Decompress);

                                        stream_offset = (int)(diff_file_header_len + ctrl_data_block_len + diff_data_block_len);
                                        stream_length = (int)(diffbuf.Length - stream_offset);
                                        using (MemoryStream extra_block_data_stream = new MemoryStream(diffbuf, stream_offset, stream_length))
                                        {
                                            GZip.GZipStream extra_block_unzip_stream = new GZip.GZipStream(extra_block_data_stream, GZip.CompressionMode.Decompress);

                                            int oldpos = 0;
                                            int newpos = 0;
                                            int[] ctrl = new int[3];

                                            /// int nbytes;
                                            while (newpos < newsize)
                                            {
                                                for (int i = 0; i <= 2; i++)
                                                {
                                                    ctrl[i] = ctrl_block_unzip_reader.ReadInt32();
                                                }

                                                if (newpos + ctrl[0] > newsize)
                                                {
                                                    new_file_stream.Close();
                                                    return false;
                                                }

                                                /// Read ctrl[0] bytes from diffBlock stream
                                                byte[] buffer = new byte[ctrl[0]];
                                                if (!_ReadBufFromFile(diff_block_unzip_stream, buffer, 0, ctrl[0]))
                                                {
                                                    new_file_stream.Close();
                                                    return false;
                                                }

                                                byte[] old_buffer = new byte[ctrl[0]];
                                                if (old_file_stream.Read(old_buffer, 0, (int)ctrl[0]) < ctrl[0])
                                                {
                                                    new_file_stream.Close();
                                                    return false;
                                                }
                                                for (int i = 0; i < ctrl[0]; i++)
                                                {
                                                    if ((oldpos + i >= 0) && (oldpos + i < oldsize))
                                                    {
                                                        buffer[i] += old_buffer[i];
                                                    }
                                                }
                                                new_file_stream.Write(buffer, 0, buffer.Length);

                                                newpos += ctrl[0];
                                                oldpos += ctrl[0];

                                                if (newpos + ctrl[1] > newsize)
                                                {
                                                    new_file_stream.Close();
                                                    return false;
                                                }

                                                buffer = new byte[ctrl[1]];
                                                if (!_ReadBufFromFile(extra_block_unzip_stream, buffer, 0, ctrl[1]))
                                                {
                                                    new_file_stream.Close();
                                                    return false;
                                                }
                                                new_file_stream.Write(buffer, 0, buffer.Length);
                                                new_file_stream.Flush();

                                                newpos += ctrl[1];
                                                oldpos += ctrl[2];
                                                old_file_stream.Seek(oldpos, SeekOrigin.Begin);
                                            }

                                            extra_block_data_stream.Close();
                                            diff_block_data_stream.Close();
                                            ctrl_block_data_stream.Close();

                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Debugger.AssertFailed("Corrupt by wrong patch file.");
                                return false;
                            }
                        }
                    }
                }
            }
        }
    }
}
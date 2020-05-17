using System;
using System.Collections.Generic;
using System.IO;
using Core.Net;

namespace Core.IO
{
    /// <summary>
    /// 用于写字节数组的接口
    /// @author fanflash.org
    /// </summary>
    public interface IByteArrayWriter
    {
        MemoryStream GetStream();
        void WriteByte(byte value);
        void WriteI16(Int16 value);
        void WriteU16(UInt16 value);
        void WriteI32(Int32 value);
        void WriteU32(UInt32 value);
        void WriteI64(Int64 value);
        void WriteU64(UInt64 value);
        void WriteFloat(float value);
        void WriteDouble(Double value);
        void WriteStr(string value, int len = -1);
        void WriteBytes(byte[] value, bool hasWriteLen = false);
        void WriteArray<T>(List<T> value);
    }
}

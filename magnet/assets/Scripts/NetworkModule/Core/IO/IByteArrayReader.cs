using System;
using System.Collections.Generic;
using System.IO;
using Core.Net;

namespace Core.IO
{
    /// <summary>
    /// 用于读字节数组的接口
    ///  @author fanflash.org
    /// </summary>
    public interface IByteArrayReader
    {
        MemoryStream GetStream();
        void ReadByte(ref byte value);
        void ReadI16(ref Int16 value);
        void ReadU16(ref UInt16 value);
        void ReadI32(ref Int32 value);
        void ReadU32(ref UInt32 value);
        void ReadI64(ref Int64 value);
        void ReadU64(ref UInt64 value);
        void ReadFloat(ref float value);
        void ReadDouble(ref Double value);
        void ReadStr(ref string value, int len = -1);
        byte[] ReadBytes(int length = int.MaxValue);
        void ReadArray<T>(out List<T> value) where T : new();
        byte[] Dump();
    }
}

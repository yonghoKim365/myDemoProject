using System;
using Core.Net;

namespace Core.IO
{
    /// <summary>
    /// 用于消息包中数组项的编码与解码
    /// @authon fanflash.org
    /// </summary>
    public interface IArrItemCoder
    {
        /// <summary>
        /// 封包
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        void Code(IByteArrayWriter writer, ref object value);

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        void Decode<T>(IByteArrayReader reader, out object value) where T : new();
    }

    public class MsgBaseCoder : IArrItemCoder
    {
        public void Code(IByteArrayWriter writer, ref object value)
        {
            MessageBase valueEx = value as MessageBase;
            if (valueEx == null)
            {
                throw new NullReferenceException("转换为空");
            }
            valueEx.PackMsg(writer);
        }

        public void Decode<T>(IByteArrayReader reader, out object value) where T : new()
        {
            value = new T();
            MessageBase valueEx = value as MessageBase;
            if (valueEx == null)
            {
                throw new NullReferenceException("转换为空");
            }
            valueEx.UnPackMsg(reader);
        }
    }

    public class U32Coder:IArrItemCoder
    {
        public void Code(IByteArrayWriter writer, ref object value)
        {
            writer.WriteU32((uint)value);
        }

        public void Decode<T>(IByteArrayReader reader, out object value) where T : new()
        {
            UInt32 valueEx = 0;
            reader.ReadU32(ref valueEx);
            value = valueEx;
        }
    }

    public class I32Coder : IArrItemCoder
    {
        public void Code(IByteArrayWriter writer, ref object value)
        {
            writer.WriteI32((Int32)value);
        }

        public void Decode<T>(IByteArrayReader reader, out object value) where T : new()
        {
            Int32 valueEx = 0;
            reader.ReadI32(ref valueEx);
            value = valueEx;
        }
    }
}

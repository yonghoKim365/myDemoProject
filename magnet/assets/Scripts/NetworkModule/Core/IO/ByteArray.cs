using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Core.Net;

namespace Core.IO
{
    /// <summary>
    /// 用于代替byte[]，方便消息包读写的类
    /// @author fanflash.org
    /// </summary>
    public class ByteArray:Disposer,IByteArrayReader,IByteArrayWriter
    {
        //实际使用用U32存时，使用的精度，如果小于零则使用原类型来存
        const float FLOAT_PREC = 1000f;
        const float FLOAT_PREC_B = 1/FLOAT_PREC;
        //实际使用用U64存时，使用的精度，如果小于零则使用原类型来存
        const Double DOUBLE_PREC = 1000000;
        const Double DOUBLE_PREC_B = 1/DOUBLE_PREC;

        private MemoryStream mBytes;
        private BinaryReader mReader;
        private BinaryWriter mWriter;

        private readonly bool mUseNetworkOrder;
        private readonly Dictionary<Type, IArrItemCoder> mArrItemCoderMap; 
      
        public ByteArray(byte[] bytes = null, int pos = 0, bool useNetworkOrder = false)
        {
            if (bytes == null)
            {
                mBytes = new MemoryStream();
            }
            else
            {
                mBytes = new MemoryStream(bytes);
            }
            
            mReader = new BinaryReader(mBytes);
            mWriter = new BinaryWriter(mBytes);
            mUseNetworkOrder = useNetworkOrder;

            if (pos >= mBytes.Length)
            {
                throw  new IndexOutOfRangeException("输入的pos值超过了内容的实际长度");
            }
            mBytes.Position = pos;

            mArrItemCoderMap = new Dictionary<Type, IArrItemCoder>();
            mArrItemCoderMap[typeof(MessageBase)] = new MsgBaseCoder();
            mArrItemCoderMap[typeof(UInt32)] = new U32Coder();
            mArrItemCoderMap[typeof(Int32)] = new I32Coder();
        }

        public MemoryStream GetStream()
        {
            return mBytes;
        }

        public long Position
        {
            get { return mBytes.Position; }
            set { mBytes.Position = value; }
        }

        public long Length
        {
            get { return mBytes.Length; }
        }

        public void SetLength(long length)
        {
            mBytes.SetLength(length);
        }

        public void Reset()
        {
            mBytes.Position = 0;
            mBytes.SetLength(0);
        }


        /// <summary>
        /// 得到可读的数据长度
        /// </summary>
        public long BytesAvailable
        {
            get { return mBytes.Length - mBytes.Position; }
        }

        /// <summary>
        /// 检查某个长度是否超出
        /// </summary>
        /// <param name="len"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        private bool CheckEndOf(UInt16 len, bool isThrow = true)
        {
            bool isEndof = len > BytesAvailable;
            if (isEndof && isThrow)
            {
                throw new EndOfStreamException(string.Format("没有足够的长度({0})来读取字符串", len));
            }

            return isEndof;
        }

        public void ReadByte(ref byte value)
        {
            value = mReader.ReadByte();
        }

        public void ReadI16(ref short value)
        {
            value = mReader.ReadInt16();
            if (mUseNetworkOrder)
            {
                value = IPAddress.NetworkToHostOrder(value);  
            }
        }
        
        public void ReadU16(ref UInt16 value)
        {
            value = mReader.ReadUInt16();
            if (mUseNetworkOrder)
            {
                value = (UInt16)IPAddress.NetworkToHostOrder((Int16)value); 
            }
        }

        public void ReadI32(ref Int32 value)
        {
            value = mReader.ReadInt32();
            if (mUseNetworkOrder)
            {
                value = IPAddress.NetworkToHostOrder(value);
            }
        }

        public void ReadU32(ref UInt32 value)
        {
            value = mReader.ReadUInt32();
            if (mUseNetworkOrder)
            {
                value = (UInt32) IPAddress.NetworkToHostOrder((Int32) value);
            }
        }

        public void ReadI64(ref Int64 value)
        {
            value = mReader.ReadInt64();
            if (mUseNetworkOrder)
            {
                value = IPAddress.NetworkToHostOrder(value);
            }
        }

        public void ReadU64(ref UInt64 value)
        {
            value = mReader.ReadUInt64();
            if (mUseNetworkOrder)
            {
                value = (UInt64) IPAddress.NetworkToHostOrder((Int64) value);
            }
        }

        public void ReadFloat(ref float value)
        {
            if (FLOAT_PREC_B > 0)
            {
                Int32 tmp = 0;
                ReadI32(ref tmp);
                value = tmp * FLOAT_PREC_B;
            }
            else
            { 
                value = mReader.ReadSingle();
            }
        }

        public void ReadDouble(ref double value)
        {
            if (DOUBLE_PREC_B > 0)
            {
                Int64 tmp = 0;
                ReadI64(ref tmp);
                value = tmp * DOUBLE_PREC_B;
            }
            else
            {
                value = mReader.ReadDouble();
            }
        }

        public void ReadStr(ref string value, int len = -1)
        {
            if (len == 0)
            {
                value = "";
                return;
            }

            //由于下面这个方法的字符串长度表示只有7位，不符合我们的要求
            //mReader.ReadString()

            //需要自己读前面的16字节为字符长度
            if (len < 0)
            {
                UInt16 tmeLen = 0;
                ReadU16(ref tmeLen);
                if (tmeLen == 0)
                {
                    value = "";
                    return;
                }
                len = tmeLen;
            }

            CheckEndOf((ushort)len);
            byte[] strBytes = mReader.ReadBytes(len);
            value = Encoding.UTF8.GetString(strBytes);
        }

        /// <summary>
        /// 读取字节串， 默认读取全部
        /// </summary>
        /// <param name="length">要读取的长度，小等于零则读取全部</param>
        /// <returns></returns>
        public byte[] ReadBytes(int length = -1)
        {
            if (length <= 0)
            {
                length = (int)BytesAvailable;
            }
            CheckEndOf((ushort)length);
            return mReader.ReadBytes(length);
        }

        public void ReadArray<T>(out List<T> value) where T : new()
        {
            Type findType = typeof (T);
            if (findType.IsSubclassOf(typeof (MessageBase)))
            {
                findType = typeof (MessageBase);
            }

            bool hasKey = mArrItemCoderMap.ContainsKey(findType);
            if (!hasKey)
            {
                throw  new Exception("没有找到对应的解码器：" + findType);
            }

            IArrItemCoder coder = mArrItemCoderMap[findType];
            UInt32 len = 0;
            ReadU32(ref len);
            value = new List<T>((int)len);
            for (int i = 0; i < len; i++)
            {
                object item;
                coder.Decode<T>(this,out item);
                value.Add((T)item);
            }
        }

        public void WriteByte(byte value)
        {
            mWriter.Write(value);
        }

        public void WriteI16(Int16 value)
        {
            if (mUseNetworkOrder)
            {
                value = IPAddress.HostToNetworkOrder(value);
            }
            mWriter.Write(value);
        }

        public void WriteU16(UInt16 value)
        {
            if (mUseNetworkOrder)
            {
                value = (UInt16)IPAddress.HostToNetworkOrder((Int16)value);
            }
            mWriter.Write(value);
        }

        public void WriteI32(Int32 value)
        {
            if (mUseNetworkOrder)
            {
                value = IPAddress.HostToNetworkOrder(value);
            }
            mWriter.Write(value);
        }

        public void WriteU32(UInt32 value)
        {
            if (mUseNetworkOrder)
            {
                //输入的参数必需是int32才不会出错
                value = (UInt32)IPAddress.HostToNetworkOrder((Int32)value);
            }
            mWriter.Write(value);
        }

        public void WriteI64(Int64 value)
        {
            if (mUseNetworkOrder)
            {
                value = IPAddress.HostToNetworkOrder(value);
            }
            mWriter.Write(value);
        }

        public void WriteU64(UInt64 value)
        {
            if (mUseNetworkOrder)
            {
                value = (UInt64)IPAddress.HostToNetworkOrder((Int64)value);
            }
            mWriter.Write(value);
        }

        public void WriteFloat(float value)
        {
            if (FLOAT_PREC > 0)
            {
                Int32 tmp = (Int32) (value*FLOAT_PREC);
                WriteI32(tmp);
            }
            else
            {
                mWriter.Write(value);
            }
        }

        public void WriteDouble(double value)
        {
            if (DOUBLE_PREC > 0)
            {
                Int64 tmp = (Int64)(value * DOUBLE_PREC);
                WriteI64(tmp);
            }
            else
            {
                mWriter.Write(value);
            }
        }

        public void WriteStr(string value, int len = -1)
        {
            if (len == 0)
            {
                return;
            }

            if (len < 0)
            {
                byte[] strBytes = Encoding.UTF8.GetBytes(value);
                WriteU16((UInt16)strBytes.Length);
                mWriter.Write(strBytes);
                return;
            }

            byte[] strBytes2 = new byte[len];
            Encoding.UTF8.GetBytes(value, 0, len, strBytes2, 0);
            mWriter.Write(strBytes2);
        }

        public void WriteBytes(byte[] value, bool hasWriteLen = false)
        {
            if (value == null)
            {
                return;
            }

            if (hasWriteLen)
            {
                WriteU16((UInt16)value.Length);
            }

            if (value.Length == 0)
            {
                return;
            }

            mWriter.Write(value);
        }

        public void WriteArray<T>(List<T> value)
        {
            if (value == null)
            {
                return;
            }

            Type findType = typeof(T);
            if (findType.IsSubclassOf(typeof(MessageBase)))
            {
                findType = typeof(MessageBase);
            }

            bool hasKey = mArrItemCoderMap.ContainsKey(findType);
            if (!hasKey)
            {
                throw new Exception("没有找到对应的解码器：" + findType);
            }

            IArrItemCoder coder = mArrItemCoderMap[findType];
            UInt32 len = (UInt32)value.Count;
            WriteU32(len);
            for (int i = 0; i < len; i++)
            {
                object valueEx = value[i];
                if (valueEx == null)
                {
                    throw new NullReferenceException("用于编码的列表数据项不能为空");
                }
                coder.Code(this, ref valueEx);
            }
        }

        public byte[] Dump()
        {
            return mBytes.ToArray();
        }

        protected override void OnDispose()
        {
            mBytes.Close();
            mReader.Close();
            mWriter.Close();

            mBytes = null;
            mReader = null;
            mWriter = null;
        }


    }
}

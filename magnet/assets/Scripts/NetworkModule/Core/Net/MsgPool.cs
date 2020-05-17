using System;
using System.Collections.Generic;
using Google.Protobuf;
using Log;
using Sw;

namespace Core.Net
{
    public class MsgPool:IMsgPool
    {
        private ILogger mLogger;
        public MsgPool(ILogger logger = null)
        {
            if (logger == null)
            {
                mLogger = new DefaultLogger("MsgPool");
            }
            else
            {
                mLogger = logger;
            }
        }

       /// <summary>
       /// 跟据消息ID，查找消息
       /// </summary>
       /// <param name="msgId"></param>
       /// <returns></returns>
        public IMessage GetMsgInst(uint msgId)
        {
            Type msgType;
            MsgDicS2C.TryGetValue(msgId, out msgType);
            if (msgType == null)
            {
                return null;
            }

            //TODO:后期做一个池子优化一下，但要考虑使用时由因逻辑已经放回池子，但实际还在使用而产生的BUG问题。
            IMessage newMsg = Activator.CreateInstance(msgType) as IMessage;
            if (newMsg == null)
            {
                mLogger.LogError("(newMsg == null", "FindMsg");
            }
            return newMsg;
        }

        /// <summary>
        /// 跟据消息类型，得到消息ID
        /// </summary>
        /// <param name="msgType"></param>
        /// <returns></returns>
        public uint GetMsgId(Type msgType)
        {
            if (!MsgType2Id.ContainsKey(msgType))
            {
                return 0;
            }

            return MsgType2Id[msgType];
        }


        /// <summary>
        /// 跟据消息类型，得到消息ID
        /// </summary>
        /// <returns></returns>
        public uint GetMsgId<T>() where T : IMessage
        {
            return GetMsgId(typeof (T));
        }



        private static readonly Dictionary<uint, Type> MsgDicS2C = new Dictionary<uint, Type>();
        private static readonly Dictionary<Type, uint> MsgType2Id = new Dictionary<Type, uint>();
        public static void RegistMsg<T>(uint msgId) where T:IMessage
        {
            Type msgType = typeof (T);
            MsgDicS2C[msgId] = msgType;
            MsgType2Id[msgType] = msgId;
        }

        /// <summary>
        /// 注册服务器端发给客户端的消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public void RegistS2CMsg<T>(MSG_DEFINE msgId) where T : IMessage
        {
            RegistMsg<T>((uint)msgId);
        }
    }
}

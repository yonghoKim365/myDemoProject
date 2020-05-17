using Google.Protobuf;
using System;

namespace Core.Net
{
    /// <summary>
    /// 消息池
    /// @author fanflash.org
    /// </summary>
    public interface IMsgPool
    {
        /// <summary>
        /// 跟据消息ID得到消息实例
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        IMessage GetMsgInst(uint msgId);

        /// <summary>
        /// 得到消息的ID
        /// </summary>
        /// <param name="msgType"></param>
        /// <returns></returns>
        uint GetMsgId(Type msgType);
    }
}

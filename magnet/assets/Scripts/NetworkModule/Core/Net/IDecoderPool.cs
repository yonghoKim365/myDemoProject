using System;

namespace Core.Net
{
    /// <summary>
    /// Message decoder pool
    /// @author fanflash.org
    /// </summary>
    public interface IDecoderPool
    {
        /// <summary>
        /// 跟据消息ID找消息解码器
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="isS2C"></param>
        /// <returns></returns>
        MsgDecoder FindDecoder(uint msgId);

        /// <summary>
        /// 跟据消息Builder的类型，得到消息ID
        /// </summary>
        /// <param name="msgBuilder"></param>
        /// <returns></returns>
        uint GetMsgId(Type msgBuilder);
    }
}

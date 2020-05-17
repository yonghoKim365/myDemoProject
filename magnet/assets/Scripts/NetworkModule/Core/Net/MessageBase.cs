using Core.IO;

namespace Core.Net
{
    /// <summary>
    /// 消息包基类
    /// @author fanflash.org
    /// </summary>
    public class MessageBase
    {   
        /// <summary>
        /// 返回消息ID
        /// </summary>
        /// <returns></returns>
        public virtual uint GetMsgId()
        {
            return 0;
        }

        /// <summary>
        /// 解消息包
        /// </summary>
        /// <param name="reader"></param>
        public virtual void UnPackMsg(IByteArrayReader reader)
        {
            
        }

        /// <summary>
        /// 写入消息包
        /// </summary>
        /// <param name="writer"></param>
        public virtual void PackMsg(IByteArrayWriter writer)
        {
           
        }
    }
}
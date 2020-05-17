using Core.Module;

namespace Core.Net
{
    public struct ResponseVo
    {
        /// <summary>
        /// s2c的消息
        /// </summary>
        public object Msg;

        /// <summary>
        /// 请求时附加的数据
        /// </summary>
        public object AttData;

        /// <summary>
        /// 错误ID号
        /// 参考ReqErrConst
        /// </summary>
        public int ErrorId;

        /// <summary>
        /// callback
        /// </summary>
        public CallbackVo CallbackVo;

        /// <summary>
        /// 是否超时
        /// </summary>
        public bool Timeout
        {
            get { return ErrorId == ReqErrConst.Timeout; }
        }
    }
}

using System;
using Core.Cache;
using Core.Module;
using Log;

namespace Core.Net
{

    /// <summary>
    /// 每次Request 对象的包装
    /// </summary>
    class RequestVo : CacheItem, ICacheItem<RequestVo>
    {
        private static uint mCount = 0;

        private uint mId = mCount++;
        private ILogger mLogger;
        private Network mNetwork;
        private Action<CallbackVo> mCallback;
        private int mResponseMsgId;

        public RequestVo()
        {

        }

        public string Name { get; private set; }

        protected override void OnInit(object[] initParams)
        {
            mNetwork = initParams[0] as Network;
            mLogger = initParams[1] as ILogger;
        }

        internal void SetData(string name, int responseMsgId, Action<CallbackVo> callback, float timeout = -1)
        {
            Name = name;
            liveTime = timeout;
            mResponseMsgId = responseMsgId;
            mCallback = callback;
            mNetwork.AddMsgListener(responseMsgId, MsgHandler, 0, true);
        }

        private void MsgHandler(CallbackVo callbackVo)
        {
            DoCallback(callbackVo);
            mResponseMsgId = -1;
            Return();
        }


        protected override void OnReturn()
        {
            if (mResponseMsgId > -1)
            {
                //说明超时，被自已回收了
                mNetwork.RemoveMsgListener(mResponseMsgId, MsgHandler);
                DoCallback(null);
            }

            mCallback = null;
        }


        protected void DoCallback(CallbackVo callbackVo)
        {
            if (mCallback == null)
            {
                mLogger.LogError("收到消息回调为空,response=" + mResponseMsgId + ", requestID=" + mId);
            }
            else
            {
                try
                {
                    mCallback(callbackVo);
                }
                catch (Exception error)
                {
                    mLogger.LogError("消息回调错误:" + error);
                }

            }
        }
    }
}

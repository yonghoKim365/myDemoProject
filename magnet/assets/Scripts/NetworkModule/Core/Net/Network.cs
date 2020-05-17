using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Cache;
using Core.Module;
using Google.Protobuf;
using Log;
using UnityEngine;
using Sw;

namespace Core.Net
{
    public enum ConnectState
    {
        None,
        Success,
        Error,
        Close
    }

    /// <summary>
    /// 网络管理类
    /// @author fanflash.org
    /// </summary>
    public class Network :Disposer
    {
        public const int BUFFER_SIZE = 4096;


        /// <summary>
        /// 默认的超时时间
        /// default timeout
        /// </summary>
#if UNITY_EDITOR
        public const int DefaultTimeout = int.MaxValue;
#else
        public const int DefaultTimeout = 1;
#endif

        /// <summary>
        /// 连接相关事件
        /// param1: this object
        /// param2: connect state
        /// </summary>
        public event Action<Network, ConnectState> EventConnect; 

        private NetworkCore mNetworkCore;
        private readonly ILogger mLogger;
        private BaseDispatcher mMsgDispatcher;
        private readonly CachePool<RequestVo> mRequestVoPool;
        private readonly MsgPool mMsgPool;
        private readonly Dictionary<MethodInfo, Action<CallbackVo>> mMsgListenerMap;

        public Network(MsgPool msgPool, string name = "Netwrok")
        {
            Name = name;
            mLogger = new DefaultLogger(name);
            mMsgPool = msgPool;
            mNetworkCore = new NetworkCore(BUFFER_SIZE, mMsgPool);
            mMsgDispatcher = new BaseDispatcher("NetworkMgr");
            mRequestVoPool = new CachePool<RequestVo>(1,0, new object[]{this, mLogger});
            mMsgListenerMap = new Dictionary<MethodInfo, Action<CallbackVo>>();
            mNetworkCore.SetCallback(HandleConnect, revMsgHandler);
        }

        protected void revMsgHandler(IMessage msg)
        {
            uint msgId = mMsgPool.GetMsgId(msg.GetType());
            mMsgDispatcher.DispatchEvent(msgId.ToString(), msg);
        }

        public string Name { get; private set; }
        
        /// <summary>
        /// 是否已经连接
        /// </summary>
        public bool Connected
        {
            get
            {
                return mNetworkCore.State == NetworkCore.NetworkState.Connected;
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string ip, int port)
        {
            mNetworkCore.Connect(ip, port);
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Close()
        {
            mNetworkCore.Close();
        }

        /// <summary>
        /// 监听消息
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="listener"></param>
        /// <param name="level"></param>
        /// <param name="autoRemove"></param>
        /// <param name="cancelable"></param>
        public void AddMsgListener(int msgId, Action<CallbackVo> listener, uint level = 0, bool autoRemove = false, bool cancelable = false)
        {
            mMsgDispatcher.AddEventListener(msgId.ToString(), listener, level, autoRemove, cancelable);
        }

        /// <summary>
        /// 监听消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <param name="level"></param>
        /// <param name="autoRemove"></param>
        /// <param name="cancelable"></param>
        /// <returns></returns>
        public bool AddMsgListener<T>(Action<CallbackVo> listener, uint level = 0, bool autoRemove = false, bool cancelable = false)where T:IMessage
        {
            uint msgId = mMsgPool.GetMsgId(typeof(T));
            if (msgId == 0)
            {
                mLogger.LogError("找不到类型" + typeof(T).FullName + "的对应消息ID", "AddMsgListener<T>");
                return false;
            } 

            mMsgDispatcher.AddEventListener(msgId.ToString(), listener, level, autoRemove, cancelable);
            return true;
        }

        /// <summary>
        /// 监听消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <param name="level"></param>
        /// <param name="autoRemove"></param>
        /// <param name="cancelable"></param>
        /// <returns></returns>
        public bool AddMsgListener<T>(Action<T> listener, uint level = 0, bool autoRemove = false,
            bool cancelable = false) where T : IMessage
        {
            uint msgId = mMsgPool.GetMsgId(typeof(T));
            if (msgId == 0)
            {
                mLogger.LogError("找不到类型" + typeof(T).FullName + "的对应消息ID", "AddMsgListener<T>");
                return false;
            }

            Action<CallbackVo> callback = vo =>
            {
                if (vo.Data == null)
                {
                    mLogger.LogError("收到的" + typeof(T).FullName + "消息为空", "AddMsgListener<T>");
                    return;
                }

                T revMsg = (T)vo.Data;
                if (revMsg == null)
                {
                    mLogger.LogError("收到的" + typeof(T).FullName + "消息转换不成功", "AddMsgListener<T>");
                    return;
                }

                listener(revMsg);
            };

            Action<CallbackVo> oldCallback;
            bool hasObj = mMsgListenerMap.TryGetValue(listener.Method, out oldCallback);

            //之前有监听过
            if (hasObj)
            {
                mMsgDispatcher.RevmoeEventListener(msgId.ToString(), oldCallback);
            }

            mMsgListenerMap[listener.Method] = callback;
            mMsgDispatcher.AddEventListener(msgId.ToString(), callback, level, autoRemove, cancelable);
            return true;
        }

        /// <summary>
        /// 移除消息监听
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool RemoveMsgListener(int msgId, Action<CallbackVo> listener)
        {
            return mMsgDispatcher.RevmoeEventListener(msgId.ToString(), listener);
        }

        /// <summary>
        /// 移除消息监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool RemoveMsgListener<T>(Action<CallbackVo> listener) where T : IMessage
        {
            uint msgId = mMsgPool.GetMsgId(typeof(T));
            if (msgId == 0)
            {
                mLogger.LogError("找不到类型" + typeof(T).FullName + "的对应消息ID", "RevmoeMsgListener<T>");
                return false;
            } 
            return mMsgDispatcher.RevmoeEventListener(msgId.ToString(), listener);
        }

        /// <summary>
        /// 移除消息监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool RemoveMsgListener<T>(Action<T> listener) where T : IMessage
        {
            uint msgId = mMsgPool.GetMsgId(typeof(T));
            if (msgId == 0)
            {
                mLogger.LogError("找不到类型" + typeof(T).FullName + "的对应消息ID", "RevmoeMsgListener<T>");
                return false;
            }

            Action<CallbackVo> callback;
            bool success = mMsgListenerMap.TryGetValue(listener.Method, out callback);
            if (!success)
            {
                mLogger.LogError("找不到对应的处理函数(" + typeof(T).FullName + ")");
                return false;
            }

            mMsgListenerMap.Remove(listener.Method);
            return mMsgDispatcher.RevmoeEventListener(msgId.ToString(), callback);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        //public bool SendMsg(IBuilderLite msg)
        //{
        //    return mNetworkCore.Send(msg);
        //}

        public bool SendMsg(MSG_DEFINE msgID, IMessage msg)
        {
            return mNetworkCore.Send((uint)msgID, msg);
        }

        /// <summary>
        /// 请求消息
        /// </summary>
        /// <param name="sendMsg"> 要发送的消息</param>
        /// <param name="responseMsgId">回应消息的消息号</param>
        /// <param name="callback">回调</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public bool Request(IMessage sendMsg, int responseMsgId, Action<CallbackVo> callback, float timeout = DefaultTimeout)
        {
            bool success = mNetworkCore.Send(sendMsg);
            if (!success)
            {
                return false;
            }

            if (callback == null)
            {
                return false;
            }

            RequestVo item = mRequestVoPool.Get(timeout);
            string name = "C2S " + sendMsg + " --> S2C" + responseMsgId;
            item.SetData(name, responseMsgId, callback, timeout);
            return true;
        }

        /// <summary>
        /// 请求消息
        /// </summary>
        /// <typeparam name="T">需要收到消息的消息类型</typeparam>
        /// <param name="sendMsg">要发送的消息</param>
        /// <param name="callback">消息回调</param>
        /// <param name="attData">附加的数据</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public bool Request<T>(IMessage sendMsg, Action<T, ResponseVo> callback, object attData = null, float timeout = DefaultTimeout) where T : IMessage
        {
            uint msgId = mMsgPool.GetMsgId(typeof(T));
            if (msgId == 0)
            {
                mLogger.LogError("找不到类型" + typeof(T).FullName + "的对应消息ID", "Request<T>");
                return false;
            }

            return Request(sendMsg, (int) msgId, callbackVo =>
            {
                ResponseVo responseVo = new ResponseVo();
                responseVo.AttData = attData;

                if (callbackVo == null)
                {
                    responseVo.ErrorId = ReqErrConst.Timeout;
                    callback(default(T), responseVo);
                    return;
                }

                responseVo.CallbackVo = callbackVo;

                T revMsg = (T)callbackVo.Data;
                if (revMsg == null)
                {
                    responseVo.ErrorId = ReqErrConst.MsgTranErr;
                    callback(default(T), responseVo);
                    return;
                }

                responseVo.Msg = callbackVo.Data;
                responseVo.ErrorId = ReqErrConst.None;
                callback(revMsg, responseVo);
            }, timeout);
        }

        /// <summary>
        /// 请求消息
        /// </summary>
        /// <typeparam name="T">接收的消息</typeparam>
        /// <param name="sendMsg">发送的消息</param>
        /// <param name="callback">
        /// 参数一为返回的消息
        /// 参数二
        /// 值小等于0: 没有错误
        /// 值 = 1：一般是超时
        /// 值 = 2：类型转换错误
        /// </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [Obsolete("This method is obsolete，please use Request<T>(MessageBase sendMsg, Action<T, ResponseVo> callback, object attData = null, float timeout = 1)")]
        public bool Request<T>(IMessage sendMsg, Action<T, int> callback, float timeout = DefaultTimeout) where T : IMessage
        {
            uint msgId = mMsgPool.GetMsgId(typeof(T));
            if (msgId == 0)
            {
                mLogger.LogError("找不到类型" + typeof(T).FullName + "的对应消息ID", "Request<T>");
                return false;
            }

            return Request(sendMsg, (int)msgId, callbackVo =>
            {
                if (callbackVo == null)
                {
                    callback(default(T), ReqErrConst.Timeout);
                    return;
                }

                T revMsg = (T)callbackVo.Data;
                if (revMsg == null)
                {
                    callback(default(T), ReqErrConst.MsgTranErr);
                    return;
                }

                callback(revMsg, ReqErrConst.None);
            }, timeout);
        }

        private float mNextCheckingTime;

        public void Update()
        {
            mNetworkCore.Update();
            CheckingRequestTimeout();
        }

        /// <summary>
        /// 清空所有的Request
        /// 主要是在断开连接时调用 
        /// </summary>
        private void ClearRequest()
        {
            mLogger.LogInfo("清空所有Request", "ClearRequest");
            while (true)
            {
                bool hasItem = false;
                foreach (CacheItemInfo<RequestVo> item in mRequestVoPool)
                {
                    hasItem = true;
                    item.Item.Return();
                    break;
                }

                if (!hasItem)
                {
                    break;
                }
            }

        }

        private void CheckingRequestTimeout()
        {
            var curTime = Time.realtimeSinceStartup;
            if (curTime > mNextCheckingTime)
            {
                bool hasError = false;
                //处理request回调超时
                foreach (CacheItemInfo<RequestVo> item in mRequestVoPool)
                {
                    //没有设置超时时间
                    if (item.Item.liveTime <= 0)
                    {
                        continue;
                    }

                    //已经超时
                    if (curTime > item.Timeout)
                    {
                        //删除一个就退，不然foreach内的index可能错乱
                        item.Item.Return();
                        mLogger.LogError(item.Item.Name + "已经超时，自动清除回调", "Update");
                        hasError = true;
                        break;
                    }
                }

                if (hasError)
                {
                    //立即
                    mNextCheckingTime = curTime - 1;
                }
                else
                {
                    //一秒检查一次
                    mNextCheckingTime = curTime + 1f;
                }
            }
        } 

        private void HandleConnect(string type)
        {
            if (type == "close")
            {
                ClearRequest();
            }

            var handler = EventConnect;
            if (handler == null)
            {
                return;
            }

            ConnectState state;
            switch(type)
            {
                case "success":
                    state = ConnectState.Success;
                    break;
                case "error":
                    state = ConnectState.Error;
                    break;
                case "close":
                    state = ConnectState.Close;
                    break;
                default:
                    state = ConnectState.None;
                    break;
            }

            handler(this,state);
        }

        protected override void OnDispose()
        {
            mLogger.LogDebug("网络模块已经清空", "OnDispose");
            if (mNetworkCore != null)
            {
                mNetworkCore.Dispose();
                mNetworkCore = null;
            }

            if (mMsgDispatcher != null)
            {
                mMsgDispatcher.Dispose();
                mMsgDispatcher = null;
            }

            mMsgListenerMap.Clear();
        }
    }
}

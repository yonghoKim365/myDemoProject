using System;
using System.IO;
using Assets.Script.Core.Thread;
using Google.Protobuf;
using Log;
using UnityEngine;
using ByteArray = Core.IO.ByteArray;

namespace Core.Net
{
    /// <summary>
    /// 网络模块核心
    /// @author fanflash.org
    /// </summary>
    public class NetworkCore:Disposer
    {
        /// <summary>
        /// 是否记录dump信息
        /// </summary>
        public static bool RecodeDump = false;

        /// <summary>
        /// 记录dump文件的目录
        /// </summary>
        public static string SaveDumpPath = "C:/NetworkDump";


        public enum NetworkState
        {
            Closed,
            ConnectError,
            Connecting,
            Connected,
        }

        private NetworkState mState;
        private int mBufferSize;
        private NetworkThread mNetwork;
        private IMsgPool mMsgPool;
        private ILogger mLogger;

        private Action<string> mConnectCallback;
        private Action<IMessage> mRevMsgCallback;
        private Action<ActorCmd>[] mHandler; 

        public NetworkCore(int bufferSize, IMsgPool msgPool = null, ILogger logger = null)
        {
            if (logger == null)
            {
                logger = new DefaultLogger("Netwrok");
            }

            mState = NetworkState.Closed;
            mLogger = logger;
            mBufferSize = bufferSize;
            mMsgPool = msgPool;

            mHandler = new Action<ActorCmd>[NetworkOutCmd.Count];
            mHandler[NetworkOutCmd.Connected] = ConnectedHandler;
            mHandler[NetworkOutCmd.ConnectError] = ConnErrHandler;
            mHandler[NetworkOutCmd.Close] = CloseHandler;
            mHandler[NetworkOutCmd.Message] = MessageHandler;
            mHandler[NetworkOutCmd.Dump] = DumpHandler;
        }

        public NetworkState State
        {
            get { return mState; }
        }

        /// <summary>
        /// 设置事件回调
        /// </summary>
        /// <param name="connectHandler"></param>
        /// <param name="revMsgHandler"></param>
        public void SetCallback(Action<string> connectHandler, Action<IMessage> revMsgHandler)
        {
            mConnectCallback = connectHandler;
            mRevMsgCallback = revMsgHandler;
        }

        public void Connect(string ip, int port)
        {
            if (mState == NetworkState.Connecting)
            {
                mLogger.LogInfo("mState = NetworkState.Connecting", "Network");
                return;
            }

            if (mNetwork != null)
            {
                mNetwork.Dispose();
            }

            if (RecodeDump)
            {
                try
                {
                    if (mDumpCount < 1 && Directory.Exists(SaveDumpPath))
                    {
                        Directory.Delete(SaveDumpPath,true);
                    }
                }
                catch (Exception e)
                {
                    mLogger.LogError("Network dump error: delete dump error, " + e, "mNetwork");
                }
            }

            mNetwork = new NetworkThread(mBufferSize,mMsgPool);
            mNetwork.RecodeDump = RecodeDump;
            mNetwork.SendBeforCallback = SendBeforCallback;
            ActorCmd connectCmd = new ActorCmd(NetworkInCmd.Connect)
            {
                Param0 = ip,
                Param1 = port
            };
            mNetwork.Input(connectCmd);
            mState = NetworkState.Connecting;
        }

        private void SendBeforCallback(uint msgId, ByteArray msg)
        {
            if (RecodeDump)
            {
                long p = msg.Position;
                int len = (int)msg.Length;
                msg.Position = 0;
                DumpSave(true,msgId,msg.ReadBytes(len));
                msg.Position = p;
            }
        }

        public bool Send(IMessage msg)
        {
            if (mState != NetworkState.Connected)
            {
                return false;
            }

            if (mNetwork == null)
            {
                mLogger.LogError("mNetwork = null", "mNetwork");
                return false;
            }

            return mNetwork.Send(msg);
        }

        public bool Send(uint msgId, IMessage msg)
        {
            if (mState != NetworkState.Connected)
            {
                return false;
            }

            if (mNetwork == null)
            {
                mLogger.LogError("mNetwork = null", "mNetwork");
                return false;
            }

			Debug.Log (" Send : " + msgId + ", msg:" + msg);
            return mNetwork.Send(msgId,msg);
        }

        public void Close()
        {
            if (mState == NetworkState.Closed)
            {
                return;
            }

            if (mNetwork != null)
            {
                mNetwork.Dispose();
            }
            SalfClose();
        }

        public void Update()
        {
            if (mNetwork == null)
            {
                return;
            }

            ActorCmd[] cmdList = mNetwork.Output();
            if (cmdList == null)
            {
                return;
            }

            foreach (ActorCmd cmd in cmdList)
            {
                if (cmd.Cmd < 0)
                {
                    mLogger.LogInfo("cmd.Cmd < 0, cmd = " + cmd.Cmd, "Update");
                    continue;
                }

                if (cmd.Cmd >= NetworkOutCmd.Count)
                {
                    mLogger.LogInfo("cmd.Cmd >= NetworkOutCmd.Count, cmd = " + cmd.Cmd, "Update");
                    continue;
                }

                mHandler[cmd.Cmd](cmd);
            }
        }

        protected void MessageHandler(ActorCmd cmd)
        {
            if (mState != NetworkState.Connected)
            {
                mLogger.LogError("mState != NetworkState.Connected", "MessageHandler");
                return;
            }

            if (mRevMsgCallback == null)
            {
                return;
            }

            IMessage msg = cmd.Param0 as IMessage;
            if (msg == null)
            {
                mLogger.LogError("msg == null", "MessageHandler");
                return;
            }

            try
            {
                mRevMsgCallback(msg);
            }
            catch (Exception e)
            {
                mLogger.LogError("Process message error:" + e, "MessageHandler");
            }
        }

        private static int mDumpCount;

        private void DumpHandler(ActorCmd cmd)
        {
            try
            {
                uint msgId = (uint)cmd.Param0;
                int msgLen = (int) cmd.Param1;
                byte[] msgData = (byte[]) cmd.Param2;
                if (msgLen != msgData.Length)
                {
                    Debug.LogWarning("msgLen != msgData.Length");
                    byte[] msgDataCopy = new byte[msgLen];
                    Array.Copy(msgData,msgDataCopy,msgLen);
                    msgData = msgDataCopy;
                }
                DumpSave(false, msgId, msgData);
            }
            catch (Exception e)
            {
                mLogger.LogError("Recode dump error: " + e, "TestRecodeDump");
            }
        }

        private void DumpSave(bool isSend, uint msgId, byte[] msgData)
        {
            if (msgData == null)
            {
                mLogger.LogError("Recode dump error: data is null", "TestRecodeDump");
                return;
            }

            try
            {
                string path = SaveDumpPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string flag = isSend ? "c2s" : "s2c";
                path = string.Format("{0}/{1}_{2}_{3}_{4}.bytes", path, mDumpCount, flag, msgId, msgData.Length);
                File.WriteAllBytes(path, msgData);
                mDumpCount++;
            }
            catch (Exception e)
            {
                mLogger.LogError("Recode dump error: " + e, "TestRecodeDump");
            }
        }

        protected void ConnectedHandler(ActorCmd obj)
        {
            if (mState != NetworkState.Connecting)
            {
                mLogger.LogError("mState != NetworkState.Connecting", "ConnectedHandler");
                return;
            }

            mState = NetworkState.Connected;
            if (mConnectCallback == null)
            {
                return;
            }

            try
            {
                mConnectCallback("success");
            }
            catch (Exception e)
            {
                mLogger.LogError("ConnectCallback(\"success\") error:" + e.Message, "ConnectedHandler");
            }
        }

        protected void ConnErrHandler(ActorCmd cmd)
        {
            if (mState != NetworkState.Connecting)
            {
                mLogger.LogError("mState != NetworkState.Connecting", "ConnErrHandler");
                return;
            }

            mState = NetworkState.ConnectError;
            if (mConnectCallback == null)
            {
                return;
            }

            try
            {
                mConnectCallback("error");
            }
            catch (Exception e)
            {
                mLogger.LogError("ConnectCallback(\"error\") error:" + e.Message, "ConnErrHandler");
            }
        }

        protected void CloseHandler(ActorCmd cmd)
        {
            if (mState == NetworkState.Closed)
            {
                mLogger.LogError("mState == NetworkState.Closed", "CloseHandler");
                return;
            }

            mState = NetworkState.Closed;
            if (mConnectCallback == null)
            {
                return;
            }

            try
            {
                mConnectCallback("close");
            }
            catch (Exception e)
            {
                mLogger.LogError("ConnectCallback(\"close\") error::" + e.Message, "CloseHandler");
            }
        }

        protected void SalfClose()
        {
            ActorCmd cmd = new ActorCmd(NetworkInCmd.Close);
            CloseHandler(cmd);
        }

        protected override void OnDispose()
        {
            mConnectCallback = null;
            mRevMsgCallback = null;

            if (mNetwork != null)
            {
                mNetwork.Dispose();
                mNetwork = null;
            }
        }
    }
}

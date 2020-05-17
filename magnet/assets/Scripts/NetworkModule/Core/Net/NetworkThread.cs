using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Assets.Script.Core.Thread;
using Core.IO;
using Google.Protobuf;
using Log;
using ByteArray = Core.IO.ByteArray;
using Sw;

namespace Core.Net
{
    /// <summary>
    /// 只对外开放一个接口
    /// 资源竞争关系非常清晰的socket类
    /// @author fanflash.org
    /// </summary>
    public class NetworkThread:ThreadActor
    {

        private bool MsgTransferLog = false;
        //包长度
        private const int HEAD_MSGLEN_SIZE = 2;
        //包cmd
        private const int HEAD_MSGCMD_SIZE = 2;
        //头总长度
        private const int HEAD_SIZE = HEAD_MSGLEN_SIZE + HEAD_MSGCMD_SIZE;

        enum NetState
        {
            None,
            Connecting,
            Connected,
            Close,
        }

        /// <summary>
        /// 是否记录发送和收到的数据包
        /// </summary>
        public bool RecodeDump = false;

        private NetState mState;
        private Socket mSocket;

        private int mBuffSize;
        private int mPosition;
        private byte[] mBuffer;
        private ByteArray mReader;
        
        private byte[] mWriterBuffer;
        private ByteArray mWriter;

        private int mNeedSize = HEAD_SIZE;
        private bool mIsHead = true;

        private IMsgPool mMsgPool;
        private Func<ActorCmd, ThreadResult>[] mHandler;

        public Action<uint,ByteArray> SendBeforCallback;

        public NetworkThread(int bufferSize, IMsgPool msgPool, ILogger logger = null)
            : base(100, logger, "NetworkThread")
        {
            mBuffSize = bufferSize;
            mMsgPool = msgPool;
            mHandler = new Func<ActorCmd, ThreadResult>[NetworkInCmd.Count];
            mHandler[NetworkInCmd.Close] = CloseHandler;
            mHandler[NetworkInCmd.Connect] = ConnectHandler;
        }

        public string GetMessageTypeString(uint msgid)
        {
            return ((MSG_DEFINE)msgid).ToString();
        }

        public bool Send(IMessage msg)
        {
            uint msgId = mMsgPool.GetMsgId(msg.GetType());
            return Send(msgId, msg);
        }

        /// <summary>
        /// 发送消息
        /// 在游戏线程调用
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Send(uint msgId, IMessage msg)
        {
            if (Disposed)
            {
                mLogger.LogError(this + "Been dispose", "Send");

                /////////////////////////////////////////////////////
                // 접속끊김
                UIMgr.instance.AddPopup(141, 109, 117, 0, 0, () =>
                {
                    SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () =>
                    {
                        UIMgr.ClearUI(true);
                        UITextList.ClearTextList();
                        //UIMgr.GetTownBasePanel().Close();
                        //UIMgr.instance.Clear(UIMgr.UIType.System);

                        //NetworkClient.instance.DisconnectGameServer();//연결 종료
                        NetData.instance.InitUserData();
                        NetData.instance.ClearCharIdc();

                        SceneManager.instance.ActionEvent(_ACTION.GO_LOGIN);
                    });
                });
                /////////////////////////////////////////////////////

                return false;
            }

            if (msg == null)
            {
                mLogger.LogError("msg == null", "Send");
                return false;
            }

            Socket socket = mSocket;
            if (socket == null)
            {
                mLogger.LogError("socket == null", "Send");
                return false;
            }

            try
            {
                if (!socket.Connected)
                {
                    mLogger.LogError("socket.Connected == false", "Send");
                    return false;
                }

                ByteArray writer = mWriter;
                if (writer == null)
                {
                    mWriterBuffer = new byte[mBuffSize];
                    mWriter = new ByteArray(mWriterBuffer);
                    writer = mWriter;
                }
                else
                {
                    writer.Reset();
                }

                UInt16 uMsgId = (UInt16)msgId;
                //先占位
                writer.WriteU16(0);
                writer.WriteU16(uMsgId);
                var ms = writer.GetStream();
                msg.WriteTo(ms);
                var len = writer.Position;

                //重写长度
                writer.Position = 0;
                writer.WriteU16((ushort)len);
                writer.Position = len;
                writer.SetLength(len);

                if (SendBeforCallback != null)
                {
                    SendBeforCallback(uMsgId, writer);
                }

                int sendSize = socket.Send(mWriterBuffer, (int)len, SocketFlags.None);

                if (MsgTransferLog)
                {
                    mLogger.LogInfo("send msg, msgId=" + msgId, "Send");
                }

                if (sendSize != writer.Length)
                {
                    mLogger.LogError("sendSize != mWriter.Length", "Send");
                    return false;
                }
            }
            catch (Exception e)
            {
                mLogger.LogError(e.Message, "Send");
                return false;
            }

            return true;
        }

        protected override bool OnInput(ActorCmd cmd)
        {
            if (cmd.Cmd == NetworkInCmd.Close)
            {
                CloseSocket(mSocket);
                return false;
            }

            return true;
        }

        protected override ThreadResult OnCmd(ActorCmd cmd)
        {
            if (cmd.Cmd < 0 || cmd.Cmd >= mHandler.Length)
            {
                mLogger.LogError("cmd.cmd < 0 || cmd.cmd >= mHandler.Length", "OnCmd");
                return ThreadResult.Sleep;
            }

            return mHandler[cmd.Cmd](cmd);
        }

        /// <summary>
        /// 处理连接
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected ThreadResult ConnectHandler(ActorCmd cmd)
        {
            if (mState == NetState.Connecting)
            {
                return ThreadResult.Sleep;
            }

            if (mState != NetState.None)
            {
                mLogger.LogError("mState != NetState.None, mState = " + mState, "ConnectHandler");
                return ThreadResult.Stop;
            }

            string ip = cmd.Param0.ToString();
            int port = (int) cmd.Param1;

            try
            {
                mState= NetState.Connecting;
                IPAddress serverIp;
                if (!IPAddress.TryParse(ip, out serverIp))
                {
                    IPAddress[] address = Dns.GetHostAddresses(ip);
                    if (address.Length < 1)
                    {
                        mLogger.LogError("ipHost.AddressList.Length < 1", "ConnectHandler");
                        return ThreadResult.Stop;
                    }
                    serverIp = address[0];
                }

                mSocket = new Socket(serverIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mSocket.Connect(serverIp,port);
                mLogger.LogInfo("Socket connect success", "ConnectHandler");
                if (mReader == null)
                {
                    mBuffer = new byte[mBuffSize];
                    mReader = new ByteArray(mBuffer);
                }
                else
                {
                    mReader.Reset();
                }

                //初始化读取数据的参数
                mNeedSize = HEAD_SIZE;
                mIsHead = true;
                mPosition = 0;

                mState = NetState.Connected;
                SendToOutput(new ActorCmd(NetworkOutCmd.Connected));
            }
            catch (SocketException e)
            {
                mState = NetState.None;
                mLogger.LogError(e.Message, "ConnectHandler");
                SendToOutput(new ActorCmd(NetworkOutCmd.ConnectError));
                //CloseSocket(mSocket);
                //return ThreadResult.Stop;
            }

            return ThreadResult.Sleep;
        }

        /// <summary>
        /// 处理关闭
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected ThreadResult CloseHandler(ActorCmd cmd)
        {
            //不会执行到这里，国为OnInput那里已经被截断了
            mThread.Dispose();
            return ThreadResult.Stop;
        }

        protected override ThreadResult OnUpdate(ThreadProxy thread)
        {
            if (mState == NetState.Close)
            {
                return ThreadResult.Stop;
            }

            if (mState != NetState.Connected)
            {
                return ThreadResult.Sleep;
            }

            Socket socket = mSocket;
            if (socket == null)
            {
                mLogger.LogError("socket == null", "OnUpdate");
                return ThreadResult.Stop;
            }

            if (!socket.Connected)
            {
                mLogger.LogError("socket.Connected = false", "OnUpdate");
                return ThreadResult.Stop;
            }

            try
            {
                //利用socket本身的buff包，先收头，再收尾，满一个包，再从头开始，如此循环
                //if (MsgTransferLog)
                //    mLogger.LogInfo("Start recv data, position =" + mPosition + ", NeedSize =" + mNeedSize, "OnUpdate");

                int allLen = mPosition + mNeedSize;
                if (allLen > mBuffSize)
                {
                    throw new Exception("allLen > mBuffSize，need = " + allLen + ", current = " + mBuffSize);
                }

                int recvLen = socket.Receive(mBuffer, mPosition, mNeedSize, SocketFlags.None);
                if (recvLen < 1)
                {
                    mLogger.LogInfo(
                        "recvLen =" + recvLen + 
                        ", socket.Connected = " + socket.Connected +
                        ", May be the server is close", "OnUpdate");

                    CloseSocket(socket);
                    return ThreadResult.ErrorStop;
                }
                
                mPosition += recvLen;
                if (recvLen < mNeedSize)
                {
                    mNeedSize -= recvLen;
                    mLogger.LogInfo("mPosition < mNeedSize, recv = " + recvLen + ",  need = " + mNeedSize, "OnUpdate");
                    //下一次循环再收
                    return ThreadResult.Just;
                }
            }
            catch (SocketException e)
            {
                mLogger.LogError("recv data error: " + e.Message, "OnUpdate");

                if((SocketError)e.ErrorCode == SocketError.ConnectionAborted)
                {
                    //서버에서 끊어진건데 클라이언트가 끊을때도 나옴

                    CloseSocket(socket);
                    return ThreadResult.ErrorStop;
                }

                mLogger.LogError("Socket Disconnect - " + String.Format("{0}({1})", e.ErrorCode, ((SocketError)e.ErrorCode).ToString()) );

                CloseSocket(socket);
                return ThreadResult.Stop;
            }

            //消息定义
            if (mIsHead)
            {
                Int16 msglen = 0;
                mReader.ReadI16(ref msglen);
                if (msglen < HEAD_SIZE)
                {
                    mLogger.LogError("msglen < HEAD_SIZE", "OnUpdate");
                    return ThreadResult.Stop;
                }

                mReader.SetLength(msglen);

                //剩下读消息主体
                mNeedSize = msglen - HEAD_SIZE;
                if (mNeedSize == 0)
                {
                    //只有消息头的资源
                    ProcessMsg(mReader, mPosition);
                    mIsHead = true;
                }
                else
                {
                    //因为还没有读完
                    mIsHead = false;
                }
            }
            else
            {
                ProcessMsg(mReader, mPosition);
                //消息主体读完
                mIsHead = true;
            }

            //说明已经读完了
            //有时一个消息就只有包头，内容长度为0
            if (mIsHead)
            {
                mPosition = 0;
                mReader.Position = 0;
                mNeedSize = HEAD_SIZE;
            }

            return ThreadResult.Just;
        }

        /// <summary>
        /// 处理消息 
        /// 在socket线程处理
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="msgLen"></param>
        private void ProcessMsg(IByteArrayReader reader, int msgLen)
        {
            UInt16 msgId = 0;
            reader.ReadU16(ref msgId);
            if (msgId < 1)
            {
                mLogger.LogError("msgID = " + msgId + ", msgID < 1", "ProcessMsg");
                return;
            }

            if (MsgTransferLog)
            {
                mLogger.LogInfo("rev msg, msgId=" + GetMessageTypeString(msgId), "ProcessMsg");
            }

            IMessage msg = mMsgPool.GetMsgInst(msgId);
            if (msg == null)
            {
                mLogger.LogWarning("MsgPool is not find the msg, msgId = " + msgId, "ProcessMsg");
                return;
            }

            if (RecodeDump)
            {
                ActorCmd dumpCmd = new ActorCmd(NetworkOutCmd.Dump)
                {
                    Param0 = (uint)msgId,
                    Param1 = msgLen,
                    Param2 = reader.Dump(),
                };
                SendToOutput(dumpCmd);
            }

            try
            {
                msg.MergeFrom(reader.GetStream());
            }
            catch (Exception e)
            {
                mLogger.LogError("msgID = " + msgId + ", MergeFrom error, " + e.Message, "ProcessMsg");
                return;
            }

            SendToOutput(new ActorCmd(NetworkOutCmd.Message, msg));
        }

        protected override void OnDispose()
        {
            CloseSocket(mSocket);
        }

        protected override void OnExit()
        {
            CloseSocket(mSocket);
            mState = NetState.Close;
            Disposed = true;

            mBuffSize = 0;
            mPosition = 0;

            mBuffer = null;
            if (mReader != null)
            {
                mReader.Dispose();
                mReader = null;
            }

            mWriterBuffer = null;
            if (mWriter != null)
            {
                mWriter.Dispose();
                mWriter = null; 
            }

            mNeedSize = HEAD_SIZE;
            mIsHead = true;

            mMsgPool = null;
            mHandler = null;
            SendBeforCallback = null;
        }

        private Boolean mSendClosedMsg;
        private void CloseSocket(Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    if (socket.Connected)
                    {
                        socket.Close(0);
                    }
                }
                catch (Exception e)
                {
                    mLogger.LogInfo("close socket error:" + e.Message, "CloseSocket");
                }
            }

            mState = NetState.Close;

            if (!mSendClosedMsg)
            {
                SendToOutput(new ActorCmd(NetworkOutCmd.Close));
                mSendClosedMsg = true;
            }
        }
    }

    public static class NetworkInCmd
    {
        public static int Close = 0;
        public static int Connect = 1;
        public static int Count = 2;
    }

    public static class NetworkOutCmd
    {
        public static int Connected = 0;
        public static int ConnectError = 1;
        public static int Close = 2;
        public static int Message = 3;
        public static int Dump = 4;
        public static int Count = 5;
    }
}

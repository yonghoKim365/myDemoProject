using System;
using System.Collections.Generic;
using Core;
using Log;
using UnityEngine;

namespace Assets.Script.Core.Thread
{
    /// <summary>
    /// 安全简易的类Actor模型 
    /// 只有input and output
    /// @author fanflash.org
    /// </summary>
    public abstract class ThreadActor:IDisposer
    {
        private static int ActorCount;

        private object mInputLock;
        private object mOutputLock;

        protected Queue<ActorCmd> mInput;
        protected Queue<ActorCmd> mOutput;
        protected ThreadProxy mThread;
        protected ILogger mLogger;

        public ThreadActor(int sleeptime = 100, ILogger logger = null, string name = null)
        {
            if (name == null)
            {
                name = "ThreadActor_" + ActorCount;
            }
            Name = name;
            ActorCount++;

            if (logger == null)
            {
                logger = new DefaultLogger(Name);
            }

            mLogger = logger;
            mInput = new Queue<ActorCmd>();
            mOutput = new Queue<ActorCmd>();
            mThread = new ThreadProxy(Update, ThreadExitHandler,sleeptime, logger);

            mInputLock = new object();
            mOutputLock = new object();
           
        }

        public string Name { get; protected set; }

        /// <summary>
        /// 输入数据
        /// </summary>
        /// <param name="cmd"></param>
        public void Input(ActorCmd cmd)
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(Name);
            }

            lock (mInput)
            {
                if (OnInput(cmd))
                {
                    mInput.Enqueue(cmd);
                }
            }
        }

        /// <summary>
        /// 内部使用的输入数据
        /// </summary>
        /// <param name="cmd"></param>
        protected void InsideInput(ActorCmd cmd)
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(Name);
            }

            lock (mInput)
            {
                mInput.Enqueue(cmd);  
            }
        }

        /// <summary>
        /// 输出数据
        /// </summary>
        /// <returns></returns>
        public ActorCmd[] Output()
        {
            if (Disposed)
            {
                return null;
            }

            Queue<ActorCmd> tmpOutput = mOutput;
            ActorCmd[] outputs;
            lock (mOutputLock)
            {
                if (tmpOutput.Count == 0)
                {
                    return null;
                }
                outputs = tmpOutput.ToArray();
                mOutput.Clear();
                return outputs;
            }
        }

        /// <summary>
        /// 内部使用的，发送数据
        /// </summary>
        /// <param name="cmd"></param>
        protected void SendToOutput(ActorCmd cmd)
        {
            lock (mOutputLock)
            {
                mOutput.Enqueue(cmd);
            }
        }

        protected ThreadResult Update(ThreadProxy thread)
        {
            ActorCmd[] cmdList = null;
            lock (mInputLock)
            {
                if (mInput.Count > 0)
                {
                    cmdList = mInput.ToArray();
                    mInput.Clear();
                }

            }

            if (cmdList != null)
            {
                foreach (ActorCmd cmd in cmdList)
                {
                    if (OnCmd(cmd) == ThreadResult.Stop)
                    {
                        mLogger.LogInfo("命令" + cmd.Cmd + "关闭了线程", "Update");
                        return ThreadResult.Stop;
                    }
                }
            }

            ThreadResult result = OnUpdate(thread);
            return result;
        }

        protected void ThreadExitHandler(ThreadProxy thread)
        {
            Disposed = true;
            OnExit();
            lock (mInputLock)
            {
                if (mInput != null)
                {
                    mInput.Clear();
                }
                mInput = null;
            }

            lock (mOutputLock)
            {
                if (mOutput != null)
                {
                    mOutput.Clear();
                }
                mOutput = null;
            }

            mThread = null;
            mInputLock = null;
            mOutputLock = null;
        }

        public bool Disposed { get; protected set; }

        public bool Dispose()
        {
            if (Disposed)
            {
                return false;
            }

            OnDispose();

            //TODO：有空研究一下这里为什么会等于空
            if (mThread == null)
            {
                try
                {
                    Debug.LogError("这里不应该等于空");
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
            else
            {
                mThread.Dispose(); 
            }
            
            Disposed = true;
            return true;
        }

        /// <summary>
        /// 对输入的数据进行预处理
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>如果返回false, 由不把这个命名添加到列表中去</returns>
        protected abstract bool OnInput(ActorCmd cmd);

        /// <summary>
        /// 当有Cmd进来时，会执行这个
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected abstract ThreadResult OnCmd(ActorCmd cmd);

        /// <summary>
        /// 当没有Cmd进来时，就会执行这个
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        protected abstract ThreadResult OnUpdate(ThreadProxy thread);

        /// <summary>
        /// 当调用Dispose时发生
        /// OnExit才是真正的退出
        /// </summary>
        protected abstract void OnDispose();

        /// <summary>
        /// 真正的退出
        /// </summary>
        protected abstract void OnExit();
    }

    public struct ActorCmd
    {
        public int Cmd;
        public object Param0;
        public object Param1;
        public object Param2;
        public object Param3;

        public ActorCmd(int cmd, object data = null)
        {
            Cmd = cmd;
            Param0 = data;
            Param1 = null;
            Param2 = null;
            Param3 = null;
        }

        public void Dispose()
        {
            Cmd = -1;
            Param0 = null;
            Param1 = null;
            Param2 = null;
            Param3 = null;
        }
    }
}

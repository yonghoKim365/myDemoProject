using System;
using System.Threading;
using Log;

namespace Core
{
    public enum ThreadResult
    {
        None,
        Sleep,
        Just,
        Stop,
        ErrorStop,
    }

    /// <summary>
    /// 线程代理
    /// @author fanflash.org
    /// </summary>
    public class ThreadProxy:Disposer
    {
        const bool Debug = false;

        private static int Count;
        private Thread mThread;
        private ILogger mLogger;
        private int mSleepTime;
        private Action<ThreadProxy> mExitCallback;

        /// <summary>
        /// 返回是否已经退出
        /// </summary>
        public Boolean Exit { get; private set; }

        public ThreadProxy(Func<ThreadProxy, ThreadResult> threadProc, Action<ThreadProxy> exitCallback, int sleepTime, ILogger logger = null)
        {
            string name = "TreadProxy_" + Count++;
            if (logger == null)
            {
                logger = new DefaultLogger(name);
            }
            mLogger = logger;
            mLogger.LogInfo("新建一个线程， name = " + name, "构造函数");

            Exit = false;
            mSleepTime = sleepTime;
            mThread = new Thread(Run)
            {
                Name = name
            };
            mThread.Start(threadProc);
            mExitCallback = exitCallback;
        }


        private void Run(object threadProc)
        {
            mLogger.LogInfo("============Start============");
            Func<ThreadProxy, ThreadResult> func = (Func<ThreadProxy, ThreadResult>) threadProc;
            if (func != null)
            {
                int curTime = DateTime.Now.Millisecond;
                int st = mSleepTime;

                while (!Disposed)
                {
                    ThreadResult result = ThreadResult.Stop;
                    try
                    {
                        if(Debug)mLogger.LogInfo("------------Thread fun statrt------------");
                        result = func(this);
                        if(Debug)mLogger.LogInfo("------------Thread fun end------------");
                    }
                    catch (Exception ex)
                    {
                        mLogger.LogError("call fun error: " + ex, "Run");
                        Dispose();
                        break;
                    }

                    switch (result)
                    {
                        case ThreadResult.Sleep:
                            var oldTime = curTime;
                            curTime = DateTime.Now.Millisecond;
                            int d = st - (curTime - oldTime);
                            if (d > 0)
                            {
                                Thread.Sleep(d);
                            }
                            break;

                        case ThreadResult.Just:
                            //马上执行
                            break;

                        case ThreadResult.ErrorStop:
                            mLogger.LogError("ErrorStop!!!");
                            TryDispose();
                            break;

                        default:
                            TryDispose(); 
                            break;
                    }
                }
            }
            else
            {
                TryDispose();
            }

            Exit = true;
            Action<ThreadProxy> callback = mExitCallback;
            if (callback != null)
            {
                try
                {
                    callback(this);
                }
                catch (Exception ex)
                {
                    mLogger.LogError("call ExitCallback error: " + ex, "TryDispose");
                }
                
            }
            mLogger.LogInfo("============End============");
            mLogger = null;
            mExitCallback = null;
        }

        protected bool TryDispose()
        {
            try
            {
                return Dispose();
            }
            catch (Exception ex)
            {
                mLogger.LogError("call Dispose error: " + ex, "Run");
                return false;
            }
        }

        protected override void OnDispose()
        {
            mLogger.LogInfo("Disponse", "OnDispose");
            mThread = null;
        }
    }
}

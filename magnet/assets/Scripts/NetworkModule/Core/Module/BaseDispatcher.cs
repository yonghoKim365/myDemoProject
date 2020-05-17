using System;
using System.Collections.Generic;
using Log;
using UnityEngine;

namespace Core.Module
{
    /// <summary>
    /// 事件派发器基类
    /// @antor fanflash.org
    /// </summary>
    public class BaseDispatcher:Disposer
    {
        protected Dictionary<string, List<CallbackVo>> mEventDic;
        protected Dictionary<string, bool> mDispatchDic;
 
        protected ILogger mLogger;
        protected PropModifier mModifier;

        public BaseDispatcher(string name, ILogger logger = null)
        {
            Name = name;
            mEventDic = new Dictionary<string, List<CallbackVo>>();
            mDispatchDic = new Dictionary<string, bool>();
            mModifier = new PropModifier();

            if (logger == null)
            {
                logger = new DefaultLogger("Dispatcher[" + name + "]");
            }
            mLogger = logger;
        }

        public string Name { protected set; get; }

        /// <summary>
        /// 增加监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <param name="level"></param>
        /// <param name="autoRemove"></param>
        /// <param name="cancelable"></param>
        /// <returns>是否成功</returns>
        public bool AddEventListener(string type, Action<CallbackVo> callback, uint level = 0, bool autoRemove = false, bool cancelable = false)
        {
            List<CallbackVo> eventList;
            mEventDic.TryGetValue(type, out eventList);
            if (eventList == null)
            {
                eventList = new List<CallbackVo>();
                mEventDic[type] = eventList;
            }

            foreach (CallbackVo callbackVo in eventList)
            {
                if (callbackVo.Callback == callback)
                {
                    mLogger.LogWarning("callbackVo.Callback == callback", "AddEventListener");
                    return false;
                }   
            }

            CallbackVo cv = new CallbackVo(
                type,
                callback,
                null,
                this,
                level,
                autoRemove,
                cancelable,
                mModifier);

            eventList.Add(cv);

            //如果是在派发中，则不排序，派发完会排序
            if (!mDispatchDic.ContainsKey(type))
            {
                eventList.Sort();
            }

            return true;
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public bool RevmoeEventListener(string type, Action<CallbackVo> callback)
        {
            bool isDispatch = mDispatchDic.ContainsKey(type);

            List<CallbackVo> eventList;
            mEventDic.TryGetValue(type, out eventList);
            if (eventList == null)
            {
                return false;
            }

            for (int i = 0; i < eventList.Count; i++)
            {
                CallbackVo callbackVo = eventList[i];
                if (callbackVo.Callback == callback)
                {
                    if (isDispatch)
                    {
                        //派发时只标记
                        callbackVo.MarkRemove();
                    }
                    else
                    {
                        eventList.RemoveAt(i);
                    }
                    break;
                }
            }

            //派发后本来就会做这个事情，所以只要处理非派发的情况就可以了
            if (!isDispatch)
            {
                if (eventList.Count == 0)
                {
                    mEventDic.Remove(type);
                }
            }

            return true;
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void DispatchEvent(string type, object data = null)
        {
            if (mDispatchDic.ContainsKey(type))
            {
                mLogger.LogError("不能在派发" + type + "事件时再派发" + type + "事件");
                return;
            }
            
            List<CallbackVo> eventList;
            mEventDic.TryGetValue(type, out eventList);
            if (eventList == null)
            {
                return;
            }

            //start dispatch
            //1. 在handler继续派发当前类型的消息, 处理方法：忽略并打错误LOG。
            //2. 在handler里增加handler，处理方法：加入事件处理尾部执行，并在处理完全部handler后排序（加入时并不排序）
            //3. 在handler里删除handler, 处理方法：标记handler已经被删除，并在处理完全部handler后真实删除
            mDispatchDic[type] = true;
            int beforCount = eventList.Count;
            for (int i = 0; i < eventList.Count; i++)
            {
                CallbackVo callbackVo = eventList[i];
                if (callbackVo.TobeRemove)
                {
                    continue;
                }

                if (data != null)
                {
                    mModifier.Set("Data", data);
                    callbackVo.Modify();
                }

                if (callbackVo.Callback != null)
                {
                    try
                    {
                        callbackVo.Callback(callbackVo);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message + ", " + ex.StackTrace);
                    }
                }

                if (callbackVo.AutoRemove)
                {
                    callbackVo.MarkRemove();
                }

                //取消了后续事件处理
                if (callbackVo.IsDefaultPrevented)
                {
                    mModifier.Set("IsDefaultPrevented", false);
                    callbackVo.Modify();
                    break;
                }
            }

            int afterCount = eventList.Count;

            //说明有增加handler，要重新排序
            bool needSort = beforCount != afterCount;

            //删除被标记移除的handler
            for (int i = afterCount -1; i >=0; i--)
            {
                CallbackVo callbackVo = eventList[i];
                if (callbackVo.TobeRemove)
                {
                    eventList.RemoveAt(i);
                }
            }

            //如果没有了，就全删除
            if (eventList.Count == 0)
            {
                mEventDic.Remove(type);
            }
            else
            {
                if (needSort)
                {
                    eventList.Sort();
                }
            }

            //end dispatch
            mDispatchDic.Remove(type);
        }

        protected override void OnDispose()
        {
            mEventDic.Clear();
            mEventDic = null;
            mDispatchDic.Clear();
            mDispatchDic = null;
            mModifier.Dispose();
            mModifier = null;
        }
    }
}

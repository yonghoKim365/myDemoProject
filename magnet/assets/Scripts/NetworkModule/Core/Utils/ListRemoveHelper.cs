using System.Collections;
using System.Collections.Generic;
using Core.Cache;
using Log;
using UnityEngine;

namespace Core.Utils
{
    /// <summary>
    /// 用来在一个循环遍历中删除项的工具
    /// @author fanflash.org
    /// </summary>
    public class ListRemoveHelper
    {
        private static ILogger mLogger;
        private static CachePool<RemoveItemList> mPool;

        static ListRemoveHelper()
        {
            mLogger = new DefaultLogger("ListRemoveHelper");
            mPool = new CachePool<RemoveItemList>(5, 0, new object[] { mLogger });
        }

        public static RemoveItemList Get(IList list)
        {
            if (list == null)
            {
                mLogger.LogError("list == null", "Get");
                return null;
            }

            RemoveItemList removeItemList = mPool.Get();
            removeItemList.Start(list);
            return removeItemList;
        }

        private static bool mStart = false;

        /// <summary>
        /// 开始检查是否有使用错误
        /// </summary>
        /// <param name="mb"></param>
        public static void StartChecking(MonoBehaviour mb)
        {
            if (mStart)
            {
                return;
            }
            mStart = true;
            mb.StartCoroutine(Update());
        }

        private static IEnumerator Update()
        {
            //等待时间
            const float waitTime = 1;
            const int onceCheckMax = 1000;

            int checkNum = 0;
            float curTime = Time.realtimeSinceStartup;
            while (true)
            {
                foreach (CacheItemInfo<RemoveItemList> item in mPool)
                {
                    checkNum++;
                    if (curTime > item.Timeout)
                    {
                        mLogger.LogError("删除列表的使用时间超时，list = " + item.Item, "Update");
                    }
                    if (checkNum >= onceCheckMax)
                    {
                        checkNum = 0;
                        yield return new WaitForSeconds(waitTime);
                    }
                }
            }
        }


        public class RemoveItemList:CacheItem, ICacheItem<RemoveItemList>
        {
            /// <summary>
            /// 每次的超时时间
            /// 单位：秒
            /// </summary>
            private const float OnceTimeout = 1;

            private IList mList;
            private List<object> mRemoveList;
            private ILogger mLogger; 

            public RemoveItemList()
            {
                mRemoveList = new List<object>();
                liveTime = OnceTimeout;
            }

            protected override void OnInit(object[] initParams)
            {
                mLogger = initParams[0] as ILogger;
            }

            public void Start(IList container)
            {
                mList = container;
                if (mRemoveList.Count > 0)
                {
                    mLogger.LogWarning("mRemoveList.Count > 0, list = " + mList, "Start");
                    mRemoveList.Clear();
                }
            }

            public void Add(object item)
            {
                mRemoveList.Add(item);
            }

            public void Remove()
            {
                if (mList == null)
                {
                    mLogger.LogError("mList == null, 是否是没有调用Start？", "Remove");
                    mRemoveList.Clear();
                    return;
                }

                foreach (object o in mRemoveList)
                {
                    mList.Remove(o);
                }

                mList = null;
                mRemoveList.Clear();
                Return();
            }

            protected override void OnReturn()
            {
                //通过这个条件来限制循环调用
                if (mList != null && mRemoveList.Count > 0)
                {
                    Remove();
                    return;
                }

                mRemoveList.Clear();
                mList = null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Log;
using UnityEngine;

namespace Core.Cache
{
    /// <summary>
    /// 需要实现子项接口的通用缓存池
    /// @author fanflash.org
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachePool<T> : IEnumerable, IEnumerator<CacheItemInfo<T>> where T : ICacheItem<T>, new()
    {
        /// <summary>
        /// 内部包装的对象，主要为了在不暴露接口的情况下，实现快速删除
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        class PoolItem<TItem>
        {
            public float Timeout;
            public int Index;
            public TItem Item;
        }

        private Stack<PoolItem<T>> mPool;
        private List<PoolItem<T>> mUsingList;
        private CacheItemInfo<T> mTmpInfo; 

        private object[] mInitParams;
        private ILogger mLogger;
        private int mMaxCount;

        public CachePool(int initCount = 0, int maxCount = 0, object[] initParams = null, ILogger logger = null)
        {
            if (logger == null)
            {
                logger = new DefaultLogger(ToString());
            }
            mLogger = logger;

            if (maxCount != 0)
            {
                if (initCount > maxCount)
                {
                    initCount = maxCount;
                    mLogger.LogWarning("initCount > maxCount， 设置的初始个数超过最大个数，已经把初始个数重置为最大个数", "构造函数");
                }
                mPool = new Stack<PoolItem<T>>(maxCount);
                mUsingList = new List<PoolItem<T>>(maxCount);
            }
            else
            {
                mPool = new Stack<PoolItem<T>>();
                mUsingList = new List<PoolItem<T>>();
            }

            mMaxCount = maxCount;
            mInitParams = initParams;



            mTmpInfo = new CacheItemInfo<T>();

            for (int i = 0; i < initCount; i++)
            {
                PoolItem<T> item = NewItem();
                mPool.Push(item);
            }
        }

        /// <summary>
        /// 返回一个实例
        /// 如果有设置maxCount的话，可能会返回空
        /// 如果反回家，表示可用的对象已经被全部取完
        /// </summary>
        /// <param name="timeout">
        /// 超时时间，单位秒
        /// 如果有设置这个值，将忽略item自身的liveTime设置
        /// </param>
        /// <returns></returns>
        public T Get(float timeout = -1)
        {
            PoolItem<T> item;
            if (mPool.Count == 0)
            {
                if (mMaxCount > 0 && mUsingList.Count >= mMaxCount)
                {
                    return default(T);
                }

                item = NewItem();
            }
            else
            {
                item = mPool.Pop();
            }

            if (timeout > 0)
            {
                item.Timeout = timeout + Time.realtimeSinceStartup;
            }
            else
            {
                item.Timeout = item.Item.liveTime + Time.realtimeSinceStartup;
            }
            
            AddToUsingList(item);
            return item.Item;
        }

        /// <summary>
        /// 新建一个对象
        /// </summary>
        /// <returns></returns>
        private PoolItem<T> NewItem()
        {
            PoolItem<T> pi = new PoolItem<T>();
            pi.Index = -1;
            pi.Item = new T();
            pi.Item.Initialize(() =>
            {
                RemoveFormUsingList(pi);
                pi.Index = -1;
                mPool.Push(pi);
            }, mInitParams);
            return pi;
        }

        /// <summary>
        /// 增加到使用列表
        /// </summary>
        /// <param name="item"></param>
        private void AddToUsingList(PoolItem<T> item)
        {
            if (item.Index != -1)
            {
                mLogger.LogError("啊~~稀吧， 这里怎么可能出现这种情况，item.Index", "Get");
            }

            item.Index = mUsingList.Count;
            mUsingList.Add(item);
        }

        /// <summary>
        /// 从使用列表移除
        /// </summary>
        /// <param name="item"></param>
        private void RemoveFormUsingList(PoolItem<T> item)
        {
            int errorCode = -1;

            if (item.Index >= mUsingList.Count)
            {
                mLogger.LogError("item.Index >= mUsingList.Count", "RemoveFormUsingList");
                errorCode = 1;
            }

            if (item.Index < 0)
            {
                mLogger.LogError("item.Index < 0", "RemoveFormUsingList");
                errorCode = 2;
            }

            PoolItem<T> itemEx = mUsingList[item.Index];
            if (itemEx != item)
            {
                mLogger.LogError("itemEx != item", "RemoveFormUsingList");
                errorCode = 3;
            }

            if (errorCode > 0)
            {
                bool success = mUsingList.Remove(item);
                if (!success)
                {
                    mLogger.LogError("remove error", "RemoveFormUsingList");
                }
                else
                {
                    item.Index = -1;
                    mLogger.LogInfo("remove success", "RemoveFormUsingList");
                }
                FixUsingList();
                return;
            }

            //下面是处理正常的节点，上面都是为了以防万一
            //下面这种删除方式会快一些，不用全部排序
            int removeIndex = mUsingList.Count - 1;
            PoolItem<T> lastItem = mUsingList[removeIndex];
            mUsingList.RemoveAt(removeIndex);

            if (lastItem != item)
            {
                lastItem.Index = item.Index;
                mUsingList[item.Index] = lastItem;
                item.Index = -1;
            }
        }

        /// <summary>
        /// 修正使用列表
        /// </summary>
        private void FixUsingList()
        {
            mLogger.LogInfo("开始修正使用者列表", "FixUsingList");
            int fixNum = 0;
            for (int i = 0; i < mUsingList.Count; i++)
            {
                PoolItem<T> item = mUsingList[i];
                if (item.Index != i)
                {
                    mLogger.LogInfo("找到一个索引错误index=" + item.Index + " => " + i, "FixUsingList");
                    item.Index = i;
                    fixNum++;
                }
            }

            mLogger.LogInfo("修复完毕，找到" + fixNum + "错误", "FixUsingList");
        }


        ////////////////////////////////////////////////////////////
        /// 下面是为了可以用foreach遍历已经使用的对象
        ////////////////////////////////////////////////////////////


        private int mIndex;
        
        public IEnumerator GetEnumerator()
        {
            mIndex = -1;
            return this;
        }

        public CacheItemInfo<T> Current
        {
            get
            {
                PoolItem<T> item = mUsingList[mIndex];
                mTmpInfo.Timeout = item.Timeout;
                mTmpInfo.Item = item.Item;
                return mTmpInfo;
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            mIndex++;
            return mIndex < mUsingList.Count;
        }

        public void Reset()
        {
            mIndex = -1;
        }

        public void Dispose()
        {
            //foreach结束时会调用 ，然而我这里并没有什么用
            mIndex = -1;
            mTmpInfo.Item = default(T);
            mTmpInfo.Timeout = -1;
        }
    }

    /// <summary>
    /// 专门用于遍历时，返回外部timeout
    /// @author fanflash.org
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheItemInfo<T>
    {
        public float Timeout;
        public T Item;
    }
}

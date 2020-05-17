using System;

namespace Core.Cache
{
    /// <summary>
    /// 缓存对象的默认实现
    /// @author fanflash.org
    /// </summary>
    public abstract class CacheItem:ICacheItem<CacheItem>
    {
        private Action mReturnFun;

        public float liveTime { get; protected set; }

        public void Initialize(Action returnFun, object[] initParams)
        {
            mReturnFun = returnFun;
            OnInit(initParams);
        }

        public void Return()
        {
            OnReturn();
            mReturnFun();
        }

        protected abstract void OnInit(object[] initParams);

        protected abstract void OnReturn();


    }
}

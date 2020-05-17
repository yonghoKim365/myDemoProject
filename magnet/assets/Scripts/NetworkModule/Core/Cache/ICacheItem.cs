using System;

namespace Core.Cache
{
    /// <summary>
    /// 缓存对象的接口
    /// @author fanflash.org
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICacheItem<T> where T:ICacheItem<T>
    {
        /// <summary>
        /// 生存时间
        /// </summary>
        float liveTime { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="returnFun"></param>
        /// <param name="initParams"></param>
        void Initialize(Action returnFun, object[] initParams);

        /// <summary>
        /// 使用完后调用这个
        /// 将返回到内存池
        /// </summary>
        void Return();
    }
}

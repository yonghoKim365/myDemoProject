using System.Collections.Generic;
namespace Core.Cache
{
    /// <summary>
    /// 一个通用的缓存项
    /// @author fanflash.org
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GeneralPool<T> where T : new()
    {
        /// <summary>
        /// 最大缓存数
        /// 值为0表示无限
        /// 默认值为0
        /// </summary>
        public uint MaxCount;

        /// <summary>
        /// 初始化时新建立的数量
        /// 这个值如果超过MaxCount的值，会被重置会MaxCount的值
        /// </summary>
        public uint InitCount;

        /// <summary>
        /// 空闲的资源库
        /// </summary>
        private readonly Stack<T> _freeLib;
        /// <summary>
        /// 已经使用的资源列表
        /// </summary>
        private readonly List<T> _useLib;

        public GeneralPool(uint initCount = 0, uint maxCount = 0)
        {
            MaxCount = maxCount;
            InitCount = initCount;

            _freeLib = new Stack<T>();
            _useLib = new List<T>();
        }

        /// <summary>
        /// 预生成缓存项
        /// </summary>
        public void Init()
        {
            if (InitCount == 0)
            {
                return;
            }

            if (MaxCount != 0 && InitCount > MaxCount)
            {
                InitCount = MaxCount;
            }

            int initCount = (int) InitCount - _freeLib.Count - _useLib.Count;
            for (int i = 0; i < initCount; i++)
            {
                T obj = NewItem(true);
                _freeLib.Push(obj);
            }
        } 

        /// <summary>
        /// 拿到一个缓存
        /// </summary>
        /// <returns></returns>
        public T Get()
        {

            T obj;
            if (_freeLib.Count == 0)
            {
                if (MaxCount > 0 && _useLib.Count >= MaxCount)
                {
                    return default(T);
                }

                obj = NewItem(false);
            }
            else
            {
                obj = _freeLib.Pop();
            }

            _useLib.Add(obj);
            GetAfter(obj);
            return obj;
        }

        /// <summary>
        /// 放回一个缓存
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Free(T obj)
        {
            FreeItem(obj);

            int index = _useLib.IndexOf(obj);
            if (index < 0)
            {
                return false;
            }

            _freeLib.Push(obj);
            int lastIndex = _useLib.Count - 1;
            T lastItem = _useLib[lastIndex];
            _useLib.RemoveAt(lastIndex);
            if (index != lastIndex)
            {
                _useLib[index] = lastItem;
            }
            return true;
        }

        /// <summary>
        /// 当池内对象不足以使用时，新建一个对象
        /// </summary>
        /// <param name="isInit">表示是否为初始化</param>
        /// <returns></returns>
        protected virtual T NewItem(bool isInit)
        {
            return new T();
        }

        /// <summary>
        /// 在返回之前，做一些初始化工作
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void GetAfter(T obj)
        {
            
        }

        /// <summary>
        /// 释放时，重置对象工作
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void FreeItem(T obj)
        {
            
        }
    }
}

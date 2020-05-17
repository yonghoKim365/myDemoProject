namespace Core
{
    /// <summary>
    /// IDisposer的实现
    /// 可以继承使用
    /// 如果只是用接口，可以直接复制使用
    /// @author fanflash.org
    /// </summary>
    public abstract class Disposer:IDisposer
    {
        public Disposer()
        {
            Disposed = false;
        }

        public bool Disposed
        {
            get; private set;
        }

        public bool Dispose()
        {
            if (Disposed)
            {
                return false;
            }

            Disposed = true;
            OnDispose();
            return true;
        }

        /// <summary>
        /// 处理释放相关事情
        /// </summary>
        protected abstract void OnDispose();
    }
}
using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 用于单例类继承
    /// @author fanflahs.org
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonNew<T>:Disposer where T :SingletonNew<T>,new()
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    new T();
                }

                return mInstance;
            }
        }

        public SingletonNew()
        {
            if (mInstance != null)
            {
#if !UNITY_EDITOR
                return;
#else
                throw  new Exception(this + "Is a singleton class that can not be instantiated twice");
#endif
            }

            mInstance = (T)this;
        }


        protected override void OnDispose()
        {
            mInstance = default(T);
        }
    }
}

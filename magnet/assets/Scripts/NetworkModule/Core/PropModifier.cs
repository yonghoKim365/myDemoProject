using System;
using System.Collections.Generic;
namespace Core
{

    /// <summary>
    /// 属性修改器
    /// 用于解决c#访问控制灵活度不足的问题
    /// @author fanflash.org
    /// </summary>
    public class PropModifier:Disposer
    {
        private Dictionary<string, object> mChangeDic;
        private bool mHasChange;

        public PropModifier()
        {
            mChangeDic = new Dictionary<string, object>();
            mHasChange = false;
        }

        public void Set(string propName, object value)
        {
            mChangeDic[propName] = value;
            mHasChange = true;
        }

        public void Change(Action<Dictionary<string,object>> changeFun)
        {
            if (!mHasChange)
            {
                return;
            }

            if (changeFun == null)
            {
                return;
            }

            changeFun(mChangeDic);
            mChangeDic.Clear();
            mHasChange = false;
        }

        protected override void OnDispose()
        {
            mChangeDic.Clear();
            mChangeDic = null;
            mHasChange = false;
        }
    }
}

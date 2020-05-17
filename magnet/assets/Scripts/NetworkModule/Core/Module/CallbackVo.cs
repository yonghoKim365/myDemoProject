using System;
using System.Collections.Generic;
using Core.Utils;

namespace Core.Module
{
    /// <summary>
    /// 模块中能用的回调参数
    /// @anthro fanflash.org
    /// </summary>
    public class CallbackVo:Disposer,IComparable<CallbackVo>
    {
        /// <summary>
        /// 回调类型
        /// </summary>
        public string Type { protected set; get; }

        /// <summary>
        /// 事件所附带的执行函数
        /// </summary>
        public Action<CallbackVo> Callback { protected set; get; }

        /// <summary>
        /// 事件附带的数据
        /// </summary>
        public object Data { protected set; get; }

        /// <summary>
        /// 发送这个事件的目标对象
        /// </summary>
        public object Target { protected set; get; }

        /// <summary>
        /// 同类回调执行时的优先级
        /// </summary>
        public uint Level {protected set; get; }

        /// <summary>
        /// 是否会自动移除
        /// </summary>
        public bool AutoRemove { protected set; get; }

        //是否充许事件在中途被取消
        public bool Cancelable { protected set; get; }

        /// <summary>
        /// 是否被阻止了
        /// </summary>
        public bool IsDefaultPrevented { protected set; get; }

        /// <summary>
        /// 是否要删除
        /// </summary>
        public bool TobeRemove { protected set; get; }

        /// <summary>
        /// 属性修改器
        /// </summary>
        protected PropModifier mModifier;

        public CallbackVo(string type, Action<CallbackVo> callback, object data = null, object target = null, uint level = 0, bool autoRemove = false, bool cancelable = false, PropModifier modifier = null)
        {
            Type = type;
            Callback = callback;
            Data = data;
            Target = target;
            Level = level;
            AutoRemove = autoRemove;
            Cancelable = cancelable;
            mModifier = modifier;
        }

        /// <summary>
        /// 取消后续的事件处理
        /// </summary>
        public void PreventDefault()
        {
            if (Cancelable)
            {
                IsDefaultPrevented = true;
            }
        }

        protected override void OnDispose()
        {
            Level = 0;
            Callback = null;
            AutoRemove = false;
            Type = null;
            Target = null;
            Data = null;
            IsDefaultPrevented = false;
            Cancelable = false;
        }

        /// <summary>
        /// 主要是为了跟据level来对所有的callbackVo对象排序
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(CallbackVo other)
        {
            return (int)(Level - other.Level);
        }

        /// <summary>
        /// 跟据修改器的值修改
        /// </summary>
        public void Modify()
        {
            if (mModifier == null)
            {
                return;
            }

            mModifier.Change(ModifyHandler);
        }

        /// <summary>
        /// 是否标记为要移除
        /// 一但标记，就不可以修改
        /// </summary>
        public void MarkRemove()
        {
            TobeRemove = true;
        }

        protected void ModifyHandler(Dictionary<string, object> changeDic)
        {
            if (changeDic.ContainsKey("Type"))
            {
                Type = (string)changeDic["Type"];
            }

            if (changeDic.ContainsKey("Data"))
            {
                Data = changeDic["Data"];
            }

            if (changeDic.ContainsKey("IsDefaultPrevented"))
            {
                IsDefaultPrevented = (bool) changeDic["IsDefaultPrevented"];
            }
        }

        /// <summary>
        /// 按类型得到一个参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="data">输出的数据</param>
        /// <returns>错误信息，没有错误返回""</returns>
        public string GetParams<T>(out T data)
        {
            return ParamsHelper.GetParams(Data,out data);
        }

        /// <summary>
        /// 按类型得到两个参数
        /// </summary>
        /// <typeparam name="T0">第一个参数的类型</typeparam>
        /// <typeparam name="T1">第二个参数的类型</typeparam>
        /// <param name="data0">输出的第一个参数</param>
        /// <param name="data1">输出的第二个参数</param>
        /// <returns>错误信息，没有错误返回""</returns>
        public string GetParams<T0, T1>(out T0 data0, out T1 data1)
        {
            object tData = Data;
            if (!(tData is object[]))
            {
                data0 = default(T0);
                data1 = default(T1);
                return "Data is null";
            }

            return ParamsHelper.GetParams((object[]) tData, out data0, out data1);
        }

        /// <summary>
        /// 按类型得到三个参数
        /// </summary>
        /// <typeparam name="T0">第一个参数类型</typeparam>
        /// <typeparam name="T1">第二个参数类型</typeparam>
        /// <typeparam name="T2">第三个参数类型</typeparam>
        /// <param name="data0">输出的第一个参数</param>
        /// <param name="data1">输出的第二个参数</param>
        /// <param name="data2">输出的第三个参数</param>
        /// <returns>错误信息，没有错误返回""</returns>
        public string GetParams<T0, T1, T2>(out T0 data0, out T1 data1, out T2 data2)
        {
            object tData = Data;
            if (!(tData is object[]))
            {
                data0 = default(T0);
                data1 = default(T1);
                data2 = default(T2);
                return "Data is null";
            }

            return ParamsHelper.GetParams((object[])tData, out data0, out data1,out data2);
        }

        /// <summary>
        /// 按类型得到四个参数
        /// </summary>
        /// <typeparam name="T0">第一个参数类型</typeparam>
        /// <typeparam name="T1">第二个参数类型</typeparam>
        /// <typeparam name="T2">第三个参数类型</typeparam>
        /// <typeparam name="T3">第四个参数类型</typeparam>
        /// <param name="data0">输出的第一个参数</param>
        /// <param name="data1">输出的第二个参数</param>
        /// <param name="data2">输出的第三个参数</param>
        /// <param name="data3">输出的第四个参数</param>
        /// <returns>错误信息，没有错误返回""</returns>
        public string GetParams<T0, T1, T2, T3>(out T0 data0, out T1 data1, out T2 data2, out T3 data3)
        {
            object tData = Data;
            if (!(tData is object[]))
            {
                data0 = default(T0);
                data1 = default(T1);
                data2 = default(T2);
                data3 = default(T3);
                return "Data is null";
            }

            return ParamsHelper.GetParams((object[])tData, out data0, out data1, out data2, out data3);

        }
    }
}

using System;

namespace Core.Utils
{
    /// <summary>
    /// 基于Object的多参数读取工具
    /// @author fanflash.org
    /// </summary>
    public static class ParamsHelper
    {
        /// <summary>
        /// 尝试得到参数
        /// </summary>
        /// <typeparam name="T">参数类形</typeparam>
        /// <param name="inData">输入的数据</param>
        /// <param name="outData">输出的数据</param>
        /// <param name="error">错误信息</param>
        /// <returns>没有错误信息，返回true, 有错误信息，返回false</returns>
        private static bool TryGetData<T>(object inData, out T outData, ref string error)
        {
            try
            {
                if (inData == null)
                {
                    outData = default(T);
                }
                else
                {
                    outData = (T)inData;
                }
                return true;
            }
            catch (Exception ex)
            {
                outData = default(T);

                if (string.IsNullOrEmpty(error))
                {
                    error += "\n" + ex.Message;
                }
                else
                {
                    error = ex.Message;
                }
                return false;
            }
        }

        /// <summary>
        /// 按类型得到一个参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="inData">输入的参数</param>
        /// <param name="data">输出的数据</param>
        /// <returns>错误信息，没有错误返回""</returns>
        public static string GetParams<T>(object inData, out T data)
        {
            if (inData == null)
            {
                data = default(T);
                return "Data is null";
            }

            if (inData is object[])
            {
                inData = (inData as object[])[0];
            }

            string error = "";
            TryGetData(inData, out data, ref error);
            return error;
        }

        /// <summary>
        /// 按类型得到两个参数
        /// </summary>
        /// <typeparam name="T0">第一个参数的类型</typeparam>
        /// <typeparam name="T1">第二个参数的类型</typeparam>
        /// <param name="paramList">参数列表</param>
        /// <param name="data0">输出的第一个参数</param>
        /// <param name="data1">输出的第二个参数</param>
        /// <returns>错误信息，没有错误返回""</returns>
        public static string GetParams<T0, T1>(object[] paramList, out T0 data0, out T1 data1)
        {
            if (paramList.Length != 2)
            {
                data0 = default(T0);
                data1 = default(T1);
                return "paramList.Length != 2";
            }

            string error = "";
            TryGetData(paramList[0], out data0, ref error);
            TryGetData(paramList[1], out data1, ref error);
            return error;
        }

        /// <summary>
        /// 按类型得到三个参数
        /// </summary>
        /// <typeparam name="T0">第一个参数类型</typeparam>
        /// <typeparam name="T1">第二个参数类型</typeparam>
        /// <typeparam name="T2">第三个参数类型</typeparam>
        /// <param name="paramList">参数列表</param>
        /// <param name="data0">输出的第一个参数</param>
        /// <param name="data1">输出的第二个参数</param>
        /// <param name="data2">输出的第三个参数</param>
        /// <returns>错误信息，没有错误返回""</returns>
        public static string GetParams<T0, T1, T2>(object[] paramList, out T0 data0, out T1 data1, out T2 data2)
        {
            if (paramList.Length != 3)
            {
                data0 = default(T0);
                data1 = default(T1);
                data2 = default(T2);
                return "paramList.Length != 3";
            }

            string error = "";
            TryGetData(paramList[0], out data0, ref error);
            TryGetData(paramList[1], out data1, ref error);
            TryGetData(paramList[2], out data2, ref error);
            return error;
        }

        /// <summary>
        /// 按类型得到四个参数
        /// </summary>
        /// <typeparam name="T0">第一个参数类型</typeparam>
        /// <typeparam name="T1">第二个参数类型</typeparam>
        /// <typeparam name="T2">第三个参数类型</typeparam>
        /// <typeparam name="T3">第四个参数类型</typeparam>
        /// <param name="paramList">参数列表</param>
        /// <param name="data0">输出的第一个参数</param>
        /// <param name="data1">输出的第二个参数</param>
        /// <param name="data2">输出的第三个参数</param>
        /// <param name="data3">输出的第四个参数</param>
        /// <returns>错误信息，没有错误返回""</returns>
        public static string GetParams<T0, T1, T2, T3>(object[] paramList, out T0 data0, out T1 data1, out T2 data2, out T3 data3)
        {
            if (paramList.Length != 4)
            {
                data0 = default(T0);
                data1 = default(T1);
                data2 = default(T2);
                data3 = default(T3);
                return "paramList.Length != 4";
            }

            string error = "";
            TryGetData(paramList[0], out data0, ref error);
            TryGetData(paramList[1], out data1, ref error);
            TryGetData(paramList[2], out data2, ref error);
            TryGetData(paramList[3], out data3, ref error);
            return error;
        }
    }
}

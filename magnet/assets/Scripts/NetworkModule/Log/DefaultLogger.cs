using System;
using UnityEngine;

namespace Log
{
    /// <summary>
    /// 默认日志
    /// @author fanflash.org
    /// </summary>
    public class DefaultLogger:ILogger
    {
        private string mName;
        public DefaultLogger(string name = null)
        {
            if (name == null)
            {
                mName = "";
            }
            else
            {
                mName = name;
            }
        }

        public void LogInfo(string msg, string path = null)
        {
            Log("Info", msg, path);
        }

        public void LogDebug(string msg, string path = null)
        {
            Log("Debug", msg, path);
        }

        public void LogWarning(string msg, string path = null)
        {
            Log("Warning", msg, path);
        }

        public void LogError(string msg, string path = null)
        {
            Log("Error",msg,path);
        }

        private void Log(string type, string msg, string path)
        {
            string head = type + " path=" + mName;
            if (!string.IsNullOrEmpty(path))
            {
                head += "." + path;
            }

            string log = String.Format("[{0} t={1}]:{2}", head, DateTime.Now.TimeOfDay, msg);

            switch (type)
            {
                case "Warning":
                    Debug.LogWarning(log);
                    break;
                case "Error":
                    Debug.LogError(log);
                    break;
                default:
                    Debug.Log(log);
                    break;
            }
        }
    }
}

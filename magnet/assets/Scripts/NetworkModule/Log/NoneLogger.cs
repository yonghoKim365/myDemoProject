namespace Log
{
    /// <summary>
    /// 用于替代没有设置日志器有情况
    /// @author fanflash.org
    /// </summary>
    public class NoneLogger:ILogger
    {
        public void LogInfo(string msg, string path = null)
        {
        }

        public void LogDebug(string msg, string path = null)
        {
        }

        public void LogWarning(string msg, string path = null)
        {
        }

        public void LogError(string msg, string path = null)
        {
        }
    }
}

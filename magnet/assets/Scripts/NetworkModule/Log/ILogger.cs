namespace Log
{
    /// <summary>
    /// 日志接口
    /// @author fanflash.org
    /// </summary>
    public interface ILogger
    {
        void LogInfo(string msg, string path = null);
        void LogDebug(string msg, string path = null);
        void LogWarning(string msg, string path = null);
        void LogError(string msg, string path = null);

    }
}

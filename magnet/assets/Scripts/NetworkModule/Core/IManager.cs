using UnityEngine;

namespace Core
{
    /// <summary>
    /// 通用管理类接口
    /// @author fanflash.org
    /// </summary>
    public interface IManager:IDisposer
    {
        void Initialize(MonoBehaviour mb);
        void Update();
    }
}

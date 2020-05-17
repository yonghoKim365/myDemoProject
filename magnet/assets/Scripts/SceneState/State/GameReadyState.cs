using UnityEngine;
using System.Collections;
using PathologicalGames;

public class GameReadyState : SceneStateBase
{
    string StageName = "GameReadyScene";
    public static _ACTION NextAction = _ACTION.PLAY_SINGLE;
    public override void OnEnter(System.Action callback)
    {
        Debug.LogError("ASSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS " + NextAction);
        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () => 
        {
            CameraManager.instance.RtsCamera.Reset();
            base.OnEnter(callback);

            if (!PoolManager.Pools.ContainsKey("Effect"))
                PoolManager.Pools["Effect"].DespawnAll();
            if (!PoolManager.Pools.ContainsKey("Projectile"))
                PoolManager.Pools["Projectile"].DespawnAll();

            LoadLevelAsync(StageName);
        });
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);
    }

    void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName == StageName)
        {
            SceneSetting();
            SetupMainCamera(false);

            //< 인게임에서 사용한 리소스 삭제
            //AssetbundleLoader.ClearAssetList();

            //< 다음 씬으로 이동
            SceneManager.instance.ActionEvent(NextAction);
        }
    }
}

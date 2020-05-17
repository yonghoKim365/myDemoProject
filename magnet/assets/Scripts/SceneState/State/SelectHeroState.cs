using UnityEngine;
using System.Collections;
using System;

public class SelectHeroState : SceneStateBase
{
    public override void OnEnter(Action callback)
    {
        base.OnEnter(callback);

        LoadLevelAsync("SelectHeroScene");
    }

    public override void OnExit(Action callback)
    {
        CameraManager.instance.mainCamera.fieldOfView = 40;
        
        base.OnExit(callback);
    }

    void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName != "SelectHeroScene") return;

        NetData.instance.GetUserInfo().ClearData();
        //캐릭터 선택 패널 띄우기
        UIMgr.Open("UIPanel/SelectHeroPanel", true);
        //선택페널에서 캐릭터 선택이 이뤄지면 


        //CameraManager.instance.mainCamera.transform.position = new Vector3(3.21f, 2.93f, 19.47f);
        //CameraManager.instance.mainCamera.transform.eulerAngles = new Vector3(6.827481f, -157.9204f, 2f);
        //CameraManager.instance.mainCamera.fieldOfView = 47;
        CameraManager.instance.transform.position = Vector3.zero;
        CameraManager.instance.transform.eulerAngles = Vector3.zero;
        CameraManager.instance.mainCamera.transform.position = new Vector3(4.02f, 1.98f, 20.85f);
        CameraManager.instance.mainCamera.transform.eulerAngles = new Vector3(6.827481f, -157.9204f, 2f);
        CameraManager.instance.mainCamera.fieldOfView = 47;
    }
}
